document.getElementById("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    const user = document.getElementById("username").value;
    const pass = document.getElementById("password").value;
    const success = await login(user, pass);
    document.getElementById("output").innerText = success ? "✅ Logged in" : "❌ Login failed";
});

document.getElementById("notifyForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    const userId = document.getElementById("userId").value;
    const message = document.getElementById("message").value;
    const token = getToken();

    const res = await fetch(`${API_BASE}/api/notification/send`, {
        method: "POST",
        headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ userId, message })
    });

    const result = await res.json();
    document.getElementById("output").innerText = res.ok
        ? `✅ ${result.message}`
        : "❌ Failed to send notification";
});
