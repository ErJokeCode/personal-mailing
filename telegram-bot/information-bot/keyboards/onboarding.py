from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

def start_choice() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Да, расскажи", callback_data="next_onb")], 
            [InlineKeyboardButton(text="Нее, давай позже", callback_data="end_onboarding")]
        ])

def to_main_menu() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Главное меню", callback_data="next_onb")], 
        ])

def next() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Продолжить", callback_data="next_onb")], 
        ])

def yes_no_help() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Да, все получилось", callback_data="next_onb")], 
            [InlineKeyboardButton(text="Нет, помоги", callback_data="no_onb")]
        ])

def worked_and_chat_curator() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Да, все получилось", callback_data="next_onb")], 
            [InlineKeyboardButton(text="Чат с куратором", url="https://t.me/RespectStoreBot")]
        ])

def que_iot(iot_disc:bool, iot_choice:bool) -> InlineKeyboardMarkup:
    inline_keyboard = []
    if iot_disc:
        inline_keyboard.append([InlineKeyboardButton(text="Отличие ядерных и неядерных дисциплин", callback_data="iot_disc")])
    if iot_choice:
        inline_keyboard.append([InlineKeyboardButton(text="Выбор дисциплин в Модеусе", callback_data="iot_choice")])
    inline_keyboard.append([InlineKeyboardButton(text="Нет, давай дальше", callback_data="next_onb")])
    return InlineKeyboardMarkup(inline_keyboard=inline_keyboard)

def que_exam() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Как проходят онлайн экзамены?", callback_data="info_exam")], 
            [InlineKeyboardButton(text="Нее, давай дальше", callback_data="next_onb")]
        ])

def que_rating() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Научный рейтинг", callback_data="science_rating")], 
            [InlineKeyboardButton(text="Внеучебный рейтинг", callback_data="extr_rating")],
            [InlineKeyboardButton(text="Общий рейтинг и что он дает", callback_data="all_rating")],
            [InlineKeyboardButton(text="Нее, давай дальше", callback_data="next_onb")]
        ])

def back_to_rating() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Назад", callback_data="back_rating")], 
    ])

def yes_no() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Да", callback_data="next_onb")], 
            [InlineKeyboardButton(text="Нет", callback_data="no_onb")]
        ])

def end() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="До связи!", callback_data="end_onboarding")], 
        ])