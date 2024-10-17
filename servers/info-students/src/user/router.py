from typing import Annotated
from bson import ObjectId
from fastapi import APIRouter, Body, HTTPException, UploadFile
from fastapi import Depends

from config import DB
from src.schemas import Course, GetUserAuth, User_course, User, UserAuth




router_user = APIRouter(
    prefix="/user",
    tags=["User"],
)

@router_user.post("/auth")
async def auth_user(user: Annotated[GetUserAuth, Body()]):
    try:
        collection_user = DB.get_user_collection()
        collection_auth = DB.get_user_auth_collection()
        collection_user_course = DB.get_user_course_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    
    user_course = collection_user_course.find_one({"email" : user.email})
    if user_course is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    user_course = User_course(**user_course)

    get_user = collection_user.find_one({"name" : user_course.name, "sername" : user_course.sername, "patronymic" : user_course.patronymic})
    if get_user is None:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")
    get_user = User(**get_user)

    if get_user.personal_number != user.personal_number:
        print("User not found")
        raise HTTPException(status_code=404, detail="User not found")

    new_auth_user = UserAuth(email=user.email, personal_number=user.personal_number, chat_id=user.chat_id, id_user=get_user.id, id_user_course=user_course.id)
    if collection_auth.find_one({"email" : new_auth_user.email}) == None:
        collection_auth.insert_one(new_auth_user.model_dump(by_alias=True, exclude=["id"]))
        new_auth_user = UserAuth(**collection_auth.find_one(new_auth_user.model_dump()))
        return new_auth_user
    else:
        raise HTTPException(status_code=423, detail="Can not log in a second time")
   
    
    
@router_user.post("/unauth")
async def unauth_user(user: Annotated[GetUserAuth, Body()]):
    try:
        collection_auth = DB.get_user_auth_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_auth.delete_one(user.model_dump(by_alias=True))
        return {"status" : "success"}
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="User not found")
    
@router_user.get("/{id}/courses")
async def get_courses(id : str) -> list[Course]:
    try:
        collection_auth = DB.get_user_auth_collection()
        collection_user_course = DB.get_user_course_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    try:
        user_auth = UserAuth(**collection_auth.find_one({"_id" : ObjectId(id)}))
        user_course = User_course(**collection_user_course.find_one({"_id" : ObjectId(user_auth.id_user_course)}))
        return user_course.courses
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="User not found")
    
    