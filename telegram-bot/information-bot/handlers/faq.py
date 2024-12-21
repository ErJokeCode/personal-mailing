from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext

import keyboards.main_menu as keyboard
from config import MANAGER_FAQ


router = Router()

@router.callback_query(lambda c: c.data.split("_")[0] == "faq")
async def faq_online_course(callback_query: types.CallbackQuery, state: FSMContext):
    data = MANAGER_FAQ.get_all(callback_data="_".join(callback_query.data.split("_")[1:]))
    
    faq_text = f"Тема: {data[0].topic}\n\n"
    for question in data:
        faq_text += f"Вопрос: {question.question}\nОтвет: {question.answer}\n\n"
        
    await callback_query.message.delete()
    await callback_query.message.answer(faq_text, reply_markup=keyboard.back_to_FAQ())
