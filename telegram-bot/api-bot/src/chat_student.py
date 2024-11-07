from aiogram.types.input_file import FSInputFile
from aiogram.utils.media_group import MediaGroupBuilder

import os
import shutil
from fastapi import APIRouter, UploadFile
from aiogram import Bot, Dispatcher
from aiogram.fsm.storage.redis import RedisStorage

from config import TOKEN_CHAT_CURATOR, URL_REDIS

bot_chat_curator = Bot(token=TOKEN_CHAT_CURATOR)
storage = RedisStorage.from_url(URL_REDIS)

router_chat_student = APIRouter(
    prefix="/chat_student",
    tags=["Chat with student"],
)


@router_chat_student.post("/{chat_id}/text")
async def chat_student_text(chat_id: str, text : str):
    await bot_chat_curator.send_message(chat_id=chat_id, text=text)
    return {"status": "success"}


@router_chat_student.post("/{chat_id}/files")
async def chat_student_files(chat_id: str, text : str, files: list[UploadFile]):
    media_group = MediaGroupBuilder(caption=text)
    paths = []
    
    for file in files:
        name = file.filename
        path = 'static/' + name
        paths.append(path)
        with open(path, "wb") as wf:
            shutil.copyfileobj(file.file, wf)
        doc = FSInputFile(path)
        media_group.add_document(doc)

    build_media = media_group.build()

    await bot_chat_curator.send_media_group(chat_id=chat_id, media=build_media) 

    for path in paths:
        os.remove(path)
    return {"status": "success"}

@router_chat_student.post("/{chat_id}/photos")
async def chat_student_files(chat_id: str, text : str, files: list[UploadFile]):
    media_group = MediaGroupBuilder(caption=text)
    paths = []
    
    for file in files:
        name = file.filename
        path = 'static/' + name
        paths.append(path)
        with open(path, "wb") as wf:
            shutil.copyfileobj(file.file, wf)
        doc = FSInputFile(path)
        media_group.add_photo(doc)

    build_media = media_group.build()

    await bot_chat_curator.send_media_group(chat_id=chat_id, media=build_media) 

    for path in paths:
        os.remove(path)
    return {"status": "success"}
