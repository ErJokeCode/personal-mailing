import requests
from config import worker_db, URL_CORE, SECRET_TOKEN
from src.schemas import HistoryUploadFileInDB

def update_status_history(hist_info_db: HistoryUploadFileInDB, text_status: str ):
    hist_info_db.status_upload = text_status
    worker_db.history.update_one(hist_info_db, get_item=False)
    
    headers = {"Authorization": f"Basic {SECRET_TOKEN}"}
    url = URL_CORE + "/core/data/uploadEvent"
    
    session = requests.Session()
    response = session.post(url, headers=headers)
    
    if response.status_code != 200:
        print(response.status_code)