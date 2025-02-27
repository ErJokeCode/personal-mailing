from pydantic import BaseModel

from models.upload_files.resp_upload import InDB


class RespModeus(BaseModel):
    index: int
    full_name: str
    direction_code: str
    name_speciality: str
    subject_name: str
    team_name: str
    teacher: str
    in_db: InDB = InDB.ERROR
