import React, { useEffect, useState } from "react";
import * as jwt_decode from "jwt-decode";

interface JwtPayload {
    email: string;
    role: string | string[];
    // ³םר³ ןמכÿ
}

function AboutView() {
    const [text, setText] = useState("");
    const [isAdmin, setIsAdmin] = useState<boolean>(localStorage.getItem("role") === "Admin");

    useEffect(() => {
        fetch("/api/about")
            .then((res) => res.text())
            .then(setText);

        const token = localStorage.getItem("token");
        if (token) {
            const decoded = jwt_decode.jwtDecode<JwtPayload>(token);
            if (Array.isArray(decoded.role)) {
                setIsAdmin(decoded.role.includes("Admin"));
            } else {
                setIsAdmin(decoded.role === "Admin");
            }
        }
    }, []);

    const handleSave = async () => {
        const token = localStorage.getItem("token");
        if (!token) return;

        const res = await fetch("/api/about", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify({ text }),
        });

        if (res.ok) alert("Saved!");
        else alert("Failed to save!");
    };

    return (
        <div className="container mt-5">
            <div className="card shadow-lg border-0">
                <div className="card-header bg-primary text-white">
                    <h3 className="mb-0">About URL Shortener Algorithm</h3>
                </div>
                <div className="card-body">
                    {isAdmin ? (
                        <>
                            <p className="text-muted mb-3">
                                You can edit the description of the algorithm below:
                            </p>
                            <textarea
                                className="form-control mb-3"
                                rows={8}
                                value={text}
                                onChange={(e) => setText(e.target.value)}
                            />
                            <button
                                className="btn btn-success"
                                onClick={handleSave}
                            >
                                ?? Save
                            </button>
                        </>
                    ) : (
                        <div className="lead">
                            {text}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default AboutView;
