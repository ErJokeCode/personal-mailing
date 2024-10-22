import dotenv
import requests
import os

dotenv.load_dotenv()

TOKEN = os.getenv("TOKEN")
URL_SERVER = os.getenv("URL_SERVER")
URL_REDIS = os.getenv("URL_REDIS")

COOKIE = ""


def get_cookie():
    global COOKIE

    if COOKIE == "":
        url = "http://core:5000/login/"
        body = {
            "email": "admin",
            "password": "admin",
        }

        session = requests.Session()
        response = session.post(url, json=body)
        name = ".AspNetCore.Identity.Application"
        COOKIE = name + "=" + response.cookies.get_dict()[name]

    return COOKIE
