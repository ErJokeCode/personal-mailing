from io import BytesIO
from fastapi import HTTPException, UploadFile
import pandas as pd

from config import DB
from src.schemas import Student


def dict_modeus_inf(dict: dict):
    collection = DB.get_modeus_inf()
    for key, item in dict.items():
        collection.update_one({"modeus": key}, {"$set":{"inf" : item}}, upsert=True)

    return {"status" : "success"}

