import requests

API = "http://localhost:5062/api"

def test_admin_login_and_actions():
    # Step 1: Login as admin
    res = requests.post(f"{API}/auth/login", json={
        "username": "admin",
        "password": "password"
    })
    assert res.status_code == 200
    token = res.json()["token"]
    print("Admin login passed")

    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }

    # Step 3: Send notification
    res = requests.post(f"{API}/notification/send", headers=headers, json={
        "userId": 1,
        "message": "Test notification from admin script"
    })
    assert res.status_code == 200
    print("Send notification passed")

if __name__ == "__main__":
    test_admin_login_and_actions()
