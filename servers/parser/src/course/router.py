from fastapi import APIRouter, HTTPException

from config import DB
from src.schemas import Course, Course_info, User_course


router_course = APIRouter(
    prefix="/course",
    tags=["Course"],
)


@router_course.get("/by_email")
async def auth_user(email: str):
    try:
        collection_user_course = DB.get_user_course_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    user_course = collection_user_course.find_one({"email": email})
    if user_course is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    user_course = User_course(**user_course)

    return user_course


@router_course.get("/search")
async def get_courses(name: str, university: str | None = None) -> Course_info:
    collection = DB.get_course_info_collection()
    query = {"name": {"$regex": name, "$options": "i"}}
    if university:
        query["university"] = {"$regex": university, "$options": "i"}
    course = collection.find_one(query)
    return Course_info(**course)
