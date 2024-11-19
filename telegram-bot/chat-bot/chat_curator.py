from aiogram import F, Bot, Dispatcher, types
from aiogram.filters.command import Command
from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton
from aiogram.fsm.context import FSMContext
from aiogram.fsm.state import State, StatesGroup
from contextlib import asynccontextmanager
from aiogram.types import Update
from aiogram.types import ChatMemberUpdated
from aiogram.filters import IS_MEMBER, IS_NOT_MEMBER, ChatMemberUpdatedFilter
from aiogram.fsm.storage.redis import RedisStorage

import aiohttp
import asyncio
import json
import requests

from config import URL_REDIS, TOKEN_CHAT_CURATOR, URL_SERVER, get_cookie


storage = RedisStorage.from_url(URL_REDIS)

bot = Bot(token=TOKEN_CHAT_CURATOR)
dp = Dispatcher(storage=storage)



@dp.message(Command("add_group"), F.chat.type.in_({"group", "supergroup"}) & F.from_user.id == 1362536052)
async def all_message_in_group(message: types.Message, state: FSMContext):
    full_name_course = " ".join(message.text.split()[1:])
    link = await message.bot.export_chat_invite_link(message.chat.id)

    async with aiohttp.ClientSession() as session:
        headers = {"cookie": f"{get_cookie()}"}
        async with session.post(
            f"{URL_SERVER}/parser/subject/add_group_tg", 
            headers=headers, 
            params={"full_name": full_name_course, "link": link}
            ) as resp:
                if resp.status == 200:
                    await message.answer(f"Группа успешно добавлена")

@dp.chat_member(ChatMemberUpdatedFilter(IS_NOT_MEMBER >> IS_MEMBER))
async def all_message_in_group(event: ChatMemberUpdated, state: FSMContext):
    await event.answer("Отправить данные на сервер")




@dp.message(Command("start"), F.chat.type.in_({"private"}))
async def cmd_start(message: types.Message, state: FSMContext):
    # await state.clear()
    user_data = await state.get_data()
    user_id = user_data.get("user_id")
    if user_id == None:
        await message.answer("Зарегестрируйтесть в боте @test123show_bot")
    else:
        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{get_cookie()}"}

            async with session.put(f"{URL_SERVER}/core/student/addChat/{user_id}", headers=headers, json={"chatId": message.chat.id}) as resp:
                if resp.status == 200:
                    await message.answer("Какой вопрос вас интенресует")
                else:
                    await message.answer("Пожалуйста нажми ещё раз /start")

@dp.message(F.chat.type.in_({"private"}))
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

                    with aiohttp.MultipartWriter('form-data') as mpwriter:
                        part = mpwriter.append_json({"content": message.text, "studentId": user_id, "adminId": admins[0]["id"]})
                        part.set_content_disposition('form-data', name="body")

                        async with session.post(f"{URL_SERVER}/core/chat/studentSend", headers=headers, data=mpwriter) as resp:
                            if resp.status == 200:
                                msg = await message.answer("Сообщение отправлено, ожидайте ответа")
                                await asyncio.sleep(5)
                                await msg.delete()
                            else:
                                msg = await message.answer("Неполучилось доставить сообщение куратору, попробуй ещё раз")
                                await asyncio.sleep(5)
                                await msg.delete()

                else:
                    msg = await message.answer("Неполучилось доставить сообщение куратору, попробуй ещё раз")
                    await asyncio.sleep(5)
                    await msg.delete()
    

async def main():
    await dp.start_polling(bot)

if __name__ == "__main__":
    asyncio.run(main())
