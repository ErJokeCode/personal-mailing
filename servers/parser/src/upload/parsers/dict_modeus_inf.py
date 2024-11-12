from io import BytesIO
from typing import Annotated
from bson import ObjectId
from fastapi import Depends, HTTPException, UploadFile
import pandas as pd

from config import DB
from src.schemas import DictNames, Student, OnlineCourseInDB


def dict_names(modeus:str, site_inf: str, file_course: str):
    collection_names = DB.get_dict_names()
    collection_subject = DB.get_subject()
    collection_course = DB.get_course_info_collection()

    dict = DictNames(
        modeus=modeus, 
        site_inf=site_inf, 
        file_course=file_course
    )

    collection_names.insert_one(dict.model_dump(by_alias=True, exclude=["id"]))

    # collection.update_one({"modeus": modeus}, 
    #                         {"$set":{"site_inf" : site_inf, "file_course": file_course}}, 
    #                         upsert=True)

    course_db = OnlineCourseInDB(**collection_course.find_one({"name" : dict.site_inf}))

    collection_subject.update_one({"full_name" : dict.modeus}, {"$set" : {"online_course_id" : ObjectId(course_db.id)}})

    return {"status" : "success"}

