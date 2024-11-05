from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

def choice() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Продолжить", callback_data="start_onboarding")], 
            [InlineKeyboardButton(text="Пропустить", callback_data="skip_onboarding")]
        ])

def one() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Ответ", callback_data="two")], 
        ])

def end() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="До связи!", callback_data="end_onboarding")], 
        ])