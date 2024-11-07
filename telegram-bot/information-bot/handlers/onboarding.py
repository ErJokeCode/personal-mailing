from typing import List
from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext

from states import LKStates, Onboarding
import keyboards.onboarding as kb_onboarding
from handlers.main_menu import process_main_menu, show_main_menu
from texts.error import Registration
from texts.onboarding import Onboarding as t_onboarding

router = Router()

async def add_msgs_in_dels(state, msgs):
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    for msg in msgs:
        del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

@router.callback_query(lambda c: c.data == "start_onboarding")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await choice_onboarding(callback_query.message, state)
    await callback_query.message.delete()

#Информация о боте
async def choice_onboarding(message: types.Message, state: FSMContext):
    await state.set_state(Onboarding.INFO_START)

    await message.answer(t_onboarding.info_bot(), reply_markup=kb_onboarding.next())

#Продолжим?
@router.callback_query(Onboarding.INFO_START, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_START)

    last_msg = await callback_query.message.edit_reply_markup()
    msg = await callback_query.message.answer(t_onboarding.start(), reply_markup=kb_onboarding.start_choice())

    await state.update_data(del_msgs_onboarding = [last_msg.message_id, msg.message_id])


#Информация о ЛК
@router.callback_query(Onboarding.QUE_START, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_PERSONAL_ACCOUNT)
   
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])
    
    await callback_query.message.answer(t_onboarding.info_personal_account(), reply_markup=kb_onboarding.next())

#Получилось посмотреть?
@router.callback_query(Onboarding.INFO_PERSONAL_ACCOUNT, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_PERSONAL_ACCOUNT)
    
    last_msg = await callback_query.message.edit_reply_markup()
    msg = await callback_query.message.answer(t_onboarding.que_personal_account(), reply_markup=kb_onboarding.yes_no_help())

    await state.update_data(del_msgs_onboarding = [last_msg.message_id, msg.message_id])


#Да, переходим к модеусу
@router.callback_query(Onboarding.QUE_PERSONAL_ACCOUNT, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_MODEUS)
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_modeus(), reply_markup=kb_onboarding.next())

#Подсказка, если нет
@router.callback_query(Onboarding.QUE_PERSONAL_ACCOUNT, lambda c: c.data == "no_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.ANS_PC)

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.ans_personal_account(), reply_markup=kb_onboarding.worked_and_chat_curator())

#Переходим к модеусу
@router.callback_query(Onboarding.ANS_PC, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_MODEUS)
    await callback_query.message.delete()

    await callback_query.message.answer(t_onboarding.info_modeus(), reply_markup=kb_onboarding.next())
    
#Получилось посмотреть?
@router.callback_query(Onboarding.INFO_MODEUS, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_MODEUS)
    
    last_msg = await callback_query.message.edit_reply_markup()
    msg = await callback_query.message.answer(t_onboarding.que_modeus(), reply_markup=kb_onboarding.yes_no_help())

    await state.update_data(del_msgs_onboarding = [last_msg.message_id, msg.message_id])


#Да, переходим к ИОТ
@router.callback_query(Onboarding.QUE_MODEUS, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_IOT)
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_iot(), reply_markup=kb_onboarding.next())

#Подсказка, если нет
@router.callback_query(Onboarding.QUE_MODEUS, lambda c: c.data == "no_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.ANS_MODEUS)

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.ans_modeus(), reply_markup=kb_onboarding.worked_and_chat_curator())

#Переходим к ИОТ
@router.callback_query(Onboarding.ANS_MODEUS, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_IOT)
    await callback_query.message.delete()

    await callback_query.message.answer(t_onboarding.info_iot(), reply_markup=kb_onboarding.next())
    
#Вопросы по ИОТ
@router.callback_query(Onboarding.INFO_IOT, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_IOT)
    
    last_msg = await callback_query.message.edit_reply_markup()
    await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(True, True))

    await state.update_data(del_msgs_onboarding = [last_msg.message_id])


#Да, переходим к студенческому билету
@router.callback_query(Onboarding.QUE_IOT, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_STUD_CARD)
    await callback_query.message.delete()
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_stud_card(), reply_markup=kb_onboarding.next())

#Вопрос о дисциплинах
@router.callback_query(Onboarding.QUE_IOT, lambda c: c.data == "iot_disc")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_IOT_DISC)
    await callback_query.message.delete()
    
    msg = await callback_query.message.answer(t_onboarding.iot_disc())
    await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(False, True))

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

#Вопрос о выборе
@router.callback_query(Onboarding.QUE_IOT, lambda c: c.data == "iot_choice")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_IOT_CHIOSE)
    await callback_query.message.delete()
    
    msg = await callback_query.message.answer(t_onboarding.iot_choice())
    await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(True, False))

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

#Вопрос о дисциплинах после вопроса о выборе
@router.callback_query(Onboarding.QUE_IOT_CHIOSE, lambda c: c.data == "iot_disc")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_IOT_DISC)
    await callback_query.message.delete()
    
    msg = await callback_query.message.answer(t_onboarding.iot_disc())
    await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(False, False))

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

#Вопрос о выборе после вопроса о дисциплинах
@router.callback_query(Onboarding.QUE_IOT_DISC, lambda c: c.data == "iot_choice")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_IOT_CHIOSE)
    await callback_query.message.delete()
    
    msg = await callback_query.message.answer(t_onboarding.iot_choice())
    await callback_query.message.answer(t_onboarding.que_iot(), reply_markup=kb_onboarding.que_iot(False, False))

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

#Из вопрос о дисциплинах переходим к студенческому билету
@router.callback_query(Onboarding.QUE_IOT_DISC, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_STUD_CARD)
    await callback_query.message.delete()
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_stud_card(), reply_markup=kb_onboarding.next())

#Из вопрос о выборе переходим к студенческому билету
@router.callback_query(Onboarding.QUE_IOT_CHIOSE, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_STUD_CARD)
    await callback_query.message.delete()
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_stud_card(), reply_markup=kb_onboarding.next())


#Вопрос о студ билетах
@router.callback_query(Onboarding.INFO_STUD_CARD, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_STUD_CARD)
    
    last_msg = await callback_query.message.edit_reply_markup()
    await callback_query.message.answer(t_onboarding.que_stud_card(), reply_markup=kb_onboarding.yes_no())

    await state.update_data(del_msgs_onboarding = [last_msg.message_id])


#Переходим к электронным пропускам
@router.callback_query(Onboarding.QUE_STUD_CARD, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_ELECTR_PASS)
    await callback_query.message.delete()
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_electr_pass(), reply_markup=kb_onboarding.next())


#Переходим к банковским картам
@router.callback_query(Onboarding.INFO_ELECTR_PASS, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_BANK_CARD)
    await callback_query.message.delete()

    await callback_query.message.answer(t_onboarding.info_bank_card(), reply_markup=kb_onboarding.next())
    

#Переходим к проектам
@router.callback_query(Onboarding.INFO_BANK_CARD, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_PROGECT)
    await callback_query.message.delete()
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_project(), reply_markup=kb_onboarding.next())

#Переходим к ответу
@router.callback_query(Onboarding.QUE_STUD_CARD, lambda c: c.data == "no_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.ANS_STUD_CARD)
    await callback_query.message.delete()
    
    await callback_query.message.answer(t_onboarding.ans_stud_card(), reply_markup=kb_onboarding.next())

#Переходим к проектам из ответа
@router.callback_query(Onboarding.ANS_STUD_CARD, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_PROGECT)
    await callback_query.message.delete()
    
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.info_project(), reply_markup=kb_onboarding.next())


#Переходим к экзаменам
@router.callback_query(Onboarding.INFO_PROGECT, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_EXAM)
    await callback_query.message.delete()

    await callback_query.message.answer(t_onboarding.info_exam(), reply_markup=kb_onboarding.next())

#Переходим к вопросам по экзаменам
@router.callback_query(Onboarding.INFO_EXAM, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_EXAM)
    
    last_msg = await callback_query.message.edit_reply_markup()

    await state.update_data(del_msgs_onboarding = [last_msg.message_id])
    
    await callback_query.message.answer(t_onboarding.que_exam(), reply_markup=kb_onboarding.que_exam())


#Ответ на вопрос
@router.callback_query(Onboarding.QUE_EXAM, lambda c: c.data == "info_exam")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await callback_query.message.delete()
    
    await callback_query.message.answer(t_onboarding.ans_exam(), reply_markup=kb_onboarding.next())


#Рейтинги
@router.callback_query(Onboarding.QUE_EXAM, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.INFO_RATNG)
    await callback_query.message.delete()

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])
    
    msg = await callback_query.message.answer(t_onboarding.info_rating(), reply_markup=kb_onboarding.next())

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)

#Вопросы по рейтингам
@router.callback_query(Onboarding.INFO_RATNG, lambda c: c.data == "next_onb")
async def que_rating_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.QUE_RATING)

    last_msg = await callback_query.message.edit_reply_markup()

    await state.update_data(del_msgs_onboarding = [last_msg.message_id])

    await callback_query.message.answer(t_onboarding.que_rating(), reply_markup=kb_onboarding.que_rating())

#Вопрос 1
@router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "science_rating")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await callback_query.message.delete()

    await callback_query.message.answer(t_onboarding.science_rating(), reply_markup=kb_onboarding.back_to_rating())

#Вопрос 2
@router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "extr_rating")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await callback_query.message.delete()

    await callback_query.message.answer(t_onboarding.extr_rating(), reply_markup=kb_onboarding.back_to_rating())
#Вопрос 3
@router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "all_rating")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await callback_query.message.delete()

    await callback_query.message.answer(t_onboarding.all_rating(), reply_markup=kb_onboarding.back_to_rating())

#Назад к рейтингам
@router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "back_rating")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await callback_query.message.delete()
    await callback_query.message.answer(t_onboarding.que_rating(), reply_markup=kb_onboarding.que_rating())
    
@router.callback_query(Onboarding.QUE_RATING, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await state.set_state(Onboarding.END)
    await callback_query.message.delete()

    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)
    
    await state.update_data(del_msgs_onboarding = [])

    await callback_query.message.answer(t_onboarding.end(), reply_markup=kb_onboarding.to_main_menu())

#Конец
@router.callback_query(Onboarding.END, lambda c: c.data == "next_onb")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await end(callback_query, state)
    await callback_query.message.delete()

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
