from datetime import datetime
from pydantic import BaseModel
from models.base.base import BaseModelInDB, EBaseModel
from models.student.db_group import InfoGroupInStudent
from models.student.db_subject import SubjectInStudent
from models.student.db_online_course import InfoOnlineCourseInStudent


class Student(EBaseModel):
    personal_number: str
    full_name: str
    name: str
    surname: str
    patronymic: str | None = None
    email: str | None = None
    date_of_birth: str
    number_group: str
    number_course: int
    direction_code: str | None = None
    name_speciality: str | None = None
    status: bool | None = False
    type_of_cost: str | None = None
    type_of_education: str | None = None
    subjects: list[SubjectInStudent] = []
    online_course: list[InfoOnlineCourseInStudent] = []

    date_set: datetime = datetime.now()

    @classmethod
    def primary_keys(self) -> list[str]:
        return ["personal_number", "email"]

    @classmethod
    def update_fields_file_student(self) -> list[str]:
        return [
            "status",
            "type_of_cost",
            "type_of_education",
            "number_group",
            "number_course",
            "surname",
            "name",
            "patronymic",
            "full_name",
            "date_of_birth",
            "date_set"
        ]

    # @classmethod
    # def filter_update_fields(self) -> list[str]:
    #     return ["personal_number"]

    # @classmethod
    # def update_fields(self) -> list[str]:
    #     return ["status", "type_of_cost", "type_of_education", "group.number", "group.number_course", "surname", "name", "patronymic", "date_of_birth"]

    # @classmethod
    # def modeus_filter_update_fields(self) -> list[str]:
    #     return ["personal_number"]

    # @classmethod
    # def modeus_update_fields(self) -> list[str]:
    #     return ["group.direction_code", "group.name_speciality", "subjects"]


class StudentInDB(Student, BaseModelInDB):
    pass
