from bson import ObjectId
from fastapi import HTTPException, UploadFile
import pandas as pd
from pandas.core.frame import DataFrame
from io import BytesIO
from pymongo.collection import Collection

from config import DB
from src.schemas import DictNames, InfoOnlineCourseInDB, StudentMoseus, Subject, SubjectInBD

def choice_in_modeus(file_read):
    try:
        excel = pd.ExcelFile(BytesIO(file_read))
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="File read error")
    
    try:
        collection_OC = DB.get_course_info_collection()

        collection_subject = DB.get_subject()
        collection_subject.delete_many({})

        collection_student = DB.get_student()

        collection_not_found = DB.get_student_not_found_modeus()
        collection_not_found.delete_many({})

        sheet_name = excel.sheet_names[0]
        df = excel.parse(sheet_name)

        fill_subjects(df, collection_subject)

        fill_students(df, collection_subject, collection_student, collection_not_found)

        return {"status" : "seccess"}

    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Parse student error")
    

def get_subject_info(subject : str) -> Subject:
    if subject.lower().find("онлайн") != -1:
        subject_split = subject.split("(")

        name = subject_split[0].strip()
        form = "online"
        university = subject_split[-1][:-1].split(", ")[1]

        return Subject(full_name=subject, name=name, form_education=form, info=university)
    
    elif subject.lower().find("смешанн") != -1:
        subject_split = subject.split("(")

        form = "mixed"
        name = subject_split[0].strip()
        info = subject_split[-1][:-1]

        return Subject(full_name=subject, name=name, form_education=form, info=info)

    elif subject.lower().find("(") != -1:
        subject_split = subject.split("(")

        form = "other"
        name = subject_split[0].strip()
        info = subject_split[-1][:-1]
        
        return Subject(full_name=subject, name=name, form_education=form, info=info)
    
    else:
        return Subject(full_name=subject, name=subject.strip(), form_education="traditional", info=None)


def fill_subjects(df : DataFrame, collection_subject: Collection):
    subjects = set(df["Муп название"].unique())
    parse_subjects = []
    for subject in subjects:
        subject_info = get_subject_info(subject)

        online_course_id = get_id_online_course_for_subject(subject_info.full_name)
        subject_db = SubjectInBD(
            full_name=subject_info.full_name, 
            name=subject_info.name, 
            form_education=subject_info.form_education,
            info=subject_info.info, 
            online_course_id=online_course_id)

        parse_subjects.append(subject_db.model_dump(by_alias=True, exclude=["id"]))
    collection_subject.insert_many(parse_subjects)


def fill_students(df : DataFrame, collection_subject, collection_student, collection_not_found):
    df_choices_student = df[["ФИО", "Код", "Специальность", "Муп название"]].groupby(['ФИО',"Код", "Специальность"])['Муп название']
    choices_student = df_choices_student.unique()

    not_found = []
    for info_student, choice_subjects in choices_student.items():
        fio = info_student[0]
        FIO = fio.split()
        surname=FIO[0] if len(FIO) > 0 else ""
        name=FIO[1] if len(FIO) > 1 else ""
        patronymic=FIO[2] if len(FIO) > 2 else ""

        info_subjects = []
            
        for choice_subject in choice_subjects:
            subject = collection_subject.find_one({"full_name" : choice_subject})
            info_subjects.append(subject["_id"]) 

        group = info_student[1]
        speciality = info_student[2] 

        if collection_student.find_one({"name" : name, "surname" : surname, "patronymic" : patronymic}):
            collection_student.update_one(
                    {"name" : name, "surname" : surname, "patronymic" : patronymic}, 
                    {"$set" : {"subjects" : info_subjects, "group.direction_code" : group, "group.name_speciality" : speciality}})
        else:
            not_found.append({"name" : name, "surname" : surname, "patronymic" : patronymic, "subjects" : info_subjects, "group" : {"direction_code" : group, "name_speciality" : speciality}})
    collection_not_found.insert_many(not_found)


def get_id_online_course_for_subject(name: str) -> ObjectId:
    try: 
        col_online_course = DB.get_course_info_collection()
    except:
        raise HTTPException(status_code=500, detail="Error DB")
    
    name_online_course = get_inf(name)
    if name_online_course == None:
        return None
    
    online_course = col_online_course.find_one({"name" : name_online_course})
    if online_course != None:
        course = InfoOnlineCourseInDB(**online_course)
        return ObjectId(course.id)
    return None
    
def get_inf(name: str):
    col_mod_inf = DB.get_dict_names()
    dict = col_mod_inf.find_one({"modeus" : name})
    if dict != None:
        return  DictNames(**dict).site_inf
    return None