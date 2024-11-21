from io import BytesIO
from bson import ObjectId
from fastapi import HTTPException, UploadFile
import pandas as pd
from pymongo.collection import Collection

from config import DB
from src.schemas import Course, InfoOnlineCourseInDB, StudentForDict, OnlineCourseStudent


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
        # data = df.to_dict('records')

        fill_in_students_from_one_sheet(df, students, collection_course)
        
        # names_columns = [df.columns[4],*df.columns[11:]]
        # df = df.loc[:, names_columns]

    return students.values()


def fill_in_students_from_one_sheet(df: pd.DataFrame, all_students: dict, collection_course: Collection):
    cols_fill_name = df.columns[:3]
    col_email = df.columns[4]
    col_name_course = df.columns[10]
    col_university = df.columns[6]
    cols_scores = df.columns[11:]
    col_group = df.columns[3]


    for index, row in df.iterrows():
        email = row[col_email]

        course = get_course2(row, col_name_course, col_university, cols_scores, collection_course)

        if email not in all_students.keys():
            all_students[email] = create_student_for_dict(row, cols_fill_name, col_group, email, course).model_dump()
        else:
            all_students[email]["courses"].append(course)

    
def get_course2(row, col_name_course: str, col_university: str, cols_scores: str, collection_course: Collection):
    name_course = row[col_name_course]
    university = row[col_university]
    scores = {}
    for col_score in cols_scores:
        scores[col_score] = row[col_score]

    info_course = collection_course.find_one({"name" : name_course, "university" : {"$regex": university}})

    if info_course == None:
        info_course = create_default_info_course(name_course, university, collection_course)

    course = create_course_in_student(info_course, scores)

    return course.model_dump()

    
def create_default_info_course(name: str, university: str, collection_course: Collection):
    online_course = InfoOnlineCourseInDB(name=name, university=university)
    collection_course.insert_one(online_course.model_dump(by_alias=True, exclude=["id"]))
    return online_course


def create_course_in_student(info_course: InfoOnlineCourseInDB, scores: dict):
    return OnlineCourseStudent(**info_course, scores=scores)


def create_student_for_dict(row, cols_full_name, col_group, email, course):
    return StudentForDict(
        surname=row[cols_full_name[0]],
        name=row[cols_full_name[1]],
        patronymic=None if str(row[cols_full_name[2]]) == 'nan' else row[cols_full_name[2]],
        email=email,
        group=row[col_group],
        courses=[course]
    )


def update_collection(students: list, collection_student: Collection):
    for student in students:
        collection_student.update_one({"name": student["name"], 
                                       "surname": student["surname"], 
                                       "patronymic" : student["patronymic"], 
                                       "group.number" : student["group"]}, 
                              {"$set" : 
                               {"email" : student["email"], 
                                "online_course" : student["courses"]}})






# def create_course(item, collection_course: Collection):
#     course = create_course_from_item(item).model_dump()

#     course_db = collection_course.find_one({"name" : course["name"], "university" : {"$regex": course["university"]}})

#     if course_db == None:
#         course_db = create_info_online_course(collection_course, course["name"], course["university"]).model_dump()

#     course_db["score"] = course["score"]

#     return course_db


# def create_course_from_item(item) -> Course:
#     if "Итоговый балл" in item:
#         return Course(name=item["Название ОК"], 
#                       score=str(item["Итоговый балл"]), 
#                       university=item["держатель курса"])
#     else:
#         return Course(name=item["Название ОК"], 
#                       score=str("Not column"), 
#                       university=item["держатель курса"])
    

# def get_email(item):
#     email = ""
#     if("Адрес электронной почты" in item.keys()):
#         email = item["Адрес электронной почты"]
#     elif("upn (e-mail)" in item.keys()):
#         email = item["upn (e-mail)"]
#     else:
#         print(item.keys())
#     return email


# def get_group(item):
#     return item["Группа"]


# def create_student_course(item, course, email, group) -> StudentForDict:
#     return StudentForDict(
#         surname=item["Фамилия"],
#         name=item["Имя"],
#         patronymic=None if str(item["Отчество"]) == "nan" else item["Отчество"],
#         email=email,
#         group=group,
#         courses=[course]
#     )


# def create_info_online_course(collection_course: Collection, name, university) -> InfoOnlineCourseInDB:
#     inline_course = InfoOnlineCourseInDB(name=name, university=university)
#     res = collection_course.insert_one(inline_course.model_dump(by_alias=True, exclude=["id"]))
#     return inline_course