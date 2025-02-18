import dotenv
import requests
import os

dotenv.load_dotenv()

TOKEN = os.getenv("TOKEN")
TOKEN_CHAT_CURATOR = os.getenv("TOKEN_CHAT_CURATOR")
URL_CORE = os.getenv("URL_CORE")
URL_REDIS = os.getenv("URL_REDIS")
SECRET_TOKEN = os.getenv("SECRET_TOKEN")