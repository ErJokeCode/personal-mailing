import dotenv
import requests
import os

dotenv.load_dotenv()

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
        url = "http://core:5000/login/"
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
    "name_course": "test", 
    "is_onboarding": True,
    "sections": [
    {
        "name": "test section 1", 
        "callback_data": "test_section_1", 
        "topics": [
        {
            "name": "test topic", 
            "text": "test_text",
            "question": "test question", 
            "answer": "test answer"
        }, 
        {
            "name": "test topic 2", 
            "text": "test_text 2", 
            "question": None, 
            "answer": None
        }, 
        ]
    },
    {
        "name": "test section 2", 
        "callback_data": "test_section_2", 
        "topics": [
        {
            "name": "test 2 topic", 
            "text": "test_text", 
            "question": None, 
            "answer": None
        },
        {
            "name": "test 2 topic 2", 
            "text": "test_text", 
            "question": None, 
            "answer": None
        }
        ]
    },
    ]
},
{
    "name_course": "add", 
    "is_onboarding": False,
    "sections": [
    {
        "name": "add section 1", 
        "callback_data": "add_section_1", 
        "topics": [
        {
            "name": "add topic", 
            "text": "add text",
            "question": "add question", 
            "answer": "add answer"
        }, 
        {
            "name": "add topic 2", 
            "text": "add text", 
            "question": None, 
            "answer": None
        }, 
        ]
    },
    {
        "name": "add section 2", 
        "callback_data": "add_section_2", 
        "topics": [
        {
            "name": "add 2 topic", 
            "text": "add text", 
            "question": None, 
            "answer": None
        },
        {
            "name": "add 2 topic 2", 
            "text": "add text", 
            "question": None, 
            "answer": None
        }
        ]
    },
    ]
} 
]


class ManegerOnboarding():
    def __init__(self, data: list[dict]):
        self.__data = data

        if len(data) > 1:
            self.__is_active_add_course = True
        else:
            self.__is_active_add_course = False

    
    @property
    def onboarding(self) -> dict:
        return self.__data[self.get_index_onboarding()]
    

    def is_active_add_course(self) -> bool:
        return self.__is_active_add_course
    

    def get_additional_courses(self) -> list[tuple[int, str, int]]:
        list = []
        for i in range(len(self.__data)):
            if i != 0:
                list.append((i, self.__data[i]["name_course"], len(self.__data[i]["sections"])))
        return list
    

    def get_index_onboarding(self) -> int:
        for i in range(len(self.__data)):
            if self.__data[i]["is_onboarding"]:
                return i
    

    def get_info_course(self, index:int) -> dict:
        return self.__data[index]
    

    def get_info_course_topic(self, index, callback_data_topic) -> dict | None:
        course = self.get_info_course(index)
        split_callback = callback_data_topic.split("__")
        index = int(split_callback[-1])
        callback_data_section = "__".join(split_callback[:-1])

        for section in course["sections"]:
            if section["callback_data"] == callback_data_section:
                return section["topics"][index]
        return None
    

MANAGER_ONB = ManegerOnboarding(test_data_course)
