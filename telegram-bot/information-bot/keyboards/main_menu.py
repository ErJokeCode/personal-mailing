from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

def menu() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Вводный курс", callback_data="start_onboarding")],
            [InlineKeyboardButton(text="Информация об обучении", callback_data="info_teaching")],
            [InlineKeyboardButton(text="Предметы", callback_data="subjects")],
            [InlineKeyboardButton(text="Онлайн курсы", callback_data="online_courses")],
            [InlineKeyboardButton(text="Чат с куратором", url="https://t.me/RespectStoreBot")],
            [InlineKeyboardButton(text="FAQ", callback_data="faq")],
        ])

def back_to_main() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Назад", callback_data="main_menu")]
        ])

def courses(courses_data) -> InlineKeyboardMarkup:
    inline_keyboard=[
            [InlineKeyboardButton(text=courses_data[i]["name"], callback_data="course_" + str(i))] for i in range(len(courses_data))
        ]
    inline_keyboard.append([InlineKeyboardButton(text="Назад", callback_data="main_menu")])
    keyboard = InlineKeyboardMarkup(inline_keyboard=inline_keyboard)
    return keyboard

def FAQ() -> InlineKeyboardMarkup:
    #Создание разделов вопросов
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Онлайн курсы", callback_data="faq_online_course")],
            [InlineKeyboardButton(text="ВУЦ", callback_data="faq_vuc")],
            [InlineKeyboardButton(text="Назад", callback_data="main_menu")]
        ])

def back_to_FAQ() -> InlineKeyboardMarkup:
    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Назад", callback_data="faq")], 
            [InlineKeyboardButton(text="Главное меню", callback_data="main_menu")]
        ])

def back_to_courses() -> InlineKeyboardMarkup:
    keyboard = InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Назад", callback_data="online_courses")],
            [InlineKeyboardButton(text="Главное меню", callback_data="main_menu")]
        ])
    return keyboard