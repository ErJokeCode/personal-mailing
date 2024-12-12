from pymongo import MongoClient
from pymongo.collection import Collection


class Database:
    def __init__(self, host: str, port: int, database: str, course_info_collection: str, 
                student_collection: str, student_not_found_modeus: str, subject : str, dict_names:str, bot_onboard: str):
        self.host = host
        self.port = port
        self.database = database
        client = MongoClient(f'mongodb://{self.host}:{self.port}/')
        self.db = client[self.database]

        self.course_info_collection = course_info_collection
        self.subject = subject
        self.student_collection = student_collection
        self.dict_names = dict_names
        self.student_not_found_modeus = student_not_found_modeus
        self.bot_onboard = bot_onboard


        

    def get_course_info_collection(self) -> Collection:
        return self.db[self.course_info_collection]
    
    def get_subject(self) -> Collection:
        return self.db[self.subject]

    def get_student(self) -> Collection:
        return self.db[self.student_collection]
    
    def get_dict_names(self) -> Collection:
        return self.db[self.dict_names]
    
    def get_student_not_found_modeus(self) -> Collection:
        return self.db[self.student_not_found_modeus]
    
    def get_bot_onboard(self) -> Collection:
        return self.db[self.bot_onboard]

    

    


