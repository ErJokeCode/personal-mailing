import asyncio
import hashlib
from fastapi import APIRouter, HTTPException, UploadFile

from src.upload.parsers.auth import upload_student
from src.upload.parsers.excel_statement import update_report
from src.upload.parsers.parser_from_inf import parse_courses
from src.upload.parsers.choice_in_modeus import choice_in_modeus
from src.upload.parsers.dict_modeus_inf import dict_names

from config import DB, s3_client

from datetime import datetime

from src.schemas import HistoryUploadFile, TypeFile

router_data = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)

@router_data.post("/site_urfu_inf")
async def site_urfu_inf():
    return parse_courses()

@router_data.post("/student")
async def upload_info_student(file: UploadFile):
    hist_file = await create_hist_file(file, TypeFile.student)
    file_read = await s3_client.get_file_read(hist_file.key)
    
    id = upload_file_to_history(hist_file)
    
    res_upload = upload_student(file_read)
    res_upload["id"] = str(id)
    
    return res_upload

@router_data.post("/choice_in_modeus")
async def post_choice_in_modeus(file: UploadFile):
    hist_file = await create_hist_file(file, TypeFile.modeus)
    file_read = await s3_client.get_file_read(hist_file.key)
    
    id = upload_file_to_history(hist_file)
    
    res_upload = choice_in_modeus(file_read)
    res_upload["id"] = str(id)
    
    return res_upload

@router_data.post("/report_online_course")
async def post_online_course_report(file: UploadFile):
    hist_file = await create_hist_file(file, TypeFile.online_course)
    file_read = await s3_client.get_file_read(hist_file.key)
    
    id = upload_file_to_history(hist_file)
    
    res = parse_courses()
    if res["status"] == "success":
        res_upload = update_report(file_read)
        res_upload["id"] = str(id)
        
        return res_upload
    else:
        return res

@router_data.post("/dict_names")
async def post_dict_modeus_inf(modeus:str = None, site_inf: str = None, file_course: str = None):
    return dict_names(modeus, site_inf, file_course)

@router_data.get("/history")
async def get_history(limit: int = 1, type: TypeFile = None):
    try: 
        collect = DB.get_history_upload()
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


async def create_hist_file(file: UploadFile, type: TypeFile) -> HistoryUploadFile:
    time = datetime.now()
    hash = hashlib.sha256(file.filename.encode("utf-8") + str(time).encode("utf-8")).hexdigest()
    key = hash + "." + file.filename.split(".")[-1]
    link = await s3_client.upload_file(file, key)
    
    hist_file = HistoryUploadFile(
        name_file=file.filename,
        key=key,
        date=time,
        type=type,
        link=link
    )
    
    return hist_file

def upload_file_to_history(hist_file: HistoryUploadFile):
    try: 
        collect = DB.get_history_upload()
    except Exception as e:
        raise HTTPException(status_code=500, detail="Error collection")
    
    id = collect.insert_one(hist_file.model_dump(by_alias=True, exclude=["id"])).inserted_id
    
    return id