from io import BytesIO
from typing import Annotated
from fastapi import Depends, HTTPException, UploadFile
import pandas as pd

from config import DB
from src.schemas import DictNames, Student


def dict_names(modeus:str, site_inf: str, file_course: str):
    collection = DB.get_dict_names()

    dict = DictNames(
        modeus=modeus, 
        site_inf=site_inf, 
        file_course=file_course
    )

    collection.insert_one(dict.model_dump(by_alias=True, exclude=["id"]))

    # collection.update_one({"modeus": modeus}, 
    #                         {"$set":{"site_inf" : site_inf, "file_course": file_course}}, 
    #                         upsert=True)

    return {"status" : "success"}

