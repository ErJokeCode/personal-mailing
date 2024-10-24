from typing import Annotated, Optional
from pydantic import AfterValidator, BaseModel, BeforeValidator, Field

from bson import ObjectId


PyObjectId = Annotated[str, BeforeValidator(str)]

class OnlineCourse(BaseModel):
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
    subjects: list[object]
    online_course: list[object]

class OnlineCourseStudent(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    name: str 
    university: str | None = None
    date_start: str | None = None
    deadline: list[str] | None = None
    info: str | None = None
    score: str | None = None




class Course(BaseModel):
    name: str | None = None
    university: str | None = None
    score: str | None = None

class StudentCourse(BaseModel):
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

    
class Course_info(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    name: str
    date: str
    university: str
    info: str


class StudentMoseus(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    fio: str
    flow: str
    code: str
    speciality: str
    subjects: list[str]