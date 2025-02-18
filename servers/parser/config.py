import enum
import dotenv
import os

import requests

from db import MongoDataBase, S3Client, WorkerCollection
from src.schemas import FAQ, DictNames, DictNamesInDB, FAQTopic, FAQTopicInDB, HistoryUploadFile, HistoryUploadFileInDB, InfoOCInFile, InfoOCInFileInDB, OnboardCourse, OnboardCourseInDB, Student, StudentInDB, Subject, SubjectInDB, InfoOnlineCourse, InfoOnlineCourseInDB


class WorkerDataBase(MongoDataBase):
    def __init__(self, host: str, port: int, name_db: str):
        super().__init__(host, port, name_db)
        
        self.history = WorkerCollection[HistoryUploadFile, HistoryUploadFileInDB](self.db["history"], HistoryUploadFile, HistoryUploadFileInDB)
        self.student = WorkerCollection[Student, StudentInDB](self.db["student"], Student, StudentInDB)
        self.subject = WorkerCollection[Subject, SubjectInDB](self.db["subject"], Subject, SubjectInDB)
        self.info_online_course = WorkerCollection[InfoOnlineCourse, InfoOnlineCourseInDB](self.db["info_online_course"], InfoOnlineCourse, InfoOnlineCourseInDB)
        self.dict_names = WorkerCollection[DictNames, DictNamesInDB](self.db["dict_names"], DictNames, DictNamesInDB)
        self.bot_faq = WorkerCollection[FAQTopic, FAQTopicInDB](self.db["bot_faq"], FAQTopic, FAQTopicInDB)
        self.bot_onboard = WorkerCollection[OnboardCourse, OnboardCourseInDB](self.db["bot_onboard"], OnboardCourse, OnboardCourseInDB)
        self.onl_cr_in_file = WorkerCollection[InfoOCInFile, InfoOCInFileInDB](self.db["onl_cr_in_file"], InfoOCInFile, InfoOCInFileInDB)

dotenv.load_dotenv()

MGO_HOST = os.getenv('MGO_HOST')
MGO_PORT = os.getenv('MGO_PORT')
MGO_NAME_DB = os.getenv('MGO_NAME_DB')

URL_S3 = os.getenv('URL_S3')
REGION_S3 = os.getenv('REGION_S3')
AWS_ACCESS_KEY_ID = os.getenv('AWS_ACCESS_KEY_ID')
AWS_SECRET_ACCESS_KEY = os.getenv('AWS_SECRET_ACCESS_KEY')
BUCKET_NAME = os.getenv('BUCKET_NAME')
URL_S3_GET = os.getenv('URL_S3_GET')

URL_CORE = os.getenv('URL_CORE')
SECRET_TOKEN = os.getenv('SECRET_TOKEN')

worker_db = WorkerDataBase(
    host=MGO_HOST, 
    port=int(MGO_PORT), 
    name_db=MGO_NAME_DB)


s3_client = S3Client(
        access_key=AWS_ACCESS_KEY_ID,
        secret_key=AWS_SECRET_ACCESS_KEY,
        endpoint_url=URL_S3, 
        bucket_name=BUCKET_NAME,
        url_files=URL_S3_GET
    )
