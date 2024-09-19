from aiogram import types, F, Router
from aiogram.types import CallbackQuery
from aiogram.types import Message
from aiogram.filters import Command
import kb
import requests
import os
from dotenv import load_dotenv

router = Router()

load_dotenv()
SERVER_URL = os.getenv('SERVER_URL')
print(SERVER_URL)

@router.message(Command("start"))
async def start_handler(msg: Message):
    await msg.answer("Привет!", reply_markup=kb.welcome_kb())

@router.message(F.text == 'Информация об онлайн курсе')
async def message_get_course(msg: Message):
    req = requests.get(SERVER_URL + "/courses/universitys")
    universitys = req.json()
    await msg.answer("Онлайн курсы от:", reply_markup=kb.university_kb(universitys))

@router.callback_query()
async def message_handler(callback: CallbackQuery):
    if(callback.data.split()[0] == "course"):
        id = int(callback.data.split()[2])
        university = callback.data.split()[1]
        req = requests.get(SERVER_URL + "/courses", json={"university" : university})
        courses = req.json()
        ans = ""
        course = courses[id]
        ans = f'Название курса: {course["name"]}\n\nУниверситет: {course["university"]}\n\nВажные даты: {course["date"]}\n\nОсновная информация: {course["info"]}'
        await callback.message.delete()
        await callback.bot.send_message(chat_id=callback.from_user.id, text=ans)
    else:
        req = requests.get(SERVER_URL + "/courses", json={"university" : callback.data})
        courses = req.json()
        await callback.message.delete()
        await callback.bot.send_message(chat_id=callback.from_user.id, text="Курсы от " + callback.data, reply_markup=kb.courses_kb(courses=courses, university=callback.data))
    


