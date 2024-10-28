from fastapi import APIRouter, Request
from aiogram.types import Update
from aiogram import Bot, Dispatcher

class Info_Bot():
    bot: Bot
    dp: Dispatcher

router_send = APIRouter(
    prefix="/send",
    tags=["Send"],
)

router_WH = APIRouter(
    prefix="",
    tags=["Webhook"],
)


@router_send.post("/{chat_id}")
async def send_text(chat_id: str, text : str):
    await Info_Bot.bot.send_message(chat_id="1362536052", text=text) 
    return {"":""}

@router_WH.post("/webhook")
async def webhook(request: Request) -> None:
    update = Update.model_validate(await request.json(), context={"bot": Info_Bot.bot})
    print(update, await request.json())
    await Info_Bot.dp.feed_update(Info_Bot.bot, update)
