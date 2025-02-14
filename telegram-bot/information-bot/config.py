import dotenv
import requests
import os
import logging

from worker import Worker, ManegerOnboarding, ManagerFaq

dotenv.load_dotenv()

logging.basicConfig(filename='bot_log.log', format='%(asctime)s - %(name)s - %(levelname)s - %(message)s', level=logging.INFO)

LOGGER = logging.getLogger(__name__)

TOKEN = os.getenv("TOKEN")
TOKEN_CHAT_CURATOR = os.getenv("TOKEN_CHAT_CURATOR")
URL_SERVER = os.getenv("URL_SERVER")
URL_REDIS = os.getenv("URL_REDIS")
NGROK_TUNNEL_URL = os.getenv("NGROK_TUNNEL_URL")
URL_BOT_CHAT_CURATOR = os.getenv("URL_BOT_CHAT_CURATOR")

COOKIE = ""


def get_cookie():
    global COOKIE

    if COOKIE == "":
        url = "http://core:5000/core/admins/login/"
        body = {
            "email": "admin",
            "password": "admin",
        }

        session = requests.Session()
        response = session.post(url, json=body)
        name = ".AspNetCore.Identity.Application"
        COOKIE = name + "=" + response.cookies.get_dict()[name]

    return COOKIE



test_data_course = [{
    "name": "Вводный курс", 
    "sections": [
    {
        "name": "ЛК УРФУ", 
        "callback_data": "lk_urfu", 
        "topics": [
        {
            "name": "Основная информация", 
            "text": "К УрФУ - это твой главный помощник в университетской среде. Здесь собрана вся информация об успеваемости(БРС) и внеучебной деятельности, доступны десятки полезных сервисов.",
            "question": None, 
            "answer": None
        }, 
        {
            "name": "Создание учетной записи", 
            "text": "Перейди по сслылке: https://urfu.ru/ru\n\nЧтобы создать учетную запись, тебе нужно кликнуть по красной кнопке “Личный кабинет” в правой верхней части экрана.\n\nЧтобы создать учетную запись, тебе нужно кликнуть по красной кнопке “Личный кабинет” в правой верхней части экрана.\n\nИз раскрывающегося перечня выбери “Абитуриенту” и кликни “Регистрация”.\n\nЗаполни предложенную электронную форму, указав:\nEmail или номер телефона.\nУровень поступления.\nПароль (дважды для подтверждения).\nПоставь галочку, подтверждая согласие на обработку данных.\nЗатем нажми кнопку “подтвердить”", 
            "question": "Получилось зарагестироваться?", 
            "answer": "Если у тебя не получилось перейти по ссылке, открой браузер, установленный на устройстве, введи в поисковой строке “УрФУ”, перейди по первой ссылке — и ты попадешь на сайт."
        }, 
        {
            "name": "Получение доступа к ЛК", 
            "text": "Получить доступ к ЛК студента ты сможешь, когда будут изданы приказы ” О зачислении в ВУЗ” и “О зачислении в академическую группу”. Персональный аккаунт регистрируется в течение суток после вступления приказа в силу.",
            "question": None, 
            "answer": None
        }, 
        {
            "name": "БРС", 
            "text": "БРС (Бально-рейтинговая система) - это система оценки успеваемости студентов, используемая для учета учебных достижений. Она позволяет отслеживать текущие оценки, результаты зачетов, экзаменов и других форм контроля.\n\nЗайди в ЛК УрФУ и посмотри свои баллы в БРС.  https://istudent.urfu.ru   БРС находится в разделе “Учеба”  на главной странице ЛК.",
            "question": "Ты смог посмотреть свои баллы?", 
            "answer": "1. Попробуй ещё раз заглянуть в ЛК УрФУ и проверь, правильно ли ты вошёл.\n\n2. Попробуй перейти по ссылке  https://istudent.urfu.ru/s/http-urfu-ru-ru-students-study-brs\n\n3. Радиогид предлагает помощь: “Подробная инструкция по входу в БРС есть по ссылке: chrome-extension://efaidnbmnnnibpcajpcglclefindmkaj/https://urfu.ru/fileadmin/user_upload/urfu.ru/documents/brs/Instrukcija_studentam.pdf”"
        }, 
        {
            "name": "Конец раздела", 
            "text": "Отлично! \n\n🎉Теперь ты умеешь пользоваться ЛК УрФУ. 🖥️Обрати внимание, что помимо БРС, в ЛК есть много других возможностей, которые ты можешь изучить самостоятельно:\n📚 Расписание занятий\n📋 Документы и справки\n📨 Обратная связь с преподавателями\n\nИсследуй сам — уверен, ты найдёшь много полезного! 🚀",
            "question": None, 
            "answer": None
        }, 
        ]
    },
    {
        "name": "Modeus", 
        "callback_data": "modeus", 
        "topics": [
        {
            "name": "Основная информация", 
            "text": "Модеус - это платформа, на которой студенты смотрят свое расписание, а также выбирают ИОТ. Про то, что такое ИОТ, мы поговорим позже.\n\nВ Модеусе выборы проходят каждые полгода. Мы обязательно напомним, когда начнется следующий этап.", 
            "question": None, 
            "answer": None
        },
        {
            "name": "Выбор предметов", 
            "text": "Выбор предметов начинается с первого курса и осуществляется на платформе Modeus. На первом семестре первого курса все студенты находятся в равных условиях  выбора. Начиная со второго курса, выбор дисциплин проходит в порядке волн. Волна студента определяется на основании его среднего балла за семестр: чем выше средний балл, тем выше волна. Средний балл высчитывается по принципу среднего арифметическго.", 
            "question": None, 
            "answer": None
        },
        {
            "name": "Расписание", 
            "text": "Загляни на платформу Modeus, найди своё расписание и возвращайся ко мне. https://urfu.modeus.org\n\nРасписание находится на главной странице в разделе “Моё расписание”", 
            "question": "Ты нашел свое расписание?", 
            "answer": "1. Попробуй ещё раз заглянуть в Modeus и проверь, правильно ли ты вошёл\n\n2. Попробуй перейти по ссылке позже https://urfu.modeus.org."
        },
        {
            "name": "Конец раздела", 
            "text": "Отлично! 🎉 \nТеперь ты умеешь пользоваться Modeus. 📱", 
            "question": None, 
            "answer": None
        }
        ]
    },
    {
        "name": "Занятия", 
        "callback_data": "subjects", 
        "topics": [
        {
            "name": "Куда идти?", 
            "text": "Большинство занятий проходят в очном формате, однако также предлагаются и онлайн-курсы. Очные занятия проходят в различных институтах УрФУ. Студент может нажать на название предмета в Modeus, чтобы узнать номер аудитории, где будет проходить занятие. Например, «Р» в номере аудитории (например, Р-100) означает, что она находится в Радиофаке. Местоположение аудиторий можно найти с помощью навигатора УрФУ на сайте https://how-to-navigate.ru/.",
            "question": None, 
            "answer": None
        },
        {
            "name": "ИОТ", 
            "text": "В ИРИТ-РТФ ты будешь учиться по системе индивидуальных образовательных траекторий (ИОТ). Выбор дисциплин так же будет проходить в Модеусе. \n\nИндивидуальные образовательные траектории  – это подход, при котором студент самостоятельно выбирает предметы, курсы и направления для изучения, исходя из своих потребностей и предпочтений.", 
            "question": None, 
            "answer": None
        },
        {
            "name": "Конец раздела", 
            "text": "Молодец! \n🎉Теперь ты знаешь, что такое ИОТ — индивидуальная образовательная траектория. 🚀Это возможность настроить своё обучение под себя:\nВыбирать интересные курсы 📚\nРазвивать свои сильные стороны 💪\nСоздавать уникальный учебный план 🌟", 
            "question": None, 
            "answer": None
        }
        ]
    },
    {
        "name": "Онлайн курсы", 
        "callback_data": "online_courses", 
        "topics": [
        {
            "name": "Что это?", 
            "text": "Онлайн-курсы  — это образовательные программы, доступные в цифровом формате, позволяющие обучаться дистанционно. Они охватывают широкий спектр дисциплин, включая инженерные науки, гуманитарные направления.",
            "question": None, 
            "answer": None
        },
        {
            "name": "Как войти?", 
            "text": "Чтобы приступить к обучению на онлайн курсах, тебе необходимо завести корпоративный почтовый ящик в домене @urfu.me.\n\nОбрати внимание, что ivan.ivanov@at.urfu.ru — не является почтовым адресом.\n\nЛогин @at.urfu.ru: Этот логин применяется для авторизации в корпоративных сервисах университета, таких как система Office365 или личный кабинет студента.\n\nПочтовый адрес @urfu.me: После активации этого адреса на него будут поступать важные уведомления, связанные с обучением на онлайн-курсах.", 
            "question": None, 
            "answer": None
        }
        ]
    },
    ]
}
]


MANAGER_ONB = ManegerOnboarding()
MANAGER_FAQ = ManagerFaq()

WORKER = Worker(10, LOGGER, get_cookie())
WORKER.add_manager_onboarding(MANAGER_ONB, URL_SERVER + "/parser/bot/onboard/")
WORKER.add_manager_faq(MANAGER_FAQ, URL_SERVER + "/parser/bot/faq/")