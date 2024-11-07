from aiogram import Bot, Dispatcher, types
from aiogram.filters.command import Command
from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton
from aiogram.fsm.context import FSMContext
from aiogram.fsm.state import State, StatesGroup
from contextlib import asynccontextmanager
from aiogram.types import Update
from aiogram.fsm.storage.redis import RedisStorage

import aiohttp
import asyncio
import json
import requests

from config import URL_REDIS, TOKEN_CHAT_CURATOR, URL_SERVER, get_cookie


storage = RedisStorage.from_url(URL_REDIS)

bot = Bot(token=TOKEN_CHAT_CURATOR)
dp = Dispatcher(storage=storage)

@dp.message(Command("start"))
async def cmd_start(message: types.Message, state: FSMContext):
    # await state.clear()
    user_data = await state.get_data()
    user_id = user_data.get("user_id")
    if user_id == None:
        await message.answer("Зарегестрируйтесть в боте @test123show_bot")
    else:
        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{get_cookie()}"}

            async with session.put(f"{URL_SERVER}/core/student/addChat/{user_id}", headers=headers, params={"chatId": message.chat.id}) as resp:
                if resp.status == 200:
                    await message.answer("Какой вопрос вас интенресует")
                else:
                    await message.answer("Пожалуйста нажми ещё раз /start")

@dp.message()
async def all_message(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    user_id = user_data.get("user_id")
    if user_id == None:
        await message.answer("Войдите в аккаунт в @test123show_bot")
    else:
        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{get_cookie()}"}

            async with session.get(f"{URL_SERVER}/core/admin", headers=headers) as response_admins:
                if response_admins.status == 200:
                    admins = await response_admins.json()

                    async with session.post(f"{URL_SERVER}/core/chat/student-to-admin", headers=headers, params={"content": message.text, "studentId": user_id, "adminId": admins[0]["id"]}) as resp:
                        if resp.status == 200:
                            await message.answer("Сообщение отправлено, ожидайте ответа")
                        else:
                            await message.answer("Неполучилось доставить сообщение куратору, попробуй ещё раз")
                else:
                    await message.answer("Неполучилось доставить сообщение куратору, попробуй ещё раз")
        
    

async def main():
    await dp.start_polling(bot)

if __name__ == "__main__":
    asyncio.run(main())
