from bson import ObjectId
from fastapi import APIRouter, HTTPException

from config import worker_db
from src.schemas import FAQ, FAQInDB, FAQTopic



router_bot_faq = APIRouter(
    prefix="/bot/faq",
    tags=["Bot FAQ"],
)

@router_bot_faq.post("/")
async def add_faq(faq: FAQ) -> FAQInDB:
    return worker_db.bot_faq.insert_one(faq)
    
@router_bot_faq.get("/")
async def get_faqs(callback_data: str = None) -> list[FAQInDB]:
    try:
        collection_faq = worker_db.bot_faq.get_collect()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB. No connection to DB")
    
    try:
        faqs = []
        for faq in collection_faq.find({'callback_data': callback_data} if callback_data else {}):
            faqs.append(FAQInDB(**faq))
        return faqs
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error get")
    
    
@router_bot_faq.get("/topic")
async def get_topics() -> list[FAQTopic]:
    try:
        collection_faq = worker_db.bot_faq.get_collect()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB. No connection to DB")
    
    try:
        faqs = []
        topics = []
        for topic in collection_faq.distinct("callback_data"):
            topics.append(topic)
            
        for topic in topics:
            faq = collection_faq.find_one({"callback_data": topic})
            faqs.append(FAQTopic(**faq))
        return faqs
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error get")
    
    
@router_bot_faq.get("/{id}")
async def get_faq(id: str) -> FAQInDB:
    return worker_db.bot_faq.get_one(id=id)
    
@router_bot_faq.put("/{id}")
async def put_faq(id: str, faq: FAQ) -> FAQInDB:
    try:
        collection_faq = worker_db.bot_faq.get_collect()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB. No connection to DB")
    
    try:
        collection_faq.update_one({"_id" : ObjectId(id)}, 
                                  faq.model_dump())
        return FAQInDB(**collection_faq.find_one({"_id" : ObjectId(id)}))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="Error update") 
    
@router_bot_faq.delete("/{id}")
async def delete_faq(id: str) -> dict:
    return worker_db.bot_faq.delete_one(id)