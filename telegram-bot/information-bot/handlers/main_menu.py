import json
from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext

import aiohttp

from config import URL_SERVER, get_cookie
import keyboards.main_menu as keyboard
from states import Info_teaching, LKStates
from texts.error import Registration, Input
from texts.main_menu import ToCurator
from handlers.information_teaching import show_info_teaching

router = Router()


async def show_main_menu(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        await message.answer("Главное меню\n\n👋 Привет! Я ваш помощник. Вы можете получить доступ к следующей информации:\n\n📚 Основная информация о предметах\nУзнайте расписание, материалы и темы лекций по вашим предметам.\n\n💻 Онлайн-курсы\nПросмотрите доступные онлайн-курсы, их описание и расписание. \n\n🔔 Уведомления\nПолучите последние уведомления о важных событиях, изменениях в расписании и других новостях.", reply_markup=keyboard.Menu())
    else:
        await message.delete()
        await message.answer(Registration.no())



@router.callback_query(LKStates.MAIN_MENU, F.data == 'chat_curator')
async def process_chat_curator(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()

    if user_data.get('email') and user_data.get('personal_number'):
        await callback_query.message.delete()
        await state.set_state(LKStates.WAITING_CHAT_WITH_CURATOR)
        msg = await callback_query.message.answer(ToCurator.preface(), 
                                            reply_markup=keyboard.Back_to_main())
        await state.update_data(msg_in_chat_with_curator = [msg.message_id])
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())



@router.message(LKStates.WAITING_CHAT_WITH_CURATOR)
async def message_to_curator(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    msg_in_chat_with_curator = user_data.get("msg_in_chat_with_curator")
    msg_in_chat_with_curator.append(message.message_id)
    await state.update_data(msg_in_chat_with_curator = msg_in_chat_with_curator)
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
        headers = {"cookie": f"{get_cookie()}"}
        async with session.get(f"{URL_SERVER}/core/student/{user_data.get('user_id')}/courses", headers=headers) as response:
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
            headers = {"cookie": f"{get_cookie()}"}
            async with session.get(f"{URL_SERVER}/parser/course/search", headers=headers, params={"name": courses_data[course_id]["name"], "university": courses_data[course_id]["university"]}) as response:
                if response.status == 200:
                    course_data = await response.json()
                else:
                    course_data = {}

        await callback_query.message.delete()
        await callback_query.message.answer(f"Информация о курсе {course_data['name']}\n{course_data['info']}\n{course_data['date']}\n{course_data['university']}\n\n Баллы: {courses_data[course_id]['score']}\n\n Если возникли вопросы по входу на онлайн курс, изучите раздел информация об обучении/онлайн курсы", reply_markup=keyboard.Back_to_courses())
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())


@router.callback_query(lambda c: c.data == "subjects")
async def process_subjects(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    # Отправляем запрос на сервер для получения списка онлайн курсов
    async with aiohttp.ClientSession() as session:
        headers = {"cookie": f"{get_cookie()}"}
        async with session.get(f"{URL_SERVER}/core/student/{user_data.get('user_id')}", headers=headers) as response:
            if response.status == 200:
                st_data = await response.json()
                info = st_data.get("info")
                subjects_data = info.get("subjects")
            else:
                subjects_data = []
    await state.update_data(subjects_data=subjects_data)

    if user_data.get('email') and user_data.get('personal_number'):
        await callback_query.message.delete()
        await callback_query.message.answer(create_text_subject(subjects_data), reply_markup=keyboard.Back_to_main())
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())

def create_text_subject(data: list):
    res = ""
    i = 1
    for item in data:
        item = item
        full_name = item.get("fullName")
        res += f"{i}. {full_name}\n\n"
        i += 1
    return res


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



@router.callback_query(LKStates.WAITING_CHAT_WITH_CURATOR, lambda c: c.data == "main_menu")
async def process_main_menu(callback_query: types.CallbackQuery, state: FSMContext): 
    await state.set_state(LKStates.MAIN_MENU)   
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        del_msg = user_data.get('msg_in_chat_with_curator')
        if del_msg:
            await callback_query.message.bot.delete_messages(chat_id=callback_query.message.chat.id, message_ids=del_msg)
        user_data.pop("msg_in_chat_with_curator")
        await state.clear()
        await state.update_data(**user_data)
        await show_main_menu(callback_query.message, state)
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())



@router.callback_query(Info_teaching.INFO, lambda c: c.data == "main_menu")
async def process_main_menu(callback_query: types.CallbackQuery, state: FSMContext): 
    await state.set_state(LKStates.MAIN_MENU)   
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        await callback_query.message.delete()
        await show_main_menu(callback_query.message, state)
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



@router.callback_query(lambda c: c.data == "info_teaching")
async def process_main_menu(callback_query: types.CallbackQuery, state: FSMContext): 
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        await state.set_state(Info_teaching.INFO)   
        await callback_query.message.delete()
        await show_info_teaching(callback_query.message, state)
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



@router.message()
async def chat(message: types.Message, state: FSMContext):
    await message.delete()
