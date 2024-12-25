from contextlib import asynccontextmanager
from typing import AsyncGenerator
from aiohttp import ClientError
from fastapi import UploadFile
from pymongo import MongoClient
from pymongo.collection import Collection

from aiobotocore.session import get_session
from aiobotocore.client import AioBaseClient


class Database:
    def __init__(self, host: str, port: int, database: str, 
                 course_info_collection: str, 
                 student_collection: str, 
                 student_not_found_modeus: str, 
                 subject : str, 
                 dict_names:str, 
                 bot_onboard: str, 
                 bot_faq: str, 
                 history_upload: str):
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
        self.bot_faq = bot_faq
        self.history_upload = history_upload
        

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
    
    
    def get_bot_faq(self) -> Collection:
        return self.db[self.bot_faq]

    
    def get_history_upload(self) -> Collection:
        return self.db[self.history_upload]
    
    


class S3Client:
    def __init__(
            self,
            access_key: str,
            secret_key: str,
            endpoint_url: str,
            bucket_name: str,
            url_files: str
    ):
        self.__config = {
            "aws_access_key_id": access_key,
            "aws_secret_access_key": secret_key,
            "endpoint_url": endpoint_url,
        }
        self.__bucket_name = bucket_name
        self.__session = get_session()
        self.__url_files = url_files
    
    @asynccontextmanager
    async def get_client(self) -> AsyncGenerator[AioBaseClient]:
        async with self.__session.create_client("s3", **self.__config) as client:
            yield client
            
    async def upload_file(
            self,
            file: UploadFile,
            key: str
    ):
        try:
            async with self.get_client() as client:
                resp = await client.put_object(
                    Bucket=self.__bucket_name,
                    Key=key,    
                    Body=file.file,
                )
                return self.__url_files + "/" + key
        except ClientError as e:
            print(f"Error uploading file: {e}")

    async def delete_file(self, object_name: str):
        try:
            async with self.get_client() as client:
                await client.delete_object(Bucket=self.__bucket_name, Key=object_name)
        except ClientError as e:
            print(f"Error deleting file: {e}")
            
    async def get_file_read(self, key: str):
        try:
            async with self.get_client() as client:
                response = await client.get_object(Bucket=self.__bucket_name, Key=key)
                data = await response["Body"].read()
                return data
        except ClientError as e:
            print(f"Error downloading file: {e}")
    

    


