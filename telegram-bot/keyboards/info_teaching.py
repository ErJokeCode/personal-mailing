from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

def next_and_back():
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Продолжить", callback_data="next")],
            [InlineKeyboardButton(text="Назад", callback_data="back")],
        ])

def yes_and_no():
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Да", callback_data="yes")],
            [InlineKeyboardButton(text="Нет", callback_data="no")],
        ])