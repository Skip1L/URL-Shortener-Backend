import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";

interface UrlInfo {
    originalUrl: string;
    shortCode: string;
    createdBy: string;
    createdAt: string;
}

function UrlInfoView() {
    const { shortCode } = useParams();
    const [info, setInfo] = useState<UrlInfo | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const token = localStorage.getItem("token");
        setLoading(true);
        fetch(`/api/urls/info/${shortCode}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        })
            .then(async (res) => {
                if (!res.ok) {
                    const data = await res.json();
                    throw new Error(data?.error || "Failed to fetch URL info");
                }
                return res.json();
            })
            .then((data) => {
                setInfo(data);
                setError(null);
            })
            .catch((err) => setError(err.message))
            .finally(() => setLoading(false));
    }, [shortCode]);

    if (loading) return <p>Loading...</p>;
    if (error) return <div className="alert alert-danger">{error}</div>;
    if (!info) return null;

    return (
        <div className="container my-5">
            <h3 className="mb-4">URL Information</h3>

            <div className="card">
                <div className="card-body">
                    <p>
                        <b>Original URL:</b>{" "}
                        <a href={info.originalUrl} target="_blank" rel="noreferrer"
                            title={info.originalUrl}
                            style={{ maxWidth: "100%", display: "inline-block", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                            {info.originalUrl}
                        </a>
                    </p>
                    <p><b>Short Code:</b> {info.shortCode}</p>
                    <p><b>Created By:</b> {info.createdBy}</p>
                    <p><b>Created At:</b> {new Date(info.createdAt).toLocaleString()}</p>

                    <Link to="/urls" className="btn btn-secondary mt-3">
                        Back to URLs
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default UrlInfoView;
