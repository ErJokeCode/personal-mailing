from datetime import datetime
import asyncio 
from fastapi import APIRouter, BackgroundTasks, HTTPException, UploadFile
from fastapi.responses import FileResponse
from config import s3_client, worker_db
from src.upload.online_course import parse_info_online_courses, upload_report
from src.schemas import HistoryUploadFile, HistoryUploadFileInDB, TypeFile

from src.upload.student import upload_student
from src.upload.modeus import upload_modeus


router_data = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)

@router_data.post("/student")
async def upload_data_student(file: UploadFile) -> dict[str, str]:
    hist = await get_history()
    if hist and hist[0].status_upload is None:
        raise HTTPException(status_code=400, detail=f"Wait for the {hist[0].name_file} file to be processed")
    
    hist_info = await s3_client.create_hist_file(file, TypeFile.student, is_upload=False)
    hist_info_db = worker_db.history.insert_one(hist_info)
    
    asyncio.create_task(background_student(file.file.read(), file.filename, file.size, file.headers, hist_info_db))
    
    return {"status": "success"}

@router_data.get("/student/example")
async def get_example_file_student():
    return FileResponse(
        path="src/upload/example/example_students.xls", 
        filename="Студенты.xls", 
        media_type="multipart/form-data")
    

@router_data.post("/choice_in_modeus")
async def post_choice_in_modeus(file: UploadFile) -> dict[str, str]:
    hist = await get_history(type=TypeFile.student)
    if len(hist) == 0:
        raise HTTPException(status_code=400, detail=f"First upload the file with students")
    
    hist = await get_history()
    if len(hist) == 1 and hist[0].status_upload is None:
        raise HTTPException(status_code=400, detail=f"Wait for the {hist[0].name_file} file to be processed")
    
    
    hist_info = await s3_client.create_hist_file(file, TypeFile.modeus, is_upload=False)
    hist_info_db = worker_db.history.insert_one(hist_info)
    
    asyncio.create_task(background_modeus(file.file.read(), file.filename, file.size, file.headers, hist_info_db))
   
    return {"status": "success"}

@router_data.get("/choice_in_modeus/example")
async def get_example_file_modeus():
    return FileResponse(
        path="src/upload/example/example_modeus.xlsx", 
        filename="Модеус.xlsx", 
        media_type="multipart/form-data")

@router_data.post("/report_online_course")
async def post_online_course_report(file: UploadFile) -> dict[str, str]:
    hist = await get_history(type=TypeFile.student)
    if len(hist) == 0:
        raise HTTPException(status_code=400, detail=f"First upload the file with students")
    
    hist = await get_history(type=TypeFile.modeus)
    if len(hist) == 0:
        raise HTTPException(status_code=400, detail=f"First upload the file with modeus")
    
    hist = await get_history()
    if len(hist) == 1 and hist[0].status_upload is None:
        raise HTTPException(status_code=400, detail=f"Wait for the {hist[0].name_file} file to be processed")
    
    hist_info = await s3_client.create_hist_file(file, TypeFile.online_course, is_upload=False)
    hist_info_db = worker_db.history.insert_one(hist_info)
    
    asyncio.create_task(background_online_course(file.file.read(), file.filename, file.size, file.headers, hist_info_db))
   
    return {"status": "success"}

@router_data.get("/report_online_course/example")
async def get_example_file_online_course():
    return FileResponse(
        path="src/upload/example/example_online_course.xlsx", 
        filename="ОнлайнКурсы.xlsx", 
        media_type="multipart/form-data")

@router_data.get("/history")
async def get_history(limit: int = 1, type: TypeFile = None) -> list[HistoryUploadFileInDB]:
    
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
        history.append(HistoryUploadFileInDB(**hist))
        i += 1
        if i == limit:
            break
    
    return history



async def background_student(file, filename, size, headers, hist_info_db: HistoryUploadFileInDB):
    try:
        file_upload = UploadFile(file=file, filename=filename, size=size, headers=headers)
        
        link = await s3_client.upload_file(file_upload, hist_info_db.key)
        
        hist_info_db.link = link
        worker_db.history.update_one(hist_info_db, get_item=False)
        
        upload_student(hist_info_db.link, worker_db)
        
        hist_info_db.status_upload = "success"
        worker_db.history.update_one(hist_info_db, get_item=False)
    except Exception as e:
        print(e)
        hist_info_db.status_upload = "error"
        worker_db.history.update_one(hist_info_db, get_item=False)

async def background_modeus(file, filename, size, headers, hist_info_db: HistoryUploadFileInDB):
    try:
        file_upload = UploadFile(file=file, filename=filename, size=size, headers=headers)
        
        link = await s3_client.upload_file(file_upload, hist_info_db.key)
        
        hist_info_db.link = link
        worker_db.history.update_one(hist_info_db, get_item=False)
            
        upload_modeus(hist_info_db.link, worker_db)
        
        hist_info_db.status_upload = "success"
        worker_db.history.update_one(hist_info_db, get_item=False)
    except Exception as e:
        print(e)
        hist_info_db.status_upload = "error"
        worker_db.history.update_one(hist_info_db, get_item=False)
        
async def background_online_course(file, filename, size, headers, hist_info_db: HistoryUploadFileInDB):
    try:
        file_upload = UploadFile(file=file, filename=filename, size=size, headers=headers)
        
        link = await s3_client.upload_file(file_upload, hist_info_db.key)
        
        hist_info_db.link = link
        worker_db.history.update_one(hist_info_db, get_item=False)
            
        parse_info_online_courses(worker_db)
        upload_report(hist_info_db.link, worker_db)

        hist_info_db.status_upload = "success"
        worker_db.history.update_one(hist_info_db, get_item=False)
    except Exception as e:
        print(e)
        hist_info_db.status_upload = "error"
        worker_db.history.update_one(hist_info_db, get_item=False)