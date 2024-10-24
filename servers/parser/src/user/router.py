from typing import Annotated
from bson import ObjectId
from fastapi import APIRouter, Body, HTTPException, UploadFile
from fastapi import Depends

from config import DB
from src.schemas import Course, GetUserAuth, StudentCourse, Student, UserAuth, OnlineCourseStudent




router_user = APIRouter(
    prefix="/student",
    tags=["Student"],
)
    
@router_user.get("/{id}/courses")
async def get_courses(id : str) -> list[OnlineCourseStudent]:
    try:
        collection_student = DB.get_student()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        res = collection_student.find_one({"_id" : ObjectId(id)})
        return res["online_course"]
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="User not found")
    
    
