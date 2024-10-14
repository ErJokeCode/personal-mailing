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

from config import NGROK_TUNNEL_URL, URL_SERVER
from config import TOKEN
from handlers import start, main_menu, information_teaching
from api_bot import Info_Bot, router


bot = Bot(token=TOKEN)
dp = Dispatcher()
dp.include_routers(start.router, main_menu.router)

Info_Bot.bot = bot
Info_Bot.dp = dp

@asynccontextmanager
async def lifespan(app: FastAPI):
    await bot.set_webhook(url=f'{NGROK_TUNNEL_URL}/bot/webhook',
                          allowed_updates=["update_id", "message_reaction", "message", "my_chat_member"],
                          drop_pending_updates=True)
    yield
    await bot.delete_webhook()


app = FastAPI(lifespan=lifespan)
app.include_router(router)

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=7000)
