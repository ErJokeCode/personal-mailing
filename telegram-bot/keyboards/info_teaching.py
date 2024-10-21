from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton

def next_and_back(next = True):
    if next:
        return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Следующий раздел", callback_data="next_topic")],
            [InlineKeyboardButton(text="Темы", callback_data="back_to_topics")],
        ])
    else:
        return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="Темы", callback_data="back_to_topics")],
        ])

def list_topics(user_data: dict[str, any]):

    return InlineKeyboardMarkup(inline_keyboard=[
            [InlineKeyboardButton(text="✅ Студенчекские, карты, пропуска" if user_data.get("cards") else "❌ Студенчекские, карты, пропуска", callback_data="cards")],
            [InlineKeyboardButton(text="✅ Расшифровка номера аудитории" if user_data.get("auditoria") else "❌ Расшифровка номера аудитории", callback_data="auditoria")],
            [InlineKeyboardButton(text="✅ Навигатор УРФУ" if user_data.get("navigator") else "❌ Навигатор УРФУ", callback_data="navigator")],
            [InlineKeyboardButton(text="✅ ЛК УРФУ" if user_data.get("lk_student") else "❌ ЛК УРФУ", callback_data="lk_student")],
            [InlineKeyboardButton(text="✅ Онлайн курсы" if user_data.get("online_course_all_info") else "❌ Онлайн курсы", callback_data="online_course_all_info")],
            [InlineKeyboardButton(text="✅ Рейтинги" if user_data.get("rating") else "❌ Рейтинги", callback_data="rating")],
            [InlineKeyboardButton(text="Главное меню", callback_data="main_menu")]
        ])