from io import BytesIO
from fastapi import HTTPException
import pandas as pd

from db import WorkerCollection
from worker import update_status_history
from src.schemas import HistoryUploadFileInDB, InfoGroupInStudent, Student
from config import WorkerDataBase

def upload_student(link: str, worker_db: WorkerDataBase, hist: HistoryUploadFileInDB) -> dict[str, str]:
    try:
        df = pd.read_excel(link, sheet_name=0)
    except Exception as e:
        print(e)
        update_status_history(hist, text_status="Error file read")
        raise HTTPException(status_code=500, detail="File read error")
    
    try:
        students = []
        for index, item in df.iterrows():
            student = create_student(item)
            students.append(student)
    except Exception as e:
        print(e)
        update_status_history(hist, text_status="Error parse file")
        raise HTTPException(status_code=500, detail="Error parse file")
            
    try:
        col_st = worker_db.student.get_collect()
        col_st.update_many({}, {"$set": {"status": False}})
        
        fl = ["personal_number", "surname", "name", "patronymic", "date_of_birth"]
        up_fl = ["status", "type_of_cost", "type_of_education", "group.number", "group.number_course"]
        
        worker_db.student.bulk_update(fl, up_fl, students, upsert=True)
        
    except Exception as e:
        print(e)
        update_status_history(hist, text_status="Error insert data")
        raise HTTPException(status_code=500, detail="Error insert data")
    
    return {"status": "success"}
        
        
def create_student(item) -> Student:
    FIO = item["Фамилия, имя, отчество"].split()
    personal_number = "0" * (8 - len(str(item["Личный №"]))) + str(item["Личный №"])
    student = Student(
        surname=FIO[0] if len(FIO) > 0 else "",
        name=FIO[1] if len(FIO) > 1 else "",
        patronymic="".join(FIO[2:]) if len(FIO) > 2 else "",
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