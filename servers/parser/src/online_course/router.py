from fastapi import APIRouter

from config import worker_db
from src.schemas import DictNamesInDB, InfoOnlineCourseInDB


router_course = APIRouter(
    prefix="/course",
    tags=["Course"],
)

@router_course.get("/search")
async def get_courses(name: str, university: str | None = None) -> InfoOnlineCourseInDB:
    collection = worker_db.info_online_course.get_collect()
    query = {"name": {"$regex": name, "$options": "i"}}
    if university:
        query["university"] = {"$regex": university, "$options": "i"}
    course = collection.find_one(query)
    return InfoOnlineCourseInDB(**course)


@router_course.get("/dict_names")
async def get_modeus_to_inf(modeus:str = None, site_inf: str = None, file_course: str = None) -> DictNamesInDB:
    req = {}
    if modeus != None:
        req["modeus"] = modeus
    if site_inf != None:
        req["site_inf"] = site_inf
    if file_course != None:
        req["file_course"] = file_course
        
    return worker_db.dict_names.get_one(find_dict=req)

