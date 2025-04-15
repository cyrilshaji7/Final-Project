document.getElementById("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    const user = document.getElementById("username").value;
    const pass = document.getElementById("password").value;
    const success = await login(user, pass);
    document.getElementById("output").innerText = success ? "Logged in" : "Login failed";
});

document.getElementById("loadBtn").addEventListener("click", async () => {
    const token = getToken();
    const userId = prompt("Enter your user ID");

    const res = await fetch(`${API_BASE}/api/notification/user/${userId}`, {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    if (!res.ok) {
        document.getElementById("output").innerText = "Failed to fetch notifications";
        return;
    }

    const notifications = await res.json();
    const list = document.getElementById("notificationsList");
    list.innerHTML = "";

    notifications.forEach(n => {
        const li = document.createElement("li");
        li.textContent = `${n.message} — ${n.sentAt}`;
        list.appendChild(li);
    });
});
