from contextlib import asynccontextmanager
from typing import AsyncGenerator, List, ParamSpec, Type, TypeVar, Generic, Sequence
from aiohttp import ClientError
from bson import ObjectId
from fastapi import HTTPException, UploadFile, status
from typing_extensions import Unpack, TypedDict
from pydantic import BaseModel
from pymongo import InsertOne, MongoClient
from passlib.context import CryptContext
from pymongo.collection import Collection
from pymongo import database, UpdateOne

from datetime import datetime
import hashlib

from aiobotocore.session import get_session
from aiobotocore.client import AioBaseClient

from src.schemas import BaseModelInDB, HistoryUploadFile, HistoryUploadFileInDB, TypeFile


V = TypeVar("V", bound=BaseModel)
T = TypeVar("T", bound=BaseModelInDB)

class WorkerCollection(Generic[V, T]):
    def __init__(self, collection: Collection, cls: V, cls_db: T) -> None:
        self.__collection = collection
        self.__cls = cls
        self.__cls_db = cls_db

    def get_collect(self) -> Collection:
        return self.__collection

    def get_one(self, get_none: bool = False, find_dict: dict = None, **kwargs: Unpack[T]) -> T | None:
        
        last_key = None
        for key in kwargs.keys():
            if "___" in key:
                last_key = key
                break
            
        if last_key != None:
            value = kwargs[last_key]
            kwargs[last_key.replace("___", ".")] = value
            del kwargs[last_key]
            
        if kwargs.get("id") != None:
            kwargs = {"_id": ObjectId(kwargs["id"])}
            
        if find_dict != None: 
            kwargs = {**kwargs, **find_dict}
            
        item = self.__collection.find_one(kwargs)
        if get_none == False and item == None:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Item not found")
        
        if item == None:
            return None
        return self.__cls_db(**item)


    def get_all(self, limit: int = None, dict_find: dict = None, **kwargs) -> list[T]:
        if dict_find != None:
            kwargs = {**kwargs, **dict_find}
        if limit == None:
            limit = 10
        i = 1
        items = []
        for item in self.__collection.find(kwargs):
            if i <= limit or limit == -1:
                i+=1
                items.append(self.__cls_db(**item))
            else:
                break
        return items


    def insert_one(self, item: V) -> T:
        item_id = self.__collection.insert_one(item.model_dump()).inserted_id
        item = self.__collection.find_one({"_id": ObjectId(item_id)})
        return self.__cls_db(**item)
    
    def insert_many(self, items: Sequence[V]) -> dict[str, str]:
        items = [item.model_dump() for item in items]
        self.__collection.insert_many(items)
        return {"status": "success"}
    
    def update_one(self, item: T | V | None = None, upsert: bool = False, dict_keys: dict = None, update_data: dict = None, get_item: bool = True, **keys_find) -> T | None:
        if item != None:
            update_data = {"$set": item.model_dump(by_alias=True, exclude="_id")}
        elif item == None and update_data == None:
            raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="Bad request no data update")
            
        keys = {} 
        if isinstance(item, self.__cls_db):
            keys = {"_id": ObjectId(item.id)}
        elif isinstance(item, self.__cls) or item == None:
            if keys_find != {}:
                keys = keys_find
            elif dict_keys != None:
                keys = dict_keys
            else:
                self.__collection.insert_one(item.model_dump())
                item = self.__collection.find_one(keys)
                return self.__cls_db(**item)
        else:
            raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="Bad request for update item")
        
        res = self.__collection.update_one(keys, update_data, upsert=upsert)
        
        if get_item:
            item = self.__collection.find_one(keys)
            return self.__cls_db(**item)
        else:
            return None
        
    def bulk_update(self, filter: list[str], update_filter: list[str], data: Sequence[T | V], upsert: bool = False) -> dict[str, str]:
        
        def create_filter(filter: list[str], item) -> dict[str, str]:
            dict = {}
            for fl in filter:
                dict[fl] = get_value(fl, item)
            return dict
        
        def get_value(filter: str, item):
            try:
                sp_fl = filter.split(".")
                if len(sp_fl) == 1:
                    return item[filter]
                else:
                    return get_value(sp_fl[1], item[sp_fl[0]])
            except Exception as e:
                print(e)
        
        collect = self.__collection
        ids = collect.find().distinct(filter[0])
        
        operations = []
        for item in data:
            dict_item = item.model_dump()
            
            if dict_item[filter[0]] in ids:
                fl = create_filter(filter, dict_item)
                
                up_fl = create_filter(update_filter, dict_item)
                
                operations.append(UpdateOne(fl, {"$set": up_fl}, upsert=upsert))
            else:
                operations.append(InsertOne(dict_item))
        
        collect.bulk_write(operations)
        
        return {"status": "success"}
    
    def delete_one(self, id: str) -> dict[str, str]:
        res = self.__collection.delete_one({"_id": ObjectId(id)})
        if res.deleted_count == 0:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Item not found")
        return {"satatus": "success"}
    
    def delete_many(self, **kwargs) -> dict[str, str]:
        res = self.__collection.delete_many(kwargs)
        return {"satatus": "success"}

class MongoDataBase():
    def __init__(self, host: str, port: int, name_db: str):
        self.__host = host
        self.__port = port
        self.__name_db = name_db
        client = MongoClient(f'mongodb://{self.__host}:{self.__port}/')
        self.__db = client[self.__name_db]
        
    @property
    def db(self) -> database.Database:
        return self.__db
       

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
            raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error uploading file to S3 storage: {e}")

    async def delete_file(self, object_name: str):
        try:
            async with self.get_client() as client:
                await client.delete_object(Bucket=self.__bucket_name, Key=object_name)
        except ClientError as e:
            raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error deleting file from S3: {e}")
            
    async def get_file_read(self, key: str):
        try:
            async with self.get_client() as client:
                response = await client.get_object(Bucket=self.__bucket_name, Key=key)
                data = await response["Body"].read()
                return data
        except ClientError as e:
            raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error downloading file from S3: {e}")
            
    async def create_hist_file(self, file: UploadFile, type: TypeFile, is_upload: bool = True) -> HistoryUploadFile:
        time = datetime.now()
        hash = hashlib.sha256(file.filename.encode("utf-8") + str(time).encode("utf-8")).hexdigest()
        key = hash + "." + file.filename.split(".")[-1]
        if is_upload:
            link = await self.upload_file(file, key)
        else:
            link = None
        
        hist_file = HistoryUploadFile(
            name_file=file.filename,
            key=key,
            date=time,
            type=type,
            link=link
        )
        
        return hist_file

    