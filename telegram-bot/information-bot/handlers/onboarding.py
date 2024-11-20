from typing import List
from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext

from states import LKStates, Onboarding
import keyboards.onboarding as kb_onboarding
from handlers.main_menu import process_main_menu, show_main_menu
from texts.error import Registration
from texts.onboarding import Onboarding as t_onboarding
from aiogram.fsm.state import State, StatesGroup
from config import MANAGER_ONB

router = Router()

class FormStates(StatesGroup):
    waiting_for_courses = State()
    waiting_for_section = State()
    waiting_for_topic = State()





async def add_msgs_in_dels(state, msgs):
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    for msg in msgs:
        del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)


async def del_msg(message: types.Message, msgs: list[types.Message]):
    await message.bot.delete_messages(message.chat.id, msgs)


@router.callback_query(lambda c: c.data == "start_onboarding")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    await state.set_state(FormStates.waiting_for_section)

    await state.update_data(index_additional_course=MANAGER_ONB.get_index_onboarding())

    crossed_topics = user_data.get("crossed_topics")

    await callback_query.message.answer("Прогресс по всем разделам 100%\n\nВыберите раздел:", reply_markup=kb_onboarding.get_sections_keyboard(MANAGER_ONB.onboarding, crossed_topics=crossed_topics))

    await callback_query.message.delete()


@router.callback_query(lambda c: c.data == "additional_courses")
async def additional_courses(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(FormStates.waiting_for_courses)

    user_data = await state.get_data()
    crossed_topics = user_data.get("crossed_topics")

    add_course = MANAGER_ONB.get_additional_courses()

    await callback_query.message.answer("Выберите курс:", reply_markup=kb_onboarding.additional_courses_keyboard(add_course, crossed_topics=crossed_topics))

    await callback_query.message.delete()


@router.callback_query(FormStates.waiting_for_courses, lambda c: c.data == "end_onboarding")
async def end_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()

    if user_data.get('user_id'):
        await state.set_state(LKStates.MAIN_MENU)
        await show_main_menu(callback_query.message, state)
        await callback_query.message.delete()
    else:
        await callback_query.message.answer(Registration.no())


@router.callback_query(FormStates.waiting_for_courses, lambda c: c.data.split("__")[0] == "additional_courses")
async def additional_courses(callback_query: types.CallbackQuery, state: FSMContext):
    index_course = callback_query.data.split("__")[-1]

    await state.update_data(index_additional_course=index_course)

    await start_additional_courses(callback_query, state)


async def start_additional_courses(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(FormStates.waiting_for_section)

    user_data = await state.get_data()
    index_add_course = int(user_data.get("index_additional_course"))
    crossed_topics = user_data.get("crossed_topics")

    await callback_query.message.answer("Прогресс по всем разделам 100%\n\nВыберите раздел:", reply_markup=kb_onboarding.get_sections_keyboard(MANAGER_ONB.get_info_course(index_add_course), from_add_course=True, crossed_topics=crossed_topics))

    await callback_query.message.delete()


#Информация о боте
async def choice_onboarding(message: types.Message, state: FSMContext):
    await state.set_state(Onboarding.QUE_START)

    last_msg = await message.answer(t_onboarding.info_bot())
    msg = await message.answer(t_onboarding.start(), reply_markup=kb_onboarding.start_choice())

    await state.update_data(del_msgs_onboarding = [last_msg.message_id, msg.message_id])


@router.callback_query(Onboarding.QUE_START, lambda c: c.data == "start_onboarding_start" or c.data == "onboarding_sections")
async def menu_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    list_del_msg = user_data.get("del_msgs_onboarding")
    await del_msg(callback_query.message, list_del_msg)

    crossed_topics = user_data.get("crossed_topics")

    await state.update_data(index_additional_course=MANAGER_ONB.get_index_onboarding())

    await state.set_state(FormStates.waiting_for_section)
    await callback_query.message.answer("Прогресс по всем разделам 100%\n\nВыберите раздел:", reply_markup=kb_onboarding.get_sections_keyboard(MANAGER_ONB.onboarding, crossed_topics=crossed_topics))


@router.callback_query(lambda c: c.data == "onboarding_sections")
async def menu_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await callback_query.message.delete()

    user_data = await state.get_data()
    index_add_course = int(user_data.get("index_additional_course"))
    crossed_topics = user_data.get("crossed_topics")

    await state.set_state(FormStates.waiting_for_section)
    await callback_query.message.answer("Прогресс по всем разделам 100%\n\nВыберите раздел:", reply_markup=kb_onboarding.get_sections_keyboard(MANAGER_ONB.get_info_course(index_add_course), crossed_topics=crossed_topics))


@router.callback_query(FormStates.waiting_for_section)
async def section(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(FormStates.waiting_for_topic)

    user_data = await state.get_data()
    index_add_course = int(user_data.get("index_additional_course"))
    del_msg_add_course = user_data.get("del_msg_add_course")

    if del_msg_add_course != None:
        await del_msg(callback_query.message, del_msg_add_course)
    
    crossed_topics = user_data.get("crossed_topics")

    if callback_query.data == "end_onboarding" or callback_query.data == "to_section___end_onboarding":
        await show_main_menu(callback_query.message, state)
        await callback_query.message.delete()

    elif callback_query.data.split("___")[0] == "to_section":
        await callback_query.message.answer(f"Прогресс по рузделу 50%\n\nВыберите тему: {callback_query.data}", reply_markup=kb_onboarding.get_topics_keyboard(MANAGER_ONB.get_info_course(index_add_course), "___".join(callback_query.data.split("___")[1:]), crossed_topics))
    
    else:
        await callback_query.message.answer(f"Прогресс по рузделу 50%\n\nВыберите тему: {callback_query.data}", reply_markup=kb_onboarding.get_topics_keyboard(MANAGER_ONB.get_info_course(index_add_course), callback_query.data, crossed_topics))
        await callback_query.message.delete()


@router.callback_query(FormStates.waiting_for_topic, lambda c: c.data.split("____")[-1] == "help")
async def topic_question(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()

    info_course = user_data.get("info_course")
    info_topic = user_data.get("info_topic")

    await callback_query.message.edit_reply_markup()

    if callback_query.data.split("___")[0] == "to_section":
        await state.set_state(FormStates.waiting_for_section)
        await section(callback_query, state)
        await callback_query.message.delete()
    else:
        await callback_query.message.answer(callback_query.data, reply_markup=kb_onboarding.topic_keyboard(info_course, callback_query.data.split("____")[0], is_help=True))



@router.callback_query(FormStates.waiting_for_topic)
async def topic(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    index_add_course = int(user_data.get("index_additional_course"))

    info_course = MANAGER_ONB.get_info_course(index_add_course)
    await state.update_data(info_course = info_course)

    if callback_query.data.split("___")[0] == "to_section":
        await state.set_state(FormStates.waiting_for_section)
        await section(callback_query, state)
        await callback_query.message.delete()
    else:
        info_topic = MANAGER_ONB.get_info_course_topic(index_add_course, callback_query.data)

        await state.update_data(info_topic = info_topic)

        crossed_topics = user_data.get("crossed_topics")
        split_info_topic = callback_query.data.split("__")
        dict_info = ("__".join(split_info_topic[:-1]), split_info_topic[-1], info_course["name_course"])


        if crossed_topics == None:
            await state.update_data(crossed_topics = {dict_info[2]: {dict_info[0]: [dict_info[1]]}})
        else:
            if dict_info[2] in crossed_topics.keys():
                if dict_info[0] in crossed_topics[dict_info[2]]:
                    if dict_info[1] not in crossed_topics[dict_info[2]][dict_info[0]]:
                        crossed_topics[dict_info[2]][dict_info[0]].append(dict_info[1])
                else:
                    crossed_topics[dict_info[2]][dict_info[0]] = [dict_info[1]]
            else:
                crossed_topics[dict_info[2]] = {}
                crossed_topics[dict_info[2]][dict_info[0]] = [dict_info[1]]

            await state.update_data(crossed_topics = crossed_topics)      
            

        del_msg_add_course = user_data.get("del_msg_add_course")

        if del_msg_add_course != None:
            await del_msg(callback_query.message, del_msg_add_course)

        if info_topic["question"] != None and info_topic["answer"] != None:
            msg1 = await callback_query.message.answer(callback_query.data)
            await callback_query.message.answer(callback_query.data, reply_markup=kb_onboarding.topic_keyboard(info_course, callback_query.data))

            await state.update_data(del_msg_add_course = [msg1.message_id])

            await callback_query.message.delete()

        else:
            await callback_query.message.answer(callback_query.data, reply_markup=kb_onboarding.topic_keyboard(info_course, callback_query.data, not_question=True))

            await callback_query.message.delete()







# #Информация о ЛК
# @router.callback_query(Onboarding.QUE_START, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_PERSONAL_ACCOUNT)
   
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])
    
#     await callback_query.message.answer(t_onboarding.info_personal_account(), reply_markup=kb_onboarding.next())

# #Получилось посмотреть?
# @router.callback_query(Onboarding.INFO_PERSONAL_ACCOUNT, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_PERSONAL_ACCOUNT)
    
#     last_msg = await callback_query.message.edit_reply_markup()
#     msg = await callback_query.message.answer(t_onboarding.que_personal_account(), reply_markup=kb_onboarding.yes_no_help())

#     await state.update_data(del_msgs_onboarding = [last_msg.message_id, msg.message_id])


# #Да, переходим к модеусу
# @router.callback_query(Onboarding.QUE_PERSONAL_ACCOUNT, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_MODEUS)
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_modeus(), reply_markup=kb_onboarding.next())

# #Подсказка, если нет
# @router.callback_query(Onboarding.QUE_PERSONAL_ACCOUNT, lambda c: c.data == "no_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.ANS_PC)

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.ans_personal_account(), reply_markup=kb_onboarding.worked_and_chat_curator())

# #Переходим к модеусу
# @router.callback_query(Onboarding.ANS_PC, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_MODEUS)
#     await callback_query.message.delete()

#     await callback_query.message.answer(t_onboarding.info_modeus(), reply_markup=kb_onboarding.next())
    
# #Получилось посмотреть?
# @router.callback_query(Onboarding.INFO_MODEUS, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_MODEUS)
    
#     last_msg = await callback_query.message.edit_reply_markup()
#     msg = await callback_query.message.answer(t_onboarding.que_modeus(), reply_markup=kb_onboarding.yes_no_help())

#     await state.update_data(del_msgs_onboarding = [last_msg.message_id, msg.message_id])


# #Да, переходим к ИОТ
# @router.callback_query(Onboarding.QUE_MODEUS, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_IOT)
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_iot(), reply_markup=kb_onboarding.next())

# #Подсказка, если нет
# @router.callback_query(Onboarding.QUE_MODEUS, lambda c: c.data == "no_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.ANS_MODEUS)

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.ans_modeus(), reply_markup=kb_onboarding.worked_and_chat_curator())

# #Переходим к ИОТ
# @router.callback_query(Onboarding.ANS_MODEUS, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_IOT)
#     await callback_query.message.delete()

#     await callback_query.message.answer(t_onboarding.info_iot(), reply_markup=kb_onboarding.next())
    
# #Вопросы по ИОТ
# @router.callback_query(Onboarding.INFO_IOT, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_IOT)
    
#     last_msg = await callback_query.message.edit_reply_markup()
#     await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(True, True))

#     await state.update_data(del_msgs_onboarding = [last_msg.message_id])


# #Да, переходим к студенческому билету
# @router.callback_query(Onboarding.QUE_IOT, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_STUD_CARD)
#     await callback_query.message.delete()
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_stud_card(), reply_markup=kb_onboarding.next())

# #Вопрос о дисциплинах
# @router.callback_query(Onboarding.QUE_IOT, lambda c: c.data == "iot_disc")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_IOT_DISC)
#     await callback_query.message.delete()
    
#     msg = await callback_query.message.answer(t_onboarding.iot_disc())
#     await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(False, True))

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")
#     del_msgs_onboarding.append(msg.message_id)
#     await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

# #Вопрос о выборе
# @router.callback_query(Onboarding.QUE_IOT, lambda c: c.data == "iot_choice")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_IOT_CHIOSE)
#     await callback_query.message.delete()
    
#     msg = await callback_query.message.answer(t_onboarding.iot_choice())
#     await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(True, False))

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")
#     del_msgs_onboarding.append(msg.message_id)
#     await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

# #Вопрос о дисциплинах после вопроса о выборе
# @router.callback_query(Onboarding.QUE_IOT_CHIOSE, lambda c: c.data == "iot_disc")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_IOT_DISC)
#     await callback_query.message.delete()
    
#     msg = await callback_query.message.answer(t_onboarding.iot_disc())
#     await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(False, False))

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")
#     del_msgs_onboarding.append(msg.message_id)
#     await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

# #Вопрос о выборе после вопроса о дисциплинах
# @router.callback_query(Onboarding.QUE_IOT_DISC, lambda c: c.data == "iot_choice")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_IOT_CHIOSE)
#     await callback_query.message.delete()
    
#     msg = await callback_query.message.answer(t_onboarding.iot_choice())
#     await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(False, False))

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")
#     del_msgs_onboarding.append(msg.message_id)
#     await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

# #Из вопрос о дисциплинах переходим к студенческому билету
# @router.callback_query(Onboarding.QUE_IOT_DISC, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_STUD_CARD)
#     await callback_query.message.delete()
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_stud_card(), reply_markup=kb_onboarding.next())

# #Из вопрос о выборе переходим к студенческому билету
# @router.callback_query(Onboarding.QUE_IOT_CHIOSE, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_STUD_CARD)
#     await callback_query.message.delete()
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_stud_card(), reply_markup=kb_onboarding.next())


# #Вопрос о студ билетах
# @router.callback_query(Onboarding.INFO_STUD_CARD, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_STUD_CARD)
    
#     last_msg = await callback_query.message.edit_reply_markup()
#     await callback_query.message.answer(t_onboarding.que_stud_card(), reply_markup=kb_onboarding.yes_no())

#     await state.update_data(del_msgs_onboarding = [last_msg.message_id])


# #Переходим к электронным пропускам
# @router.callback_query(Onboarding.QUE_STUD_CARD, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_ELECTR_PASS)
#     await callback_query.message.delete()
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_electr_pass(), reply_markup=kb_onboarding.next())


# #Переходим к банковским картам
# @router.callback_query(Onboarding.INFO_ELECTR_PASS, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_BANK_CARD)
#     await callback_query.message.delete()

#     await callback_query.message.answer(t_onboarding.info_bank_card(), reply_markup=kb_onboarding.next())
    

# #Переходим к проектам
# @router.callback_query(Onboarding.INFO_BANK_CARD, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_PROGECT)
#     await callback_query.message.delete()
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_project(), reply_markup=kb_onboarding.next())

# #Переходим к ответу
# @router.callback_query(Onboarding.QUE_STUD_CARD, lambda c: c.data == "no_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.ANS_STUD_CARD)
#     await callback_query.message.delete()
    
#     await callback_query.message.answer(t_onboarding.ans_stud_card(), reply_markup=kb_onboarding.next())

# #Переходим к проектам из ответа
# @router.callback_query(Onboarding.ANS_STUD_CARD, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_PROGECT)
#     await callback_query.message.delete()
    
#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.info_project(), reply_markup=kb_onboarding.next())


# #Переходим к экзаменам
# @router.callback_query(Onboarding.INFO_PROGECT, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_EXAM)
#     await callback_query.message.delete()

#     await callback_query.message.answer(t_onboarding.info_exam(), reply_markup=kb_onboarding.next())

# #Переходим к вопросам по экзаменам
# @router.callback_query(Onboarding.INFO_EXAM, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_EXAM)
    
#     last_msg = await callback_query.message.edit_reply_markup()

#     await state.update_data(del_msgs_onboarding = [last_msg.message_id])
    
#     await callback_query.message.answer(t_onboarding.que_exam(), reply_markup=kb_onboarding.que_exam())


# #Ответ на вопрос
# @router.callback_query(Onboarding.QUE_EXAM, lambda c: c.data == "info_exam")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await callback_query.message.delete()
    
#     await callback_query.message.answer(t_onboarding.ans_exam(), reply_markup=kb_onboarding.next())


# #Рейтинги
# @router.callback_query(Onboarding.QUE_EXAM, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.INFO_RATNG)
#     await callback_query.message.delete()

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])
    
#     msg = await callback_query.message.answer(t_onboarding.info_rating(), reply_markup=kb_onboarding.next())

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")
#     del_msgs_onboarding.append(msg.message_id)
#     await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

# #Вопросы по рейтингам
# @router.callback_query(Onboarding.INFO_RATNG, lambda c: c.data == "next_onb")
# async def que_rating_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.QUE_RATING)

#     last_msg = await callback_query.message.edit_reply_markup()

#     await state.update_data(del_msgs_onboarding = [last_msg.message_id])

#     await callback_query.message.answer(t_onboarding.que_rating(), reply_markup=kb_onboarding.que_rating())

# #Вопрос 1
# @router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "science_rating")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await callback_query.message.delete()

#     await callback_query.message.answer(t_onboarding.science_rating(), reply_markup=kb_onboarding.back_to_rating())

# #Вопрос 2
# @router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "extr_rating")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await callback_query.message.delete()

#     await callback_query.message.answer(t_onboarding.extr_rating(), reply_markup=kb_onboarding.back_to_rating())
# #Вопрос 3
# @router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "all_rating")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await callback_query.message.delete()

#     await callback_query.message.answer(t_onboarding.all_rating(), reply_markup=kb_onboarding.back_to_rating())

# #Назад к рейтингам
# @router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "back_rating")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await callback_query.message.delete()
#     await callback_query.message.answer(t_onboarding.que_rating(), reply_markup=kb_onboarding.que_rating())
    
# @router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await state.set_state(Onboarding.END)
#     await callback_query.message.delete()

#     user_data = await state.get_data()
#     del_msgs_onboarding = user_data.get("del_msgs_onboarding")

#     for msg in del_msgs_onboarding:
#         await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
#     await state.update_data(del_msgs_onboarding = [])

#     await callback_query.message.answer(t_onboarding.end(), reply_markup=kb_onboarding.to_main_menu())

# #Конец
# @router.callback_query(Onboarding.END, lambda c: c.data == "next_onb")
# async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
#     await end(callback_query, state)
#     await callback_query.message.delete()

@router.callback_query(lambda c: c.data == "end_onboarding")
async def end(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)

    await state.update_data(del_msgs_onboarding = [])

    if user_data.get('user_id'):
        await state.set_state(LKStates.MAIN_MENU)
        await show_main_menu(callback_query.message, state)
    else:
        await callback_query.message.answer(Registration.no())
