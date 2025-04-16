import requests

API = "http://localhost:5062/api"
ADMIN_EMAIL = "admin"
ADMIN_PASSWORD = "password"

def get_token(email, password):
    res = requests.post(f"{API}/auth/login", json={
        "username": email,
        "password": password
    })
    res.raise_for_status()
    return res.json()["token"]

def test_add_book():
    token = get_token(ADMIN_EMAIL, ADMIN_PASSWORD)

    res = requests.post(f"{API}/book/add", headers={
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }, json={
        "title": "The Automated Testing Bible",
        "author": "QA Bot",
        "genre": "Tech"
    })

    assert res.status_code == 200
    print("Book added successfully.")

if __name__ == "__main__":
    test_add_book()
