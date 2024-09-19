from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton, WebAppInfo
from aiogram.utils.keyboard import InlineKeyboardBuilder
from aiogram.types import ReplyKeyboardRemove, ReplyKeyboardMarkup, KeyboardButton


def welcome_kb():
   kb = [
       [
           KeyboardButton(text="Информация об онлайн курсе"),
           KeyboardButton(text="Баллы за онлайн курсы")
       ],
   ]
   return ReplyKeyboardMarkup(keyboard=kb, resize_keyboard=True)

def university_kb(universitys):
    inline_kb_list = [[InlineKeyboardButton(text = university, callback_data=university)] for university in universitys]
    return InlineKeyboardMarkup(inline_keyboard=inline_kb_list)

def courses_kb(courses, university):
    inline_kb_list = [[InlineKeyboardButton(text = courses[i]["name"], callback_data="course " + university + " " + str(i))] for i in range(len(courses))]
    return InlineKeyboardMarkup(inline_keyboard=inline_kb_list)