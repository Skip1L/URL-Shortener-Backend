import React, { useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext"
import { useNavigate } from "react-router-dom";

function LoginView() {
    const [userName, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const { login } = useContext(AuthContext);
    const navigate = useNavigate();

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();

        const response = await fetch("/api/account/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userName, password })
        });

        if (response.ok) {
            const data = await response.json();
            localStorage.setItem("token", data.token);
            login(data.token);
            navigate("/");
        } else {
            alert("Invalid credentials");
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
            <div className="col-md-5">
                <div className="card shadow-lg">
                    <div className="card-header bg-primary text-white text-center">
                        <h3 className="mb-0">Login</h3>
                    </div>
                    <div className="card-body p-4">
                        <form onSubmit={handleLogin}>
                            <div className="mb-3">
                                <label className="form-label fw-bold">Username</label>
                                <input
                                    className="form-control"
                                    type="text"
                                    placeholder="Enter username"
                                    value={userName}
                                    onChange={(e) => setLogin(e.target.value)}
                                    required
                                />
                            </div>
                            <div className="mb-3">
                                <label className="form-label fw-bold">Password</label>
                                <input
                                    className="form-control"
                                    type="password"
                                    placeholder="Enter password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    required
                                />
                            </div>
                            <button className="btn btn-primary w-100">
                                ?? Login
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );

}

export default LoginView;
