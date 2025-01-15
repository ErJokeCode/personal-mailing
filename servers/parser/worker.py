import requests
from config import ADMIN_EMAIL_CORE, ADMIN_PASSWORD_CORE, worker_db, URL_CORE
from src.schemas import HistoryUploadFileInDB


COOKIE_CORE = ""

def get_cookie_core():
    global COOKIE_CORE

    if COOKIE_CORE == "":
        url = "http://core:5000/login/"
        body = {
            "email": ADMIN_EMAIL_CORE,
            "password": ADMIN_PASSWORD_CORE,
        }

        session = requests.Session()
        response = session.post(url, json=body)
        name = ".AspNetCore.Identity.Application"
        COOKIE_CORE = name + "=" + response.cookies.get_dict()[name]

    return COOKIE_CORE

def update_status_history(hist_info_db: HistoryUploadFileInDB, text_status: str ):
    hist_info_db.status_upload = text_status
    worker_db.history.update_one(hist_info_db, get_item=False)
    
    headers = {"cookie": f"{get_cookie_core()}"}
    url = URL_CORE + "/core/data/uploadEvent"
    
    session = requests.Session()
    response = session.post(url, headers=headers)
    
    if response.status_code != 200:
        print(response.status_code)