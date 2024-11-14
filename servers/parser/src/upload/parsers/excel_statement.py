from io import BytesIO
from bson import ObjectId
from fastapi import HTTPException, UploadFile
import pandas as pd
from pymongo.collection import Collection

from config import DB
from src.schemas import Course, OnlineCourseInDB, StudentCourse


def update_report(file: UploadFile):
    try:
        excel = pd.ExcelFile(BytesIO(file.file.read()))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="File read error")
    
    try:
        collection_student = DB.get_student()
        collection_course = DB.get_course_info_collection()
    except Exception as e:
        raise HTTPException(status_code=500, detail="Error collection")
    
    students = parse_students(excel, collection_course)

    update_collection(students, collection_student)
    
    return {"status" : "success"}


def parse_students(excel: pd.ExcelFile, collection_course: Collection):
    students = {}
    
    for sheet_name in excel.sheet_names[1:]:
        df = excel.parse(sheet_name)

        df = df[df["Группа"].apply(lambda x: "РИ-" in x)]
        data = df.to_dict('records')

        fill_in_students_from_one_sheet(data, students, collection_course)
        
        # names_columns = [df.columns[4],*df.columns[11:]]
        # print(df.loc[:, names_columns].to_dict('records'))

    return students.values()


def fill_in_students_from_one_sheet(data: list[dict], all_students: dict, collection_course: Collection):
    for student in data:
        email = get_email(student)
        course = create_course(student, collection_course)
        
        if email not in all_students.keys():
            all_students[email] = create_student_course(student, course, email, get_group(student)).model_dump()
        else:
            all_students[email]["courses"].append(course)


def create_course(item, collection_course: Collection):
    course = create_course_from_item(item).model_dump()

    course_db = collection_course.find_one({"name" : course["name"], "university" : {"$regex": course["university"]}})

    if course_db == None:
        course_db = create_info_online_course(collection_course, course["name"], course["university"]).model_dump()

    course_db["score"] = course["score"]

    return course_db


def update_collection(students: list, collection_student: Collection):
    for student in students:
        collection_student.update_one({"name": student["name"], 
                                       "surname": student["surname"], 
                                       "patronymic" : student["patronymic"], 
                                       "group.number" : student["group"]}, 
                              {"$set" : 
                               {"email" : student["email"], 
                                "online_course" : student["courses"]}})


def create_course_from_item(item) -> Course:
    if "Итоговый балл" in item:
        return Course(name=item["Название ОК"], 
                      score=str(item["Итоговый балл"]), 
                      university=item["держатель курса"])
    else:
        return Course(name=item["Название ОК"], 
                      score=str("Not column"), 
                      university=item["держатель курса"])
    

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


def create_info_online_course(collection_course: Collection, name, university) -> OnlineCourseInDB:
    inline_course = OnlineCourseInDB(name=name, university=university)
    res = collection_course.insert_one(inline_course.model_dump(by_alias=True, exclude=["id"]))
    return inline_course