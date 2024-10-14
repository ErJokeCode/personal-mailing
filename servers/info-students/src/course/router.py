from fastapi import APIRouter

from config import DB
from src.schemas import Course, Course_info


router_course = APIRouter(
    prefix="/course",
    tags=["Course"],
)

@router_course.get("/search")
async def get_courses(name : str, university : str | None = None) -> Course_info:
    collection = DB.get_course_info_collection()
    query = {"name": {"$regex": name, "$options": "i"}}
    if university:
        query["university"] = {"$regex": university, "$options": "i"}
    course = collection.find_one(query)
    return Course_info(**course)
