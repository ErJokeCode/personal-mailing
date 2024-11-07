from typing import List
from fastapi import APIRouter, HTTPException

from config import DB
from src.schemas import Course, Course_info, Modeus_to_inf, OnlineCourse, StudentCourse
from bson import ObjectId


router_course = APIRouter(
    prefix="/course",
    tags=["Course"],
)


# Извиняюсь за вторжение
@router_course.get("/by_email")
async def get_by_email(email: str):
    try:
        collection_user_course = DB.get_user_course_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    user_course = collection_user_course.find_one({"email": email})
    if user_course is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    user_course = StudentCourse(**user_course)

    return user_course


@router_course.get("/by_id")
async def get_by_id(id: str):
    try:
        collection_user_course = DB.get_user_course_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    user_course = collection_user_course.find_one({"_id": ObjectId(id)})
    if user_course is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    user_course = StudentCourse(**user_course)

    return user_course


@router_course.get("/search")
async def get_courses(name: str, university: str | None = None) -> OnlineCourse:
    collection = DB.get_course_info_collection()
    query = {"name": {"$regex": name, "$options": "i"}}
    if university:
        query["university"] = {"$regex": university, "$options": "i"}
    course = collection.find_one(query)
    return OnlineCourse(**course)


@router_course.get("/modeus_to_inf")
async def get_modeus_to_inf(name_modeus: str = None) -> Modeus_to_inf:
    col_mod = DB.get_modeus_inf()
    res = col_mod.find_one({"modeus" : name_modeus})
    if res == None:
        raise HTTPException(status_code=404, detail="Course not found")
    return Modeus_to_inf(**res)


