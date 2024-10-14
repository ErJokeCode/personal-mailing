from aiogram import Bot, Dispatcher, types
from aiogram.filters.command import Command
from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton
from aiogram.fsm.context import FSMContext
from aiogram.fsm.state import State, StatesGroup
from contextlib import asynccontextmanager
from aiogram.types import Update

import aiohttp
import asyncio
import json

from fastapi import FastAPI, Request
import uvicorn

from config import URL_SERVER
from config import TOKEN
from handlers import start, main_menu, information_teaching



bot = Bot(token=TOKEN)
dp = Dispatcher()
dp.include_routers(start.router, main_menu.router)

async def main():
    await dp.start_polling(bot)

if __name__ == "__main__":
    asyncio.run(main())
