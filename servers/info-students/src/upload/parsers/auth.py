from io import BytesIO
from fastapi import HTTPException, UploadFile
import pandas as pd

from config import DB
from src.schemas import User


def update_auth(file: UploadFile):
    try:
        excel = pd.ExcelFile(BytesIO(file.file.read()))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="File read error")
    
    collection = DB.get_user_collection()
    collection_auth = DB.get_user_auth_collection()
    collection.delete_many({})

    sheet_name = excel.sheet_names[0]

    df = pd.read_excel(excel, sheet_name=sheet_name)
    data = df.to_dict('records')

    for item in data:
        user_auth = get_user_auth(item)
        if(collection.find_one({"personal_number" : user_auth.personal_number}) == None):
            user = collection.insert_one(user_auth.model_dump(by_alias=True, exclude=["id"]))
            if(collection_auth.find_one({"personal_number" : user_auth.personal_number})):
                collection_auth.update_one({"personal_number" : user_auth.personal_number}, {"$set" : {"id_user" : str(user.inserted_id)}})

    return {"status" : "success"}

def get_user_auth(item):
    FIO = item["Фамилия, имя, отчество"].split()
    return User(
        sername=FIO[0] if len(FIO) > 0 else "",
        name=FIO[1] if len(FIO) > 1 else "",
        patronymic=FIO[2] if len(FIO) > 2 else "",
        faculty = item["Форм. факультет"],
        city = item["Территориальное подразделение"],
        department = item["Кафедра"],
        number_course = item["Курс"],
        group = item["Группа"],
        status = True if item["Состояние"] == "Активный" else False,
        type_of_cost = item["Вид возм. затрат"],
        type_of_education = item["Форма освоения"],
        date_of_birth = item["Дата рождения"],
        personal_number = str(item["Личный №"])
    )
