from pydantic import BaseModel


class InfoOnlineCourse(BaseModel):
    name: str
    university: str | None = None
    date_start: str | None = None
    deadline: list[str] | None = None
    info: str | None = None


class InfoOnlineCourseInStudent(InfoOnlineCourse):
    scores: dict | None = None
