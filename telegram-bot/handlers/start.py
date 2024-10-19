from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext
from aiogram.filters.command import Command

from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

import aiohttp
import re

from config import URL_SERVER
from handlers.main_menu import show_main_menu
from states import RegistrationStates, LKStates

router = Router()


def is_valid_email(email):
    pattern = r"^[\w\.-]+@[\w\.-]+\.\w+$"
    return re.match(pattern, email) is not None


# @router.message(Command("start"))
# async def cmd_start(message: types.Message, state: FSMContext):
#     await message.answer("jdjhdfjh", reply_markup=InlineKeyboardMarkup(inline_keyboard=[
#             [InlineKeyboardButton(text="Да", callback_data="yes")],
#             [InlineKeyboardButton(text="Нет", callback_data="no")],
#         ]))

# @router.message(F.data == "yes")
# async def yes(message: types.Message, state: FSMContext):
#     await message.answer("jdjhdfjh")


@router.message(Command("start"))
async def cmd_start(message: types.Message, state: FSMContext):
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
            async with session.post(f"{URL_SERVER}/core/auth", json=body) as response:
                if response.status < 400:
                    response_data = await response.json()
                    await state.update_data(user_id=response_data.get("id"))
                    await show_main_menu(message, state)
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
        async with session.post(f"{URL_SERVER}/core/auth", json=body) as response:
            if response.status < 400:
                response_data = await response.json()
                await state.update_data(user_id=response_data.get("id"))
                # Delete all registration messages
                for msg_id in [
                    user_data.get("welcome_msg_id"),
                    user_data.get("email_msg_id"),
                    user_data.get("student_id_msg_id"),
                    user_data.get("student_id_response_msg_id"),
                ]:
                    if msg_id:
                        await message.bot.delete_message(
                            chat_id=message.chat.id, message_id=msg_id
                        )
                await state.set_state(LKStates.MAIN_MENU)
                await show_main_menu(message, state)
            else:
                await message.answer(
                    "Произошла ошибка при регистрации. Пожалуйста, введите вашу почту."
                )
                await state.set_state(RegistrationStates.WAITING_FOR_EMAIL)
