from typing import Annotated
from fastapi import APIRouter, Body, HTTPException
from bson import ObjectId

from config import DB
from src.schemas import Student


router_user_info = APIRouter(
    prefix="/user_info",
    tags=["User Info"],
)


@router_user_info.post("/")
async def add_user(user: Annotated[Student, Body()]):
    try:
        collection = DB.get_student()
        new_user = collection.insert_one(user.model_dump(by_alias=True, exclude=["id"]))
        created_user = collection.find_one({"_id": new_user.inserted_id})
        return Student(**created_user)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")


@router_user_info.get("/by_name")
async def get_user_by_name(name: str, sername: str, patronymic: str):
    try:
        collection_user = DB.get_student()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    get_user = collection_user.find_one(
        {
            "name": name,
            "sername": sername,
            "patronymic": patronymic,
        }
    )
    if get_user is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    get_user = Student(**get_user)

    return get_user


@router_user_info.get("/{id}")
async def get_user(id: str):
    try:
        object_id = ObjectId(id)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=400, detail="Invalid ID")

    try:
        collection = DB.get_student()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    user = collection.find_one({"_id": ObjectId(id)})
    if user is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    return Student(**user)
