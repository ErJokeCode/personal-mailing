from aiogram import Router
from aiogram import types
from aiogram.fsm.context import FSMContext

from states import Info_teaching
from keyboards.info_teaching import next_and_back, yes_and_no


router = Router()

async def show_info_teaching(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    if user_data.get('email') and user_data.get('personal_number'):
        await message.answer("Пройти краткий экскурс по обучению?", reply_markup=yes_and_no())
    else:
        await message.delete()
        await message.answer("Вы не зарегистрированы. Пожалуйста, введите вашу почту и номер студенческого билета. Для повторного входа используйте команду /start")

@router.callback_query(lambda c: c.data == "yes")
async def yes(message: types.Message, state: FSMContext):
    await message.answer("Пройти краткий экскурс по обучению", reply_markup=next_and_back())
    await state.set_state(Info_teaching.CARD)
    await card()

@router.message(Info_teaching.INFO)
async def card(message: types.Message, state: FSMContext):
    await message.answer("Пройти краткий экскурс по обучению", reply_markup=next_and_back())

@router.message(Info_teaching.CARD)
async def card(message: types.Message, state: FSMContext):
    await message.answer("Текст о студенческих билетах и пропусках", reply_markup=next_and_back())


