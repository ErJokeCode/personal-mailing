from pymongo import MongoClient


class Database:
    def __init__(self, host: str, port: int, database: str, user_course_collection: str, course_info_collection: str, 
                user_collection: str, user_auth_collection: str):
        self.host = host
        self.port = port
        self.database = database
        client = MongoClient(f'mongodb://{self.host}:{self.port}/')
        self.db = client[self.database]

        self.user_course_collection = user_course_collection
        self.course_info_collection = course_info_collection
        self.user_collection = user_collection
        self.user_auth_collection = user_auth_collection

    def get_user_course_collection(self):
        return self.db[self.user_course_collection]
    
    def get_course_info_collection(self):
        return self.db[self.course_info_collection]
    
    def get_user_collection(self):
        return self.db[self.user_collection]
    
    def get_user_auth_collection(self):
        return self.db[self.user_auth_collection]
    


