from typing import Annotated
from bson import ObjectId
from fastapi import APIRouter, Body, HTTPException, UploadFile
from fastapi import Depends
from bson import json_util
import bson
import json
import datetime

from config import DB
from src.schemas import (
    Course,
    GetUserAuth,
    StudentCourse,
    Student,
    UserAuth,
    OnlineCourseStudent,
)


router_user = APIRouter(
    prefix="/student",
    tags=["Student"],
)


@router_user.get("/{id}/courses")
async def get_courses(id: str) -> list[OnlineCourseStudent]:
    try:
        collection_student = DB.get_student()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        res = collection_student.find_one({"_id": ObjectId(id)})
        return res["online_course"]
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="User not found")


def my_handler(x):
    if isinstance(x, datetime.datetime):
        return x.isoformat()
    elif isinstance(x, bson.ObjectId):
        return str(x)
    else:
        raise TypeError(x)


@router_user.get("/")
async def get_student(email: str):
    try:
        collection = DB.get_student()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    user = collection.find_one({"email": email})

    if user is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")

    return json.loads(json.dumps(user, default=my_handler))


@router_user.get("/all")
async def get_student():
    try:
        collection = DB.get_student()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    users = collection.find({}).to_list()

    return json.loads(json.dumps(users, default=my_handler))
