from typing import Annotated, Optional
from pydantic import AfterValidator, BaseModel, BeforeValidator, Field

from bson import ObjectId


PyObjectId = Annotated[str, BeforeValidator(str)]



class Course(BaseModel):
    name: str | None = None
    university: str | None = None
    score: str | None = None

class User_course(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    sername: str | None = None
    name: str | None = None
    patronymic: str | None = None
    email: str
    group: str | None = None
    courses: list[Course]

class User(BaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    personal_number: str 
    name: str 
    sername: str 
    patronymic: str | None = None
    email: str | None = None
    date_of_birth: str
    group: str
    faculty: str | None = None
    city: str | None = "Екатеринбург"
    department: str | None = None
    number_course: int | None = None
    status: bool | None = False
    type_of_cost: str | None = None
    type_of_education: str | None = None


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
