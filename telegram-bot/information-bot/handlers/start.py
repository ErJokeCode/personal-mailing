import asyncio
from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext
from aiogram.filters.command import Command
import requests

from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

import aiohttp
import re

from config import URL_SERVER, get_cookie
from handlers.main_menu import show_main_menu
from handlers.onboarding import choice_onboarding
from states import RegistrationStates, LKStates

router = Router()


def is_valid_email(email):
    pattern = r"^[\w\.-]+@[\w\.-]+\.\w+$"
    return re.match(pattern, email) is not None


@router.message(Command("start"))
async def cmd_start(message: types.Message, state: FSMContext):
    # await state.clear()
    user_data = await state.get_data()
    
    if not user_data.get("email") and not user_data.get("personal_number"):
        welcome_msg = await message.answer(
            "Добро пожаловать! Пожалуйста, введите вашу почту."
        )
        await state.set_state(RegistrationStates.WAITING_FOR_EMAIL)
        await state.update_data(welcome_msg_id=welcome_msg.message_id)
    else:
        user_data = await state.get_data()
        body = {
            "email": user_data.get("email"),
            "personal_number": user_data.get("personal_number"),
            "chat_id": str(message.chat.id),
        }
        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{get_cookie()}"}
            async with session.post(
                f"{URL_SERVER}/core/student/auth", json=body, headers=headers
            ) as response:
                if response.status < 400:
                    response_data = await response.json()
                    await state.update_data(user_id=response_data.get("id"))
                    await choice_onboarding(message, state)
                elif response.status == 423:
                    msg_login = await message.answer("Пользователь уже вошел в аккаунт")
                    await asyncio.sleep(5)
                    await message.bot.delete_messages(
                        message.chat.id, [msg_login.message_id, message.message_id]
                    )
                else:
                    await message.answer(
                        "Произошла ошибка при регистрации. Пожалуйста, введите вашу почту."
                    )
                    await state.set_state(RegistrationStates.WAITING_FOR_EMAIL)
                    await state.update_data(welcome_msg_id=welcome_msg.message_id)


@router.message(RegistrationStates.WAITING_FOR_EMAIL)
async def process_email(message: types.Message, state: FSMContext):
    if is_valid_email(message.text):
        await state.update_data(email=message.text, email_msg_id=message.message_id)
        student_id_msg = await message.answer(
            "Спасибо! Теперь введите номер вашего студенческого билета."
        )
        await state.update_data(student_id_msg_id=student_id_msg.message_id)
        await state.set_state(RegistrationStates.WAITING_FOR_STUDENT_ID)
    else:
        await message.answer("Неверный формат почты. Пожалуйста, попробуйте еще раз.")


@router.message(RegistrationStates.WAITING_FOR_STUDENT_ID)
async def process_student_id(message: types.Message, state: FSMContext):
    await state.update_data(
        personal_number=message.text, student_id_response_msg_id=message.message_id
    )
    user_data = await state.get_data()
    body = {
        "email": user_data.get("email"),
        "personal_number": user_data.get("personal_number"),
        "chat_id": str(message.chat.id),
    }
    async with aiohttp.ClientSession() as session:
        headers = {"cookie": f"{get_cookie()}"}
        async with session.post(
            f"{URL_SERVER}/core/student/auth", json=body, headers=headers
        ) as response:
            if response.status < 400:
                response_data = await response.json()
                # Delete all registration messages
                for msg in [
                    "welcome_msg_id",
                    "email_msg_id",
                    "student_id_msg_id",
                    "student_id_response_msg_id",
                ]:
                    msg_id = user_data.get(msg)
                    if msg_id:
                        await message.bot.delete_message(
                            chat_id=message.chat.id, message_id=msg_id
                        )
                await state.set_state(LKStates.MAIN_MENU)
                await state.clear()
                await state.update_data(
                    email=body["email"],
                    personal_number=body["personal_number"],
                    user_id=response_data.get("id"),
                )
                await choice_onboarding(message, state)
            else:
                await message.answer(
                    "Произошла ошибка при регистрации. Пожалуйста, введите вашу почту."
                )
                await state.set_state(RegistrationStates.WAITING_FOR_EMAIL)
