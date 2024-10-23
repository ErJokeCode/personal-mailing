from pymongo import MongoClient


class Database:
    def __init__(self, host: str, port: int, database: str, user_course_collection: str, course_info_collection: str, 
                student_collection: str, user_auth_collection: str, student_not_found_modeus: str, subject : str):
        self.host = host
        self.port = port
        self.database = database
        client = MongoClient(f'mongodb://{self.host}:{self.port}/')
        self.db = client[self.database]

        self.course_info_collection = course_info_collection
        self.subject = subject

        self.user_course_collection = user_course_collection
        self.student_collection = student_collection
        self.user_auth_collection = user_auth_collection
        self.student_not_found_modeus = student_not_found_modeus

    def get_course_info_collection(self):
        return self.db[self.course_info_collection]
    
    def get_subject(self):
        return self.db[self.subject]

    def get_student(self):
        return self.db[self.student_collection]




    def get_user_course_collection(self):
        return self.db[self.user_course_collection]
    
    def get_user_auth_collection(self):
        return self.db[self.user_auth_collection]
    
    def get_student_not_found_modeus(self):
        return self.db[self.student_not_found_modeus]
    


