from datetime import datetime
from fastapi import APIRouter, HTTPException, UploadFile
from config import s3_client, worker_db
from src.upload.dict_names import upload_dict_names
from src.upload.online_course import parse_info_online_courses, upload_report
from src.schemas import HistoryUploadFile, HistoryUploadFileInDB, TypeFile

from src.upload.student import upload_student
from src.upload.modeus import upload_modeus


router_data = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)

@router_data.post("/student")
async def upload_data_student(file: UploadFile) -> HistoryUploadFileInDB:
    hist_info = await s3_client.create_hist_file(file, TypeFile.student)
    
    res = worker_db.history.insert_one(hist_info)
    
    upload_student(hist_info.link, worker_db)
    
    return res

@router_data.post("/choice_in_modeus")
async def post_choice_in_modeus(file: UploadFile):
    hist_info = await s3_client.create_hist_file(file, TypeFile.modeus)
    
    res = worker_db.history.insert_one(hist_info)
    
    upload_modeus(hist_info.link, worker_db)
   
    return res

@router_data.post("/report_online_course")
async def post_online_course_report(file: UploadFile):
    hist_info = await s3_client.create_hist_file(file, TypeFile.online_course)
    
    res = worker_db.history.insert_one(hist_info)
    
    res = parse_info_online_courses(worker_db)
    if res["status"] == "success":
        res_upload = upload_report(hist_info.link, worker_db)

        return res
    else:
        return res
    
@router_data.post("/dict_names")
async def post_dict_modeus_inf(modeus:str = None, site_inf: str = None, file_course: str = None):
    return upload_dict_names(modeus, site_inf, file_course, worker_db)

@router_data.get("/history")
async def get_history(limit: int = 1, type: TypeFile = None):
    
    try: 
        collect = worker_db.history.get_collect()
    except Exception as e:
        raise HTTPException(status_code=500, detail="Error collection")
    
    query = {}
    if type:
        query["type"] = type
    
    history = []
    i = 0
    for hist in collect.find(query).sort("date", -1):
        history.append(HistoryUploadFile(**hist))
        i += 1
        if i == limit:
            break
    
    return history