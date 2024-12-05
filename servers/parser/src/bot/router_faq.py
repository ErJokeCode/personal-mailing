from bson import ObjectId
from fastapi import APIRouter, HTTPException

from config import DB
from src.schemas import OnboardCourse, OnboardCourseInDB, OnboardSection, OnboardTopic



router_bot_faq = APIRouter(
    prefix="/bot/faq",
    tags=["Bot FAQ"],
)