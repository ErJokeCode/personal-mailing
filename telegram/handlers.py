from aiogram import types, F, Router
from aiogram.types import Message
from aiogram.filters import Command
import requests
import os
from dotenv import load_dotenv

router = Router()



load_dotenv()
SERVER_URL = os.getenv('SERVER_URL')


@router.message(Command("start"))
async def start_handler(msg: Message):
    await msg.answer("Привет! Я помогу тебе узнать твой ID, просто отправь мне любое сообщение")


@router.message()
async def message_handler(msg: Message):
    courses = requests.get("http://192.168.0.105:8000/api/courses")
    await msg.answer(courses.json()[0]["name"])