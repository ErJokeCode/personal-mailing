from typing import Annotated, List, Optional
from pydantic import AfterValidator, BaseModel, BeforeValidator, Field

from bson import DBRef, ObjectId


PyObjectId = Annotated[str, BeforeValidator(str)]


class InfoOnlineCourseInDB(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    name: str
    university: str | None = None
    date_start: str | None = None
    deadline: list[str] | None = None
    info: str | None = None


class Subject(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    full_name: str
    name: str
    form_education: str
    info: str | None = None
    online_course: InfoOnlineCourseInDB | None = None
    group_tg_link: str | None = None


class SubjectInBD(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    full_name: str
    name: str
    form_education: str
    info: str | None = None
    online_course_id: PyObjectId | None = None
    group_tg_link: str | None = None


class OnlineCourseStudent(InfoOnlineCourseInDB):
    scores: dict | None = None


class Student(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    personal_number: str
    name: str
    surname: str
    patronymic: str | None = None
    email: str | None = None
    date_of_birth: str
    group: dict
    status: bool | None = False
    type_of_cost: str | None = None
    type_of_education: str | None = None
    subjects: list[Subject]
    online_course: list[OnlineCourseStudent]

class StudentInBD(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    personal_number: str
    name: str
    surname: str
    patronymic: str | None = None
    email: str | None = None
    date_of_birth: str
    group: dict
    status: bool | None = False
    type_of_cost: str | None = None
    type_of_education: str | None = None
    subjects: list[PyObjectId]
    online_course: list[OnlineCourseStudent]


class Course(BaseModel):
    name: str | None = None
    university: str | None = None
    score: str | None = None


class StudentForDict(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    surname: str | None = None
    name: str | None = None
    patronymic: str | None = None
    email: str
    group: str | None = None
    courses: list[dict]


class UserAuth(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    email: str
    personal_number: str
    chat_id: str
    id_user: str | None = None
    id_user_course: str | None = None


class GetUserAuth(BaseModel):
    email: str
    personal_number: str
    chat_id: str


class StudentMoseus(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    fio: str
    flow: str
    code: str
    speciality: str
    subjects: list[str]

class DictNames(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    modeus: str
    site_inf: str | None = None
    file_course: str | None = None
