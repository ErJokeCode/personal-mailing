from bson import ObjectId
from fastapi import APIRouter, HTTPException

from config import DB
from src.schemas import OnboardCourse, OnboardCourseInDB, OnboardSection, OnboardTopic



router_bot_onboard = APIRouter(
    prefix="/bot/onboard",
    tags=["Bot onboard"],
)



@router_bot_onboard.post("/one_course")
async def add_course(course: OnboardCourse) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        id = collection_bot.insert_one(course.model_dump()).inserted_id
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error add")
    
    
@router_bot_onboard.post("/{id}/section")
async def add_section(id: str, section: OnboardSection) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.update_one({"_id" : ObjectId(id)}, 
                                  {"$push" : {"sections" : section.model_dump()}})
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error add")
    

@router_bot_onboard.post("/{id}/{section_name}/topic")
async def add_topic(id: str, section_name: str, topic: OnboardTopic) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.update_one({"_id" : ObjectId(id)}, 
                                  {"$push" : {"sections.$[i].topics" : topic.model_dump()}}, 
                                  array_filters=[{"i.name": section_name}])
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error add")
    

@router_bot_onboard.put("/{id}")
async def put_course(id: str, course: OnboardCourse) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.update_one({"_id" : ObjectId(id)}, 
                                  course.model_dump())
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error update")
    
    
@router_bot_onboard.put("/{id}/{section_name}")
async def put_section(id: str, section_name: str, section: OnboardSection) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.update_one({"_id" : ObjectId(id)}, 
                                  {"$set" : {"sections.$[i]" : section.model_dump()}}, 
                                  array_filters=[{"i.name": section_name}])
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error update")
    

@router_bot_onboard.put("/{id}/{section_name}/{topic_name}")
async def put_topic(id: str, section_name: str, topic_name: str, topic: OnboardTopic) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.update_one({"_id" : ObjectId(id)}, 
                                  {"$set" : {"sections.$[i].topics.$[j]" : topic.model_dump()}}, 
                                  array_filters = [{"i.name": section_name}, {"j.name": topic_name}])
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error update")
    

@router_bot_onboard.delete("/{id}")
async def delete_course(id: str) -> dict:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.delete_one({"_id" : ObjectId(id)})
        return {"status" : "success"}
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error delete")    

    
@router_bot_onboard.delete("/{id}/{section_name}")
async def delete_section(id: str, section_name: str) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.update_one({"_id" : ObjectId(id)}, 
                                  {"$pull" : {"sections" : {"name" : section_name}}})
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error delete")
    

@router_bot_onboard.delete("/{id}/{section_name}/{topic_name}")
async def delete_topic(id: str, section_name: str, topic_name: str) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_bot.update_one({"_id" : ObjectId(id)}, 
                                  {"$pull" : {"sections.$[i].topics" : {"name" : topic_name}}}, 
                                  array_filters = [{"i.name": section_name}])
                                  
        return OnboardCourseInDB(**collection_bot.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error delete")
    
    
@router_bot_onboard.get("/{id}")
async def get_course(id: str) -> OnboardCourseInDB:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        course = collection_bot.find_one({"_id" : ObjectId(id)})
        return OnboardCourseInDB(**course)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error get")
    
    
@router_bot_onboard.get("/")
async def get_courses() -> list[OnboardCourseInDB]:
    try:
        collection_bot = DB.get_bot_onboard()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        courses = []
        for course in collection_bot.find({}):
            courses.append(OnboardCourseInDB(**course))
        return courses
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error get")