const API_BASE = "http://localhost:5062";

async function login(username, password) {
    const res = await fetch(`${API_BASE}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    });

    const data = await res.json();
    if (data.token) {
        localStorage.setItem("token", data.token);
        return true;
    }
    return false;
}

function getToken() {
    return localStorage.getItem("token");
}
