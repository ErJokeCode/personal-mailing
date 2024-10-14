from typing import Annotated
from fastapi import APIRouter, Body, HTTPException
from bson import ObjectId

from config import DB
from src.schemas import User


router_user_info = APIRouter(
    prefix="/user_info",
    tags=["User Info"],
)

@router_user_info.post("/")
async def add_user(user: Annotated[User, Body()]):
    try:
        collection = DB.get_user_collection()
        new_user = collection.insert_one(user.model_dump(by_alias=True, exclude=["id"]))
        created_user = collection.find_one(
            {"_id": new_user.inserted_id}
        )
        return User(**created_user), 
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    
@router_user_info.get("/{id}")
async def get_user(id: str):
    try:
        object_id = ObjectId(id)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=400, detail="Invalid ID")
    
    try:
        collection = DB.get_user_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    user = collection.find_one({"_id": ObjectId(id)})
    if user is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    return User(**user)
