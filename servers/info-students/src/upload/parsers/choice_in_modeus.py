from fastapi import HTTPException, UploadFile
import pandas as pd
from io import BytesIO

from config import DB
from src.schemas import StudentMoseus

def choice_in_modeus(file: UploadFile):
    try:
        excel = pd.ExcelFile(BytesIO(file.file.read()))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="File read error")
    
    try:
        collection_user_modeus = DB.get_user_modeus()
        collection_user_modeus.delete_many({})

        sheet_name = excel.sheet_names[0]
        df = excel.parse(sheet_name)
        data = df.to_dict('records')

        info_one_student = []

        for i in range(len(data)):
            if i == 0:
                info_one_student.append(data[0])
            else:
                item = data[i]
                if item["ФИО"] == info_one_student[-1]["ФИО"]:
                    info_one_student.append(data[i])
                else:
                    parse_data(info_one_student, collection_user_modeus)
                    info_one_student = []
                    info_one_student.append(item)

        if len(info_one_student) != 0:
            parse_data(info_one_student, collection_user_modeus)
    except:
        print(e)
        raise HTTPException(status_code=500, detail="Parse student error")


def parse_data(student, collection):
    fio = student[0]["ФИО"]
    flow = student[0]["Поток"]
    code = student[0]["Код"]
    speciality = student[0]["Специальность"]

    subjects = []
    for item in student:
        subjects.append(item["РМУП"])

    subjects = subjects

    student_modeus = StudentMoseus(fio=fio, flow=flow, code = code, speciality=speciality, subjects=subjects)

    collection.insert_one(student_modeus.model_dump(by_alias=True, exclude=["id"]))