from enum import Enum
from pydantic import BaseModel


class TypeFile(Enum):
    ONLINE_COURSE = "online_course"
    STUDENT = "student"
    MODEUS = "modeus"


class ResponseUpload(BaseModel):
    type_file: TypeFile
    key: str
    count_rows: int
