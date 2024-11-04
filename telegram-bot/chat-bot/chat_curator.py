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

from config import URL_REDIS, TOKEN_CHAT_CURATOR


storage = RedisStorage.from_url(URL_REDIS)

bot = Bot(token=TOKEN_CHAT_CURATOR)
dp = Dispatcher(storage=storage)

@dp.message(Command("start"))
async def cmd_start(message: types.Message, state: FSMContext):
    # await state.clear()
    user_data = await state.get_data()
    res = user_data.get("user_id")
    if res == None:
        await message.answer("Зарегестрируйтесть в боте @test123show_bot")
    else:
        await message.answer("Какой вопрос вас интенресует")

@dp.message()
async def all_message(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    res = user_data.get("user_id")
    if res == None:
        await message.answer("Войдите в аккаунт в @test123show_bot")
    else:
        await message.bot.send_message(chat_id="1362536052", text = f"{res} : {message.text}")
    

async def main():
    await dp.start_polling(bot)

if __name__ == "__main__":
    asyncio.run(main())