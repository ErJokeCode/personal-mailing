from aiogram.types.input_file import FSInputFile
from aiogram.utils.media_group import MediaGroupBuilder

import os
import shutil
from fastapi import APIRouter, UploadFile
from aiogram import Bot, Dispatcher
from aiogram.fsm.storage.redis import RedisStorage

from config import TOKEN, URL_REDIS


bot = Bot(token=TOKEN)
storage = RedisStorage.from_url(URL_REDIS)

router_send = APIRouter(
    prefix="/send",
    tags=["Send"],
)

@router_send.post("/files")
async def send_text_with_file(chat_ids: list[str], text: str, files: list[UploadFile]):
    media_group = MediaGroupBuilder(caption=text)
    paths = []
    chat_ids = chat_ids
    
    for file in files:
        name = file.filename
        path = 'static/' + name
        paths.append(path)
        with open(path, "wb") as wf:
            shutil.copyfileobj(file.file, wf)
        doc = FSInputFile(path)
        media_group.add_document(doc)

    build_media = media_group.build()
    for id in chat_ids:
        await bot.send_media_group(chat_id=id, media=build_media) 

    for path in paths:
        os.remove(path)
    return {"status": "success"}

@router_send.post("/photos")
async def send_text_with_photo(chat_ids: list[str], text: str, files: list[UploadFile]):
    media_group = MediaGroupBuilder(caption=text)
    paths = []
    chat_ids = chat_ids
    
    for file in files:
        name = file.filename
        path = 'static/' + name
        paths.append(path)
        with open(path, "wb") as wf:
            shutil.copyfileobj(file.file, wf)
        doc = FSInputFile(path)
        media_group.add_photo(doc)

    build_media = media_group.build()
    for id in chat_ids:
        await bot.send_media_group(chat_id=id, media=build_media) 

    for path in paths:
        os.remove(path)
    return {"status": "success"}

@router_send.post("/{chat_id}")
async def send_text(chat_id: str, text : str):
    await bot.send_message(chat_id=chat_id, text=text)
    # res = await bot.send_message(chat_id=chat_id, text=text, reply_markup=InlineKeyboardMarkup(inline_keyboard=[
    #         [InlineKeyboardButton(text="В архив", callback_data="to_archive")], 
    #         [InlineKeyboardButton(text="В избранное", callback_data="to_favourite")]
    #     ])) 
    return {"status": "success"}
