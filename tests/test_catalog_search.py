import requests

API = "http://localhost:5062/api"

def test_search_books():
    res = requests.get(f"{API}/book/search?query=Zahan")
    assert res.status_code == 200
    data = res.json()
    assert isinstance(data, list)
    print("Search books passed")

if __name__ == "__main__":
    test_search_books()
