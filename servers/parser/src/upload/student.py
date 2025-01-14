from io import BytesIO
from fastapi import HTTPException
import pandas as pd

from db import WorkerCollection
from src.schemas import HistoryUploadFileInDB, InfoGroupInStudent, Student
from config import WorkerDataBase


def upload_student(link: str, worker_db: WorkerDataBase, hist: HistoryUploadFileInDB) -> dict[str, str]:
    try:
        df = pd.read_excel(link, sheet_name=0)
    except Exception as e:
        print(e)
        hist.status_upload = "Error file read"
        worker_db.history.update_one(hist, get_item=False)
        raise HTTPException(status_code=500, detail="File read error")
    
    try:
        students = []
        for index, item in df.iterrows():
            student = create_student(item)
            students.append(student)
    except Exception as e:
        print(e)
        hist.status_upload = "Error parse file"
        worker_db.history.update_one(hist, get_item=False)
        raise HTTPException(status_code=500, detail="Error parse file")
            
    try:
        worker_db.student.delete_many()
        worker_db.student.insert_many(students)
        
    except Exception as e:
        print(e)
        hist.status_upload = "Error insert data"
        worker_db.history.update_one(hist, get_item=False)
        raise HTTPException(status_code=500, detail="Error insert data")
    
    return {"status": "success"}
        
        
def create_student(item) -> Student:
    FIO = item["Фамилия, имя, отчество"].split()
    personal_number = "0" * (8 - len(str(item["Личный №"]))) + str(item["Личный №"])
    student = Student(
        surname=FIO[0] if len(FIO) > 0 else "",
        name=FIO[1] if len(FIO) > 1 else "",
        patronymic="".join(FIO[2:]) if len(FIO) > 2 else "",
        faculty = item["Форм. факультет"],
        city = item["Территориальное подразделение"],
        department = item["Кафедра"],
        group = InfoGroupInStudent(number=item["Группа"], number_course=int(item["Курс"])),
        status = True if item["Состояние"] == "Активный" else False,
        type_of_cost = item["Вид возм. затрат"],
        type_of_education = item["Форма освоения"],
        date_of_birth = item["Дата рождения"],
        personal_number = personal_number, 
        subjects=[],
        online_course=[]
    )
    
    return student