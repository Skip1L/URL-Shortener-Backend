import { useContext } from "react";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import LoginView from "./pages/LoginView";
import UrlsTableView from "./pages/UrlsTableView";
import UrlInfoView from "./pages/UrlInfoView";
import AboutView from "./pages/AboutView";
import { AuthProvider, AuthContext } from "./context/AuthContext";
import RegisterView from "./pages/RegisterView";


function AppContent() {
    const { isAuthenticated, logout } = useContext(AuthContext);

    return (
        <>
            <nav className="navbar navbar-expand-lg navbar-light bg-light shadow-sm">
                <div className="container">
                    <Link className="navbar-brand fw-bold" to="/">URL Shortener</Link>

                    <button
                        className="navbar-toggler"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#navbarNav"
                        aria-controls="navbarNav"
                        aria-expanded="false"
                        aria-label="Toggle navigation"
                    >
                        <span className="navbar-toggler-icon"></span>
                    </button>

                    <div className="collapse navbar-collapse" id="navbarNav">
                        <ul className="navbar-nav me-auto">
                            <li className="nav-item">
                                <Link className="nav-link" to="/">Home</Link>
                            </li>

                            <li className="nav-item">
                                <Link className="nav-link" to="/about">About</Link>
                            </li>


                            <li className="nav-item">
                                <Link className="nav-link" to="/urls">Short URLs</Link>
                            </li>

                        </ul>

                        <ul className="navbar-nav">
                            {!isAuthenticated ? (
                                <>
                                    <li className="nav-item">
                                        <Link className="nav-link" to="/login">Login</Link>
                                    </li>
                                    <li className="nav-item">
                                        <Link className="nav-link" to="/register">Register</Link>
                                    </li>
                                </>
                            ) : (
                                <li className="nav-item">
                                    <button className="btn btn-outline-danger" onClick={logout}>
                                        Logout
                                    </button>
                                </li>
                            )}
                        </ul>
                    </div>
                </div>
            </nav>

            <div className="container mt-4">
                <Routes>
                    <Route path="/" element={<UrlsTableView />} />
                    <Route path="/login" element={<LoginView />} />
                    <Route path="/register" element={<RegisterView />} />
                    <Route path="/urls" element={<UrlsTableView />} />
                    <Route path="/urls/:shortCode" element={<UrlInfoView />} />
                    <Route path="/about" element={<AboutView />} />
                </Routes>
            </div>
        </>
    );
}

export default function App() {
    return (
        <AuthProvider>
            <Router>
                <AppContent />
            </Router>
        </AuthProvider>
    );
}