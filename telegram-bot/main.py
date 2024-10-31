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

from config import URL_REDIS, URL_SERVER, TOKEN, NGROK_TUNNEL_URL
from handlers import start, main_menu, information_teaching


storage = RedisStorage.from_url(URL_REDIS)

bot = Bot(token=TOKEN)
dp = Dispatcher(storage=storage)
dp.include_routers(start.router, main_menu.router, information_teaching.router)

async def main():
    await dp.start_polling(bot)

if __name__ == "__main__":
    asyncio.run(main())