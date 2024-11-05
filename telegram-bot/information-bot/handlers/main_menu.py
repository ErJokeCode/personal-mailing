from asyncio import sleep
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
    if user_data.get('user_id'):
        await message.answer("Главное меню\n\n👋 Привет! Я ваш помощник. Вы можете получить доступ к следующей информации:\n\n📚 Основная информация о предметах\nУзнайте расписание, материалы и темы лекций по вашим предметам.\n\n💻 Онлайн-курсы\nПросмотрите доступные онлайн-курсы, их описание и расписание. \n\n🔔 Уведомления\nПолучите последние уведомления о важных событиях, изменениях в расписании и других новостях.", reply_markup=keyboard.menu())
        
        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{get_cookie()}"}
            async with session.get(f"{URL_SERVER}/core/student/{user_data.get('user_id')}", headers=headers) as response:
                if response.status == 200:
                    student_data = await response.json()
                    student_info = student_data.get("info")
                    subjects = student_info.get("subjects")
                    online_courses = student_info.get("onlineCourse")
                    onboard_status = student_info.get("onboardStatus")

                    await state.update_data(subjects=subjects)
                    await state.update_data(online_courses=online_courses)
                    await state.update_data(onboard_status=onboard_status)
    else:
        await message.delete()
        await message.answer(Registration.no())



@router.callback_query(lambda c: c.data == "online_courses")
async def process_online_courses(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()

    if user_data.get('user_id'):
        await callback_query.message.delete()
        await callback_query.message.answer("Выберите онлайн курс:", reply_markup=keyboard.courses(courses_data=user_data.get("online_courses")))
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())



@router.callback_query(lambda c: c.data.startswith("course_"))
async def process_course_info(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()

    if user_data.get('user_id'):
        online_courses = user_data.get('online_courses')
        course_id = int(callback_query.data.split("_")[1])

        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{get_cookie()}"}

            async with session.get(f"{URL_SERVER}/parser/course/search", headers=headers, params={"name": online_courses[course_id]["name"], "university": online_courses[course_id]["university"]}) as response:
                if response.status == 200:
                    indo_course = await response.json()
                    await callback_query.message.delete()
                    await callback_query.message.answer(f"Информация о курсе {indo_course['name']}\n{indo_course['university']}\n{indo_course['date_start']}\n{indo_course['deadline']}\n{indo_course['info']}\n\n Баллы: {online_courses[course_id]['score']}\n\n Если возникли вопросы по входу на онлайн курс, изучите раздел информация об обучении/онлайн курсы", reply_markup=keyboard.back_to_courses())
                else:
                    not_info = await callback_query.message.answer("Пока нет информации, подожтите немного")
                    sleep(5)
                    await not_info.delete()
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())


@router.callback_query(lambda c: c.data == "subjects")
async def process_subjects(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()

    if user_data.get('user_id'):
        subjects = user_data.get("subjects")
        await callback_query.message.delete()
        await callback_query.message.answer(create_text_subject(subjects), reply_markup=keyboard.back_to_main())
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
    faq_text = "Часто задаваемые вопросы, выберите раздел"
    await callback_query.message.delete()
    await callback_query.message.answer(faq_text, reply_markup=keyboard.FAQ())


@router.callback_query(lambda c: c.data == "main_menu")
async def process_main_menu(callback_query: types.CallbackQuery, state: FSMContext): 
    await state.set_state(LKStates.MAIN_MENU)   
    user_data = await state.get_data()

    if user_data.get('user_id'):
        await callback_query.message.delete()
        await show_main_menu(callback_query.message, state)
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())



@router.callback_query(lambda c: c.data == "info_teaching")
async def process_main_menu(callback_query: types.CallbackQuery, state: FSMContext): 
    user_data = await state.get_data()

    if user_data.get('user_id'):
        await state.set_state(Info_teaching.INFO)   
        await callback_query.message.delete()
        await show_info_teaching(callback_query.message, state)
    else:
        await callback_query.message.delete()
        await callback_query.message.answer(Registration.no())


@router.message()
async def chat(message: types.Message, state: FSMContext):
    await message.delete()
