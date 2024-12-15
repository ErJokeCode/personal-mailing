import asyncio
from logging import Logger
import aiohttp
import requests
            
            
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
                list.append((i, self.__data[i]["name"], len(self.__data[i]["sections"])))
        return list
    

    def get_index_onboarding(self) -> int:
        return 0
    

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
    
    def update_data(self, data: list[dict]):
        self.__data = data


class Worker:
    def __init__(self, time_sleep, logger: Logger):
        self.__time_sleep = time_sleep
        self.logger = logger
        self.manager = None
        
    def add_manager_onboarding(self, manager: ManegerOnboarding, url_update: str, cookie: str):
        self.manager = manager
        self.url_update = url_update
        self.cookie = cookie

    async def work(self):
        if self.manager == None:
            self.logger.error("Manager not found")
            raise Exception("Manager not found")
        
        while True:
            try:
                async with aiohttp.ClientSession() as session:
                    headers = {"cookie": f"{self.cookie}"}
                    async with session.get(f"{self.url_update}", headers=headers) as response:
                        if response.status == 200:
                            data = await response.json()
                            self.manager.update_data(data)
                            self.logger.info(f"Update data onboarding in worker")
                        else:
                            self.logger.error(f"Error update data {response.status}")
                        
                        await asyncio.sleep(self.__time_sleep)
            except Exception as e:
                self.logger.error(e)