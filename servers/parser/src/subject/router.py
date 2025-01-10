from fastapi import APIRouter

from config import worker_db


router_subject = APIRouter(
    prefix="/subject",
    tags=["Subject"],
)


@router_subject.post("/add_group_tg")
async def add_group_tg(full_name: str, link: str) -> dict:
    worker_db.subject.update_one(get_item=False, update_data = {"$set" : {"group_tg_link" : link}}, full_name = full_name)
    
    return {"status" : "success"}