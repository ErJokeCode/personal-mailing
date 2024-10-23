from typing import Annotated
from fastapi import APIRouter, Body, HTTPException
import requests

from src.schemas import Student
from config import TOKEN_BOT


router_bot = APIRouter(
    prefix="/bot",
    tags=["Bot"],
)

@router_bot.post("/{id}")
async def add_user(id: str, text: str):
    response = requests.post(url = f"https://api.telegram.org/bot{TOKEN_BOT}/sendMessage",
                             data={'chat_id': id, 'text': text})
    return response.json()

@router_bot.get("/getChat/{id}")
async def get_update(id: str):
    response = requests.post(url = f"https://api.telegram.org/bot{TOKEN_BOT}/getChat",
                            data={'chat_id': id})
    return response.json()
