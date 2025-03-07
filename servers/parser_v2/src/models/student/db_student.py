from datetime import datetime
from pydantic import BaseModel, field_validator
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

    @field_validator("personal_number", mode="before")
    def personal_number_valid(cls, v: str | int) -> str:
        return (8 - len(str(v))) * "0" + str(v)

    @field_validator("status", mode="before")
    def status_valid(cls, v: str | bool) -> bool:
        if isinstance(v, bool):
            return v
        if v == "Активный":
            return True
        return False

    @field_validator("date_of_birth", mode="before")
    def date_of_birth_valid(cls, v: datetime | str) -> str:
        if isinstance(v, str):
            return v
        return v.strftime("%d.%m.%Y")

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

    @classmethod
    def get_names_col_db(self) -> list[str]:
        return [
            "student__personal_number",
            "student__full_name",
            "student__name",
            "student__surname",
            "student__patronymic",
            "student__email",
            "student__date_of_birth",
            "student__number_group",
            "student__number_course",
            "student__direction_code",
            "student__name_speciality",
            "student__status",
            "student__type_of_cost",
            "student__type_of_education",
            "subjects__full_name",
            "subjects__name",
            "teams__name",
            "teachers_name",
            "online_course__name",
            "online_course__score"
        ]


class StudentInDB(Student, BaseModelInDB):
    pass
