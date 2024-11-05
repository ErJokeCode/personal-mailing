from aiogram import F, Router, types
from aiogram.fsm.context import FSMContext

from states import LKStates, Onboarding
import keyboards.onboarding as kd_onboarding
from handlers.main_menu import process_main_menu, show_main_menu
from texts.error import Registration

router = Router()

async def choice_onboarding(message: types.Message, state: FSMContext):
    user_data = await state.get_data()
    if user_data.get('user_id'):
        await message.answer("Пройди краткий экскурс в учебный процесс", reply_markup=kd_onboarding.choice())


@router.callback_query(lambda c: c.data == "start_onboarding")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await callback_query.message.delete()
    await one(callback_query, state)


@router.callback_query(lambda c: c.data == "skip_onboarding")
async def start_onboarding(callback_query: types.CallbackQuery, state: FSMContext):
    await process_main_menu(callback_query, state)


@router.callback_query(lambda c: c.data == "one")
async def one(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    await state.set_state(Onboarding.ONE)

    msg = await callback_query.message.answer("One", reply_markup=kd_onboarding.one())
    await state.update_data(del_msgs_onboarding = [msg.message_id])


@router.callback_query(lambda c: c.data == "two")
async def two(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    await state.set_state(Onboarding.TWO)
    await callback_query.message.edit_reply_markup()

    msg = await callback_query.message.answer("Two", reply_markup=kd_onboarding.end())
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")
    del_msgs_onboarding.append(msg.message_id)
    await state.update_data(del_msgs_onboarding = del_msgs_onboarding)


@router.callback_query(lambda c: c.data == "end_onboarding")
async def two(callback_query: types.CallbackQuery, state: FSMContext):
    user_data = await state.get_data()
    del_msgs_onboarding = user_data.get("del_msgs_onboarding")

    for msg in del_msgs_onboarding:
        await callback_query.message.bot.delete_message(callback_query.message.chat.id, msg)

    if user_data.get('user_id'):
        await state.set_state(LKStates.MAIN_MENU)
        await show_main_menu(callback_query.message, state)
    else:
        await callback_query.message.answer(Registration.no())
