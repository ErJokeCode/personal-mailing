from pydantic import BaseModel

from models.upload_files.resp_upload import InDB


class RespStudent(BaseModel):
    index: int
    personal_number: str
    full_name: str
    name: str
    surname: str
    patronymic: str | None = None
    date_of_birth: str
    number_group: str
    number_course: int
    status: bool | None = False
    type_of_cost: str | None = None
    type_of_education: str | None = None
    in_db: InDB = InDB.ERROR
