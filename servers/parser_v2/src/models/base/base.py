
from typing import Annotated, Optional

from pydantic import BaseModel, BeforeValidator, Field


PyObjectId = Annotated[str, BeforeValidator(str)]


class EBaseModel(BaseModel):
    @classmethod
    def primary_keys(self) -> list[str]:
        return []


class BaseModelInDB(EBaseModel):
    id: Optional[PyObjectId] = Field(alias="_id", default=None)
    version: int
