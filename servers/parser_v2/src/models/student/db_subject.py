from pydantic import BaseModel

from models.base.base import PyObjectId


class SubjectInStudent(BaseModel):
    full_name: str
    name: str
    teams: list["TeamInSubjectInStudent"]
    form_education: str
    site_oc_id: PyObjectId | None = None
    file_oc_id: PyObjectId | None = None
    group_tg_link: str | None = None


class TeamInSubjectInStudent(BaseModel):
    name: str
    teachers: list[str]
    group_tg_link: str | None = None
