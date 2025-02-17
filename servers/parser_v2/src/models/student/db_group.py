from pydantic import BaseModel


class InfoGroupInStudent(BaseModel):
    number: str
    number_course: int
    direction_code: str | None = None
    name_speciality: str | None = None
