from enum import Enum
from pydantic import BaseModel


class ResponseStudent(BaseModel):
    name: dict[int, str]
    cafedra: dict[int, str]
    number_course: dict[int, int]
    group: dict[int, str]
    status: dict[int, str]
    type_of_cost: dict[int, str]
    type_of_education: dict[int, str]
    date_birth: dict[int, str]
    personal_number: dict[int, str]
    in_db: dict[int, bool]
