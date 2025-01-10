from io import BytesIO
from fastapi import HTTPException
import pandas as pd

from db import WorkerCollection
from src.schemas import InfoGroupInStudent, Student
from config import WorkerDataBase


def upload_student(link: str, worker_db: WorkerDataBase) -> dict[str, str]:
    try:
        df = pd.read_excel(link, sheet_name=0)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="File read error")
    
    try:
        students = []
        for index, item in df.iterrows():
            student = create_student(item)
            students.append(student)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error parse file")
            
    try:
        worker_db.student.delete_many()
        worker_db.student.insert_many(students)
        
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error insert data")
    
    return {"status": "success"}
        
        
def create_student(item) -> Student:
    FIO = item["Фамилия, имя, отчество"].split()
    student = Student(
        surname=FIO[0] if len(FIO) > 0 else "",
        name=FIO[1] if len(FIO) > 1 else "",
        patronymic=FIO[2] if len(FIO) > 2 else "",
        faculty = item["Форм. факультет"],
        city = item["Территориальное подразделение"],
        department = item["Кафедра"],
        group = InfoGroupInStudent(number=item["Группа"], number_course=int(item["Курс"])),
        status = True if item["Состояние"] == "Активный" else False,
        type_of_cost = item["Вид возм. затрат"],
        type_of_education = item["Форма освоения"],
        date_of_birth = item["Дата рождения"],
        personal_number = str(item["Личный №"]), 
        subjects=[],
        online_course=[]
    )
    
    return student