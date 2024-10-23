from fastapi import UploadFile
import pandas as pd

from config import DB
from schemas import Course, Student



def update_statement(file: UploadFile):
    excel = pd.ExcelFile(file.file.read())
    collection = DB.get_collection("users")

    i = 0
    for sheet_name in excel.sheet_names:
        if i == 0:
            i += 1
            continue
        i += 1
        print(f"Processing sheet: {i} / {len(excel.sheet_names)}")

        df = excel.parse(sheet_name)
        data = df.to_dict('records')

        for item in data:
            parse_data(item, collection)
    return {"message" : "Success"}


def parse_data(item, collection):
    email = get_email(item)
    
    if(collection.find_one({"email" : email})):
        course = get_course(item)
        collection.update_one({"email" : email}, {"$push" : {"courses" : course.model_dump()}})
    else:
        group = get_group(item)
        if(group[:2] == "РИ"):
            user = create_user(item, email, group)
            collection.insert_one(user.model_dump())

def get_course(item) -> Course:
    return Course(name=item["Название ОК"], score=str(item["Итоговый балл"]))
    
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

def create_user(item, email, group) -> Student:
    course = get_course(item)
    return Student(
        surname=item["Фамилия"],
        name=item["Имя"],
        patronymic=None if str(item["Отчество"]) == "nan" else item["Отчество"],
        email=email,
        group=group,
        courses=[course.model_dump()]
    )
