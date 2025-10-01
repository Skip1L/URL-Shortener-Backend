import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";

interface Url {
    id: string;
    originalUrl: string;
    shortCode: string;
    createdBy: string;
}

function UrlsTableView() {
    const [urls, setUrls] = useState<Url[]>([]);
    const [newUrl, setNewUrl] = useState("");

    const fetchUrls = async () => {
        const res = await fetch("/api/urls");
        if (res.ok) {
            setUrls(await res.json());
        }
    };

    useEffect(() => {
        fetchUrls();
    }, []);

    const handleAdd = async () => {
        const token = localStorage.getItem("token");
        const res = await fetch("/api/urls", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`
            },
            body: JSON.stringify({ originalUrl: newUrl })
        });

        if (res.ok) {
            setNewUrl("");
            fetchUrls(); // auto refresh
        } else {
            alert("Error: URL already exists or unauthorized");
        }
    };

    const handleDelete = async (id: string) => {
        const token = localStorage.getItem("token");
        const res = await fetch(`/api/urls/${id}`, {
            method: "DELETE",
            headers: { Authorization: `Bearer ${token}` }
        });

        if (res.ok) {
            fetchUrls();
        } else {
            alert("You can't delete this record");
        }
    };

    return (
        <div className="container my-5">
            <h3>Shortened URLs</h3>

            {localStorage.getItem("token") && (
                <div className="input-group mb-3">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Enter URL"
                        value={newUrl}
                        onChange={(e) => setNewUrl(e.target.value)}
                    />
                    <button className="btn btn-success" onClick={handleAdd}>Shorten</button>
                </div>
            )}

            <table className="table table-striped table-hover align-middle">
                <thead className="table-light">
                    <tr>
                        <th style={{ width: "40%" }}>Original URL</th>
                        <th style={{ width: "20%" }}>Short</th>
                        <th style={{ width: "20%" }}>Created By</th>
                        <th style={{ width: "20%" }}></th>
                    </tr>
                </thead>
                <tbody>
                    {urls.map((u) => (
                        <tr key={u.id}>
                            <td title={u.originalUrl} style={{ maxWidth: "250px", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                                {u.originalUrl}
                            </td>
                            <td>
                                <a href={`/${u.shortCode}`} target="_blank" rel="noreferrer">{u.shortCode}</a>
                            </td>
                            <td>{u.createdBy}</td>
                            <td>
                                <Link to={`/urls/${u.shortCode}`} className="btn btn-sm btn-info me-1">Info</Link>
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(u.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default UrlsTableView;
