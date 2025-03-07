from pydantic import BaseModel, field_validator

from models.base.base import PyObjectId


class SubjectInStudent(BaseModel):
    full_name: str
    name: str
    teams: list["TeamInSubjectInStudent"]
    site_oc_id: PyObjectId | None = None
    file_oc_id: PyObjectId | None = None
    group_tg_link: str | None = None


class TeamInSubjectInStudent(BaseModel):
    name: str
    teachers: list[str]
    group_tg_link: str | None = None

    @field_validator("teachers", mode="before")
    def teachers_valid(cls, v: dict | list[str]) -> list[str]:
        if isinstance(v, list):
            return v

        names = v.get("name")
        if names is not None:
            if isinstance(names, str):
                return [names]
            return names
        raise ValueError
