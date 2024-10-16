from io import BytesIO
from fastapi import HTTPException, UploadFile
import pandas as pd

from config import DB
from src.schemas import Course, User_course


def update_report(file: UploadFile):
    try:
        excel = pd.ExcelFile(BytesIO(file.file.read()))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="File read error")
    
    collection = DB.get_user_course_collection()
    collection_auth = DB.get_user_auth_collection()
    collection.delete_many({})

    for sheet_name in excel.sheet_names[1:]:
        df = excel.parse(sheet_name)
        data = df.to_dict('records')

        for item in data:
            parse_data(item, collection, collection_auth)
    return {"status" : "success"}


def parse_data(item, collection, collection_auth):
    try:
        email = get_email(item)
        
        if(collection.find_one({"email" : email})):
            course = get_course(item)
            collection.update_one({"email" : email}, {"$addToSet" : {"courses" : course.model_dump()}})
        else:
            group = get_group(item)
            if(group[:2] == "РИ"):
                user = create_user(item, email, group)
                user = collection.insert_one(user.model_dump(by_alias=True, exclude=["id"]))
                user_id = user.inserted_id
                if(collection_auth.find_one({"email" : email})):
                    collection_auth.update_one({"email" : email}, {"$set" : {"id_user_course" : str(user_id)}})
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Parse student error")

def get_course(item) -> Course:
    return Course(name=item["Название ОК"], score=str(item["Итоговый балл"]), university=item["держатель курса"])
    
def get_email(item):
    email = ""
    if("Адрес электронной почты" in item.keys()):
        email = item["Адрес электронной почты"]
    elif("upn (e-mail)" in item.keys()):
        email = item["upn (e-mail)"]
    else:
        print(item.keys())
    return email

def get_group(item):
    return item["Группа"]

def create_user(item, email, group) -> User_course:
    course = get_course(item)
    return User_course(
        sername=item["Фамилия"],
        name=item["Имя"],
        patronymic=None if str(item["Отчество"]) == "nan" else item["Отчество"],
        email=email,
        group=group,
        courses=[course.model_dump()]
    )
