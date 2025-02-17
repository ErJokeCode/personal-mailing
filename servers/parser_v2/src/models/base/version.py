from datetime import datetime
from pydantic import BaseModel


class BaseVersion(BaseModel):
    branch: int
    version: int
