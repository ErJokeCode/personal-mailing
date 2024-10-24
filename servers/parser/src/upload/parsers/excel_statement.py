from io import BytesIO
from fastapi import HTTPException, UploadFile
import pandas as pd

from config import DB
from src.schemas import Course, StudentCourse


def update_report(file: UploadFile):
    try:
        excel = pd.ExcelFile(BytesIO(file.file.read()))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="File read error")
    
    collection = DB.get_student()
    collection_course = DB.get_course_info_collection()
    all_students = {}
    
    for sheet_name in excel.sheet_names[1:]:
        df = excel.parse(sheet_name)

        data = df[df["Группа"].apply(lambda x: "РИ-" in x)].to_dict('records')
        for student in data:
            email = get_email(student)
            course = get_course(student).model_dump()
            course_db = collection_course.find_one({"name" : course["name"], "university" : {"$regex": course["university"]}})
            course_db["score"] = course["score"]
            if email not in all_students.keys():
                all_students[email] = create_student_course(student, course_db, get_email(student), get_group(student)).model_dump()
            else:
                all_students[email]["courses"].append(course_db)

    for student in all_students.values():
        collection.update_one({"name": student["name"], "surname": student["surname"], "patronymic" : student["patronymic"], "group.number" : student["group"]}, 
                              {"$set" : {"email" : student["email"], "online_course" : student["courses"]}})
    
    return {"status" : "success"}


def get_student(item, collection):
    try:
        email = get_email(item)
        
        if(collection.find_one({"email" : email})):
            course = get_course(item)
            collection.update_one({"email" : email}, {"$addToSet" : {"courses" : course.model_dump()}})
        else:
            group = get_group(item)
            if(group[:2] == "РИ"):
                user = create_student_course(item, email, group)
                return user.model_dump(by_alias=True, exclude=["id"])
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

def create_student_course(item, course, email, group) -> StudentCourse:
    return StudentCourse(
        surname=item["Фамилия"],
        name=item["Имя"],
        patronymic=None if str(item["Отчество"]) == "nan" else item["Отчество"],
        email=email,
        group=group,
        courses=[course]
    )
