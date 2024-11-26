from typing import Annotated, List
from bson import ObjectId
from fastapi import APIRouter, Body, HTTPException, UploadFile
from fastapi import Depends
from bson import json_util
import bson
import json
import datetime
from pymongo.collection import Collection

from config import DB
from src.schemas import (
    Course,
    GetUserAuth,
    InfoOnlineCourseInDB,
    StudentForDict,
    Student,
    StudentInBD,
    Subject,
    SubjectInBD,
    UserAuth,
    OnlineCourseStudent,
)


router_user = APIRouter(
    prefix="/student",
    tags=["Student"],
)


@router_user.get("/{id}/courses")
async def get_courses(id: str) -> list[OnlineCourseStudent]:
    try:
        collection_student = DB.get_student()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    try:
        res = collection_student.find_one({"_id": ObjectId(id)})
        return res["online_course"]
    except Exception as e:
        print(e)
        raise HTTPException(status_code=404, detail="User not found")


@router_user.get("/")
async def get_student(email: str) -> Student:
    try:
        collection = DB.get_student()
        collection_subject = DB.get_subject()
        collection_course = DB.get_course_info_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")

    user = collection.find_one({"email": email})

    student_db = StudentInBD(**user)

    student = from_StudentInBD_to_Student(collection_subject, collection_course, student_db)

    return student


@router_user.get("/all")
async def get_student() -> List[Student]:
    try:
        collection = DB.get_student()
        collection_subject = DB.get_subject()
        collection_course = DB.get_course_info_collection()
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error DB")
    
    users = []
    for user in collection.find({}):
        student_db = StudentInBD(**user)
        student = from_StudentInBD_to_Student(collection_subject, collection_course, student_db)

        users.append(student)

    return users


def from_StudentInBD_to_Student(
        collection_subject: Collection, 
        collection_course: Collection, 
        student_db: StudentInBD) -> Student:
    
    student = create_Student(student_db)
        
    for subject_id in student_db.subjects:
        info_subject = collection_subject.find_one({"_id" : ObjectId(subject_id)})
        if info_subject != None:
            subject_db = SubjectInBD(**info_subject)

            course_info = collection_course.find_one({"_id": ObjectId(subject_db.online_course_id)})

            course=None
            if course_info != None:
                course = InfoOnlineCourseInDB(**course_info)
            subject = create_Subject(subject_db, course)

            student.subjects.append(subject)
    
    return student


def create_Student(student_db: StudentInBD) -> Student:
    return Student(
            id=student_db.id, 
            personal_number=student_db.personal_number, 
            name=student_db.name,
            surname=student_db.surname,
            patronymic=student_db.patronymic,
            email=student_db.email, 
            date_of_birth=student_db.date_of_birth, 
            group=student_db.group, 
            status=student_db.status, 
            type_of_cost=student_db.type_of_cost, 
            type_of_education=student_db.type_of_education, 
            subjects=[], 
            online_course=student_db.online_course)


def create_Subject(subject_db: SubjectInBD, course: InfoOnlineCourseInDB) -> Subject:
    return Subject(
                _id=subject_db.id,
                full_name=subject_db.full_name,
                name=subject_db.name,
                form_education=subject_db.form_education,
                info=subject_db.info,
                online_course=course, 
                group_tg_link=subject_db.group_tg_link)