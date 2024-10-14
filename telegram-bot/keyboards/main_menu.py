from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

def Menu() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Информация об обучении", callback_data="info_teaching")],
            [InlineKeyboardButton(text="Архив уведомлений", callback_data="notif")],
            [InlineKeyboardButton(text="Онлайн курсы", callback_data="online_courses")],
            [InlineKeyboardButton(text="Чат с куратором", callback_data="chat_curator")],
            [InlineKeyboardButton(text="FAQ", callback_data="faq")],
            # [InlineKeyboardButton(text="Выйти из аккаунта", callback_data="logout")]
        ])

def Back_to_main() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Назад", callback_data="main_menu")]
        ])

def Courses(courses_data) -> InlineKeyboardMarkup:
    inline_keyboard=[
            [InlineKeyboardButton(text=courses_data[i]["name"], callback_data="course_" + str(i))] for i in range(len(courses_data))
        ]
    inline_keyboard.append([InlineKeyboardButton(text="Назад", callback_data="main_menu")])
    keyboard = InlineKeyboardMarkup(inline_keyboard=inline_keyboard)
    return keyboard

def Back_to_courses() -> InlineKeyboardMarkup:
    keyboard = InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Назад", callback_data="online_courses")],
            [InlineKeyboardButton(text="Главное меню", callback_data="main_menu")]
        ])
    return keyboard