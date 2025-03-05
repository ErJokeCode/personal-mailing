import logging
import sys
from typing import Generic, Sequence, TypeVar, Callable
from bson import ObjectId
from pymongo import InsertOne, MongoClient
from fastapi import HTTPException, status

from pydantic import BaseModel
from pymongo import database, UpdateOne
from pymongo.collection import Collection

from models.base.base import BaseModelInDB, EBaseModel
from models.student.db_student import Student, StudentInDB
from models.upload_files.resp_upload import InDB
from models.file.stucture import StuctureExcel, StuctureExcelInDB

_log = logging.getLogger(__name__)


V = TypeVar("V", bound=EBaseModel)
T = TypeVar("T", bound=BaseModelInDB)


class ECollection(Generic[V, T]):
    def __init__(self, collection: Collection, cls: Callable[[], V], cls_db: Callable[[], T]) -> None:
        self.__collection = collection
        self.__cls = cls
        self.__cls_db = cls_db

    def get_collect(self) -> Collection:
        return self.__collection

    def get_one(self, get_none: bool = False, find_dict: dict | None = None, **kwargs) -> T | None:

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
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND, detail="Item not found")

        if item == None:
            return None

        return self.__cls_db(**item)

    def get_all(self, limit: int | None = None, dict_find: dict | None = None, **kwargs) -> list[T]:
        if dict_find != None:
            kwargs = {**kwargs, **dict_find}
        if limit == None:
            limit = 10
        i = 1
        items = []
        for item in self.__collection.find(kwargs):
            if i <= limit or limit == -1:
                i += 1
                items.append(self.__cls_db(**item))
            else:
                break
        return items

    def insert_one(self, item: V) -> T:
        item_id = self.__collection.insert_one(item.model_dump()).inserted_id
        item_db = self.__collection.find_one({"_id": ObjectId(item_id)})
        if item_db == None:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND, detail="Item not found")
        return self.__cls_db(**item_db)

    def insert_many(self, items: Sequence[V]) -> dict[str, str]:
        items_dict = [item.model_dump() for item in items]
        self.__collection.insert_many(items_dict)
        return {"status": "success"}

    def update_one(self, item: T | V | None = None, upsert: bool = False, dict_keys: dict | None = None, update_data: dict | None = None, get_item: bool = True, **keys_find) -> T | None:
        if item != None:
            update_data = {"$set": item.model_dump(
                by_alias=True, exclude="_id")}
        elif item == None and update_data == None:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST, detail="Bad request no data update")

        keys = {}
        if isinstance(item, type(self.__cls_db)):
            keys = {"_id": ObjectId(item.id)}
        elif isinstance(item, type(self.__cls)) or item == None:
            if keys_find != {}:
                keys = keys_find
            elif dict_keys != None:
                keys = dict_keys
            else:
                if item == None:
                    raise HTTPException(
                        status_code=status.HTTP_400_BAD_REQUEST,
                        detail="Bad request for update item",
                    )
                self.__collection.insert_one(item.model_dump())
                item_db = self.__collection.find_one(keys)
                if item_db == None:
                    raise HTTPException(
                        status_code=status.HTTP_404_NOT_FOUND,
                        detail="Item not found",
                    )
                return self.__cls_db(**item_db)
        else:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST, detail="Bad request for update item")

        res = self.__collection.update_one(keys, update_data, upsert=upsert)

        if get_item:
            item_db = self.__collection.find_one(keys)
            if item_db == None:
                raise HTTPException(
                    status_code=status.HTTP_404_NOT_FOUND, detail="Item not found")
            return self.__cls_db(**item_db)
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

        try:
            collect = self.__collection
            ids = collect.find().distinct(filter[0])

            operations: list[UpdateOne | InsertOne] = []
            for item in data:
                dict_item = item.model_dump()

                if dict_item[filter[0]] in ids:
                    fl = create_filter(filter, dict_item)

                    up_fl = create_filter(update_filter, dict_item)

                    operations.append(
                        UpdateOne(fl, {"$set": up_fl}, upsert=upsert))
                else:
                    operations.append(InsertOne(dict_item))

            if len(operations) > 0:
                collect.bulk_write(operations)
        except Exception as e:
            print(e)
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail="Error bulk update")

        return {"status": "success"}

    def delete_one(self, id: str) -> dict[str, str]:
        res = self.__collection.delete_one({"_id": ObjectId(id)})
        if res.deleted_count == 0:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND, detail="Item not found")
        return {"satatus": "success"}

    def delete_many(self, **kwargs) -> dict[str, str]:
        res = self.__collection.delete_many(kwargs)
        return {"satatus": "success"}


class ECollectV2(Generic[V, T]):
    def __init__(self, db_client: database.Database, cls: type[V], cls_db: type[T]) -> None:
        self.__db_client = db_client

        name_collect = cls.__name__[0].lower() + cls.__name__[1:]
        name_collect = "".join(
            [ch if ch.islower() else "_" + ch.lower() for ch in name_collect])
        self.__collect = db_client[name_collect]
        self.__collect_version = db_client[name_collect + "_version"]

        if len(cls.primary_keys()) == 0 or len(cls_db.primary_keys()) == 0:
            _log.error(
                "Method 'primary_keys' not found in class %s or class %s", cls.__name__, cls_db.__name__)
            sys.exit()

        self.__cls = cls
        self.__cls_db = cls_db

    @property
    def collect(self) -> Collection:
        return self.__collect

    @property
    def collect_version(self) -> Collection:
        return self.__collect_version

    def find_one(self, **kwargs) -> T | None:
        item = self.__collect.find_one(kwargs)
        if item == None:
            return None
        return self.__cls_db(**item)

    def is_in(self, **kwargs) -> InDB:
        ln = len(self.__collect.find(kwargs).to_list())

        if ln == 0:
            return InDB.NOT_IN_DB
        elif ln == 1:
            return InDB.IN_DB
        else:
            return InDB.MANY

    def update_auto(
        self,
        item: V,
        type_job: str = "$setOnInsert",
        fields_update: list[str] | None = None,
        get_item_db: bool = False
    ) -> T | None:

        query = {key: item.model_dump().get(key)
                 for key in self.__cls.primary_keys()}
        set_update = self.__create_fields_update(item, fields_update)

        self.__collect.update_one(
            query, {type_job: set_update, "$inc": {"version": 1}}, upsert=True)
        item_db = self.__collect.find_one(query)

        if item_db == None:
            _log.warning("Item %s not found", item)
            return None

        if get_item_db:
            model = self.__cls_db(**item_db)
        del item_db["_id"]
        self.__collect_version.insert_one(item_db)

        if get_item_db:
            return model
        return None

    def __create_fields_update(self, item: V | T, fields_update: list[str] | None = None) -> dict:
        if fields_update == None:
            return item.model_dump()

        set_update = {}
        for key in fields_update:
            value = item.model_dump().get(key)
            if value != None:
                set_update[key] = value
            else:
                _log.warning("Field %s not found in item %s", key, item)
        return set_update


I = TypeVar("I", bound=BaseModel)
O = TypeVar("O", bound=BaseModelInDB)


class ECollectV3(Generic[I, O]):
    def __init__(self, db_client: database.Database, cls: type[I], cls_db: type[O]) -> None:
        self.__db_client = db_client

        name_collect = cls.__name__[0].lower() + cls.__name__[1:]
        name_collect = "".join(
            [ch if ch.islower() else "_" + ch.lower() for ch in name_collect])
        self.__collect = db_client[name_collect]
        self.__collect_version = db_client[name_collect + "_version"]

        self.__cls = cls
        self.__cls_db = cls_db

    @property
    def collect(self) -> Collection:
        return self.__collect

    def insert_one(self, item: I, look_for: bool = False) -> None | O:
        id = self.__collect.insert_one(
            {**item.model_dump(), "version": 1}).inserted_id

        if id is None:
            raise Exception("Item not inserted")

        item_db = self.__collect.find_one({"_id": id})
        if item_db is None:
            raise Exception("Item not found")
        m_item_db = self.__cls_db(**item_db)

        d_item_db = m_item_db.model_dump()

        self.__collect_version.insert_one(d_item_db)

        if look_for:
            return m_item_db

        return None

    def update_one(
        self,
        item: I | None = None,
        filter: dict | None = None,
        query: dict = {},
        look_for: bool = False,
        upsert: bool = False
    ) -> None | O:

        if filter is None:
            raise Exception("Filter not found")

        start_item = self.__collect.find_one(filter)
        if start_item is None:
            raise Exception("Item not found")
        d_start_item = self.__cls_db(**start_item).model_dump()

        queries = {}
        if item is not None:
            queries = {"$set": item.model_dump(), **query}
        else:
            queries = query

        self.__collect.update_one(
            filter, queries, upsert=upsert)

        item_db = self.__collect.find_one(filter)
        if item_db is None:
            raise Exception("Item not found")
        m_item_db = self.__cls_db(**item_db)

        save_version = {"id": m_item_db.id, "version": m_item_db.version}
        for key, value in m_item_db.model_dump().items():
            start_value = d_start_item.get(key)
            if start_value != value:
                save_version[key] = value

        self.__collect_version.insert_one(save_version)

        if look_for:
            return self.__cls_db(**item_db)

        return None

    def delete_one(self, id: str) -> dict:
        cnt = self.__collect.delete_one({"_id": ObjectId(id)}).deleted_count

        if cnt == 1:
            return {"status": "success"}
        return {"status": "error"}

    def find(self, filter: dict = {}, start: int = 0, limit: int = -1) -> list[O]:
        res = []
        i = 0

        if limit == -1:
            for item in self.__collect.find(filter):
                if i >= start:
                    res.append(self.__cls_db(**item))
                i += 1
            return res

        for item in self.__collect.find(filter):
            if i >= start:
                res.append(self.__cls_db(**item))
            if i == start + limit:
                break
            i += 1

        return res

    def find_one(self, **kwargs) -> O | None:
        if kwargs.get("id") != None:
            kwargs["_id"] = ObjectId(kwargs["id"])
            del kwargs["id"]

        item = self.__collect.find_one(kwargs)
        if item == None:
            return None

        return self.__cls_db(**item)

    def is_in(self, query: dict | None = None, **kwargs) -> InDB:
        if query is not None:
            kwargs = {**kwargs, **query}

        ln = len(self.__collect.find(kwargs).to_list())

        if ln == 0:
            return InDB.NOT_IN_DB
        elif ln == 1:
            return InDB.IN_DB
        else:
            return InDB.MANY


class EDataBase():
    student: ECollectV3[Student, StudentInDB]
    structure: ECollectV3[StuctureExcel, StuctureExcelInDB]

    def __init__(self, host: str, port: int, name_db: str):
        self.__db: database.Database = MongoClient(
            host=host, port=port)[name_db]

        self.student = ECollectV3(
            self.__db, Student, StudentInDB)

        self.structure = ECollectV3(
            self.__db, StuctureExcel, StuctureExcelInDB
        )

    @property
    def db(self) -> database.Database:
        return self.__db


db = EDataBase("localhost", 27017, "parser")
