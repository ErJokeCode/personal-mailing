import asyncio
from logging import Logger
import aiohttp
import requests
from schemas import FAQ, FAQTopic, OnboardCourse
            
            
class ManegerOnboarding():
    def __init__(self):
        self.__data: list[OnboardCourse] = []
        self.__index_is_main = None

    
    @property
    def onboarding(self) -> OnboardCourse:
        index = self.get_index_onboarding()
        return self.__data[index]
    
    def is_active_main(self) -> bool:
        return self.__index_is_main != None

    def is_active_add_course(self) -> bool:
        if len(self.__data) > 0 and self.__index_is_main == None or len(self.__data) > 1 and self.__index_is_main != None:
            return True
        return False
    

    def get_additional_courses(self) -> list[tuple[int, str, int]]:
        list = []
        for i in range(len(self.__data)):
            if i != self.__index_is_main:
                list.append((i, self.__data[i].name, len(self.__data[i].sections)))
        return list
    

    def get_index_onboarding(self) -> int:
        return self.__index_is_main

    def get_info_course(self, index:int) -> OnboardCourse:
        return self.__data[index]

    def get_info_course_topic(self, index, callback_data_topic) -> dict | None:
        course = self.get_info_course(index)
        split_callback = callback_data_topic.split("__")
        index = int(split_callback[-1])
        callback_data_section = "__".join(split_callback[:-1])

        for section in course.sections:
            if section.callback_data == callback_data_section:
                return section.topics[index]
        return None
    
    def update_data(self, data: list[dict]):
        self.__data: list[OnboardCourse] = []
        i = 0
        for course in data:
            course_onboard = OnboardCourse(**course)
            if course_onboard.is_main == True:
                self.__index_is_main = i
            self.__data.append(course_onboard)
            i += 1
        
        
class ManagerFaq():
    def __init__(self):
        self.__data = []
        self.__list_topics = []
        
    def update_data(self, data: list[dict]):
        self.__data = data
        
    def update_list_topics(self, data: list[dict]):
        self.__list_topics = []
        for topic in data:
            topic = FAQTopic(**topic)
            self.__list_topics.append(topic)
        
    def get_all(self, callback_data: str) -> list[FAQ]:
        return [FAQ(**i) for i in self.__data if i["callback_data"] == callback_data]
    
    def get_list_topics(self) -> list[FAQTopic]:
        return self.__list_topics
    


class Worker:
    def __init__(self, time_sleep, logger: Logger, cookie: str):
        self.__time_sleep = time_sleep
        self.logger = logger
        self.manager_onboarding = None
        self.cookie = cookie
        
    def add_manager_onboarding(self, manager: ManegerOnboarding, url_update: str):
        self.manager_onboarding = manager
        self.url_update_onb = url_update
        
    def add_manager_faq(self, manager: ManagerFaq, url_update_data: str, url_update_topic: str):
        self.manager_faq = manager
        self.url_update_faq = url_update_data
        self.url_update_faq_topic = url_update_topic

    async def work(self):
        if self.manager_onboarding == None:
            self.logger.error("Manager not found")
            raise Exception("Manager not found")
        
        while True:
            try:
                await self.__update_onboarding_info()
                await self.__update_faq_info()
                
                await asyncio.sleep(self.__time_sleep)
            except Exception as e:
                self.logger.error(e)
                
                
    async def __update_onboarding_info(self):
        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{self.cookie}"}
            async with session.get(f"{self.url_update_onb}", headers=headers) as response:
                if response.status == 200:
                    data = await response.json()
                    self.manager_onboarding.update_data(data)
                else:
                    self.logger.error(f"Error update data {response.status}")
                    
    
    async def __update_faq_info(self):
        async with aiohttp.ClientSession() as session:
            headers = {"cookie": f"{self.cookie}"}
            async with session.get(f"{self.url_update_faq}", headers=headers) as response:
                if response.status == 200:
                    data = await response.json()
                    self.manager_faq.update_data(data)
                else:
                    self.logger.error(f"Error update faq data {response.status}")
                    
            async with session.get(f"{self.url_update_faq_topic}", headers=headers) as response:
                if response.status == 200:
                    data = await response.json()
                    self.manager_faq.update_list_topics(data)
                else:
                    self.logger.error(f"Error update faq topic data {response.status}")
                        
                