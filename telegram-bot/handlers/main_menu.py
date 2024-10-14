from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext

import aiohttp

from config import URL_SERVER
import keyboards.main_menu as keyboard
from states import LKStates
from texts.error import Registration, Input
from texts.main_menu import ToCurator

router = Router()


async def show_main_menu(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        await message.answer("Главное меню:", reply_markup=keyboard.Menu())
    else:
        await message.delete()
        await message.answer(Registration.no())



@router.callback_query(LKStates.MAIN_MENU, F.data == 'chat_curator')
async def process_chat_curator(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()

    if user_data.get('email') and user_data.get('personal_number'):
        await callback_query.message.delete()
        await state.set_state(LKStates.WAITING_CHAT_WITH_CURATOR)
        await callback_query.message.answer(ToCurator.preface(), 
                                            reply_markup=keyboard.Back_to_main())
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())

@router.message(LKStates.WAITING_CHAT_WITH_CURATOR)
async def message_to_curator(message: types.Message, state: FSMContext):
    print(message.text)

# @router.callback_query(LKStates.MAIN_MENU, lambda c: c.data == "chat_curator")
# async def process_chat_curator(callback_query: types.CallbackQuery, state: FSMContext):
#     user_data = await state.get_data()
#     if user_data.get('email') and user_data.get('personal_number'):
#         await callback_query.message.delete()
#         await state.set_state(LKStates.WAITING_CHAT_WITH_CURATOR)
#         await callback_query.message.answer("Введите ваш вопрос:", reply_markup=keyboard.Back_to_main())
#     else:
#         await callback_query.message.delete()
#         await callback_query.message.answer("Вы не зарегистрированы. Пожалуйста, введите вашу почту и номер студенческого билета. Для повторного входа используйте команду /start")

@router.callback_query(lambda c: c.data == "online_courses")
async def process_online_courses(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    # Отправляем запрос на сервер для получения списка онлайн курсов
    async with aiohttp.ClientSession() as session:
        async with session.get(f"{URL_SERVER}/user/{user_data.get('user_id')}/courses") as response:
            if response.status == 200:
                courses_data = await response.json()
            else:
                courses_data = []
    await state.update_data(courses_data=courses_data)

    if user_data.get('email') and user_data.get('personal_number'):
        await callback_query.message.delete()
        await callback_query.message.answer("Выберите онлайн курс:", reply_markup=keyboard.Courses(courses_data=courses_data))
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())

@router.callback_query(lambda c: c.data.startswith("course_"))
async def process_course_info(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        courses_data = user_data.get('courses_data')
        course_id = int(callback_query.data.split("_")[1])
        async with aiohttp.ClientSession() as session:
            async with session.get(f"{URL_SERVER}/course/search", params={"name": courses_data[course_id]["name"], "university": courses_data[course_id]["university"]}) as response:
                if response.status == 200:
                    course_data = await response.json()
                else:
                    course_data = {}

        await callback_query.message.delete()
        await callback_query.message.answer(f"Информация о курсе {course_data['name']}\n{course_data['info']}\n{course_data['date']}\n{course_data['university']}\n\n Баллы: {courses_data[course_id]['score']}", reply_markup=keyboard.Back_to_courses())
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())

@router.callback_query(lambda c: c.data == "faq")
async def process_faq(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        faq_text = "Часто задаваемые вопросы:\n1. Вопрос 1\n2. Вопрос 2\n3. Вопрос 3"
        await callback_query.message.delete()
        await callback_query.message.answer(faq_text, reply_markup=keyboard.Back_to_main())
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())

@router.callback_query(lambda c: c.data == "main_menu")
async def process_main_menu(callback_query: types.CallbackQuery, state: FSMContext): 
    await state.set_state(LKStates.MAIN_MENU)   
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        await callback_query.message.delete()
        await show_main_menu(callback_query.message, state)
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())

# @router.callback_query(lambda c: c.data == "logout")
# async def process_logout(callback_query: types.CallbackQuery, state: FSMContext):
#     user_data = await state.get_data()
#     body = {
#             "email": user_data.get('email'),
#             "personal_number": user_data.get('personal_number'),
#             "chat_id": str(callback_query.message.chat.id)
#         }
#     async with aiohttp.ClientSession() as session:
#         async with session.post(f"{URL_SERVER}/user/unauth", json=body) as response:
#             await state.clear()
#             await callback_query.message.delete()
#             await callback_query.message.answer("Вы вышли из аккаунта. Для повторного входа используйте команду /start")



@router.message(LKStates.MAIN_MENU)
async def chat(message: types.Message, state: FSMContext):
    await message.answer(Input.incorrect())