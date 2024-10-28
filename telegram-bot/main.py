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

from fastapi import FastAPI, Request
import uvicorn

from config import URL_REDIS, URL_SERVER, TOKEN, NGROK_TUNNEL_URL
from handlers import start, main_menu, information_teaching
from api_bot import Info_Bot, router_send, router_WH


storage = RedisStorage.from_url(URL_REDIS)

bot = Bot(token=TOKEN)
dp = Dispatcher(storage=storage)
dp.include_routers(start.router, main_menu.router, information_teaching.router)

Info_Bot.bot = bot
Info_Bot.dp = dp

@asynccontextmanager
async def lifespan(app: FastAPI):
    await bot.set_webhook(url=f'{NGROK_TUNNEL_URL}/webhook',
                          allowed_updates=[],
                          drop_pending_updates=True)
    yield
    await bot.delete_webhook()

app = FastAPI(lifespan=lifespan)
app.include_router(router_send)
app.include_router(router_WH)

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)