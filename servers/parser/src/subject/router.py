from typing import Annotated, List
from bson import ObjectId
from fastapi import APIRouter, Body, HTTPException, UploadFile
from fastapi import Depends
from bson import json_util
import bson
import json
import datetime
from pymongo.collection import Collection

from config import DB
from src.schemas import SubjectInBD



router_subject = APIRouter(
    prefix="/subject",
    tags=["Subject"],
)


@router_subject.post("/add_group_tg")
async def add_group_tg(full_name: str, link: str) -> dict:
    try:
        collection_subject = DB.get_subject()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        collection_subject.update_one({"full_name": full_name}, {"$set" : {"group_tg_link" : link}})
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error update")
    return {"status" : "success"}
