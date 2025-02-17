from pydantic import BaseModel


class BaseFilter(BaseModel):
    lenght: int = 10
    page: int = 1


class FilterStudent(BaseFilter):
    name_student: str = ""
    number_group: str = ""
    number_course: str = ""
    form_education: str = ""


class FilterModeus(BaseFilter):
    name_student: str = ""
    subject: str = ""
    team: str = ""


class FilterOnline(BaseFilter):
    oc_name: str = ""


class FilterOnlineStudent(BaseFilter):
    name_student: str = ""
    number_group: str = ""
