from fastapi import APIRouter, Request
from aiogram.types import Update
from aiogram import Bot, Dispatcher
from fastapi import FastAPI, Request

import uvicorn
from aiogram.fsm.storage.base import StorageKey
from aiogram.fsm.context import FSMContext
from aiogram.fsm.storage.base import StorageKey
from aiogram.fsm.storage.redis import RedisStorage

from config import TOKEN

bot = Bot(token=TOKEN)

router_send = APIRouter(
    prefix="/send",
    tags=["Send"],
)

@router_send.post("/{chat_id}")
async def send_text(chat_id: str, text : str):
    await bot.send_message(chat_id="1362536052", text=text) 
    return {"status": "success"}


app = FastAPI()
app.include_router(router_send)

if __name__ == "__main__":
    uvicorn.run(app, host="localhost", port=8000)
    