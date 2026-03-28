import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import styles from './Login.module.css';
import PassIn from '../components/PassInput'
import UsernameInput from '../components/EmailInput'

export default function SignUp() {
    const navigate = useNavigate();
    const location = useLocation();
    const returnUrl =
        new URLSearchParams(location.search).get("returnUrl") || "/";

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    // Check if already logged in
    useEffect(() => {
        const token = localStorage.getItem("authToken");
        if (token) {
            navigate(returnUrl, { replace: true });
        }
    }, [navigate, returnUrl]);

    function validate() {
        if (!username.trim()) return "Username is required.";
        if (username.trim().length < 3) return "Username must be at least 3 characters.";
        if (!password) return "Password is required.";
        if (password.length < 6) return "Password must be at least 6 characters.";
        return "";
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");
        const validationError = validate();
        if (validationError) {
            setError(validationError);
            return;
        }

        setLoading(true);
        try {
            const resp = await fetch("https://localhost:61954/api/Authentication/Register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ name: username.trim(), password }),
            });

            if (!resp.ok) {
                const payload = await resp.json().catch(() => ({}));
                throw new Error(payload?.message || "Sign up failed");
            }

            // Registration successful, now log in
            const loginResp = await fetch("https://localhost:61954/api/Authentication/Login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ name: username.trim(), password }),      
            });

            if (!loginResp.ok) {
                throw new Error("Registration successful, but login failed. Please log in manually.");
            }

            const data = await loginResp.json();
            if (!data?.accessToken) throw new Error("Invalid server response.");

            localStorage.setItem("authToken", data.accessToken);
            if (data.refreshToken) {
                localStorage.setItem("refreshToken", data.refreshToken);
            }
            navigate(returnUrl, { replace: true });
        } catch (err) {
            setError(err.message || "An unexpected error occurred.");
        } finally {
            setLoading(false);
        }
    }

    const handleEnter = () => {
        document.querySelector('form')?.requestSubmit();
    };

    return (
        <>
        <img className={styles.farmer} src="/public/farmer2.jpg"></img>,
            <div className={styles.container}>
                <span className={styles.brand}>🌿 Root & Rot</span>
            <a href="/login" className={styles.signup}>
                Have an account? Sign in
            </a>
            <form onSubmit={handleSubmit} className={styles.form} aria-label="signup form">
                <h2 className={styles.title}>Sign up</h2>

                {error && <div role="alert" className={styles.error}>{error}</div>}

                <UsernameInput
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    onEnter={handleEnter}
                />

                <PassIn
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    onEnter={handleEnter}
                />

                <button type="submit" className={styles.button} disabled={loading}>
                    {loading ? "Signing up..." : "Sign up"}
                </button>

            </form>
            </div>
        </>
    );
}