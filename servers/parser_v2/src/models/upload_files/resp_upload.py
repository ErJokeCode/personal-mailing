from enum import Enum
from pydantic import BaseModel


class TypeFile(Enum):
    ONLINE_COURSE = "online"
    STUDENT = "student"
    MODEUS = "modeus"


class InDB(Enum):
    IN_DB = "in_db"
    NOT_IN_DB = "not_in_db"
    MANY = "many"
    ERROR = "error"


class ResponseUpload(BaseModel):
    type_file: TypeFile
    key: str
    count_rows: int
