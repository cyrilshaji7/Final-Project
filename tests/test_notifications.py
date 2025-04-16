import requests

API = "http://localhost:5062/api"

def test_get_user_notifications():
    user_id = 1
    res = requests.get(f"{API}/notification/user/{user_id}")
    assert res.status_code == 200
    data = res.json()
    assert isinstance(data, list)
    print("Load user notifications passed")

if __name__ == "__main__":
    test_get_user_notifications()
