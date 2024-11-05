from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext

import keyboards.main_menu as keyboard
from texts.error import Registration


router = Router()

@router.callback_query(lambda c: c.data == "faq_online_course")
async def faq_online_course(callback_query: types.CallbackQuery, state: FSMContext):
    faq_text = "Вопрос 1 \nОтвет \n\nВопрос 2 \nОтвет "
    await callback_query.message.delete()
    await callback_query.message.answer(faq_text, reply_markup=keyboard.back_to_FAQ())


@router.callback_query(lambda c: c.data == "faq_vuc")
async def faq_vuc(callback_query: types.CallbackQuery, state: FSMContext):
    faq_text = "Вопрос 1 \nОтвет \n\nВопрос 2 \nОтвет "
    await callback_query.message.delete()
    await callback_query.message.answer(faq_text, reply_markup=keyboard.back_to_FAQ())