from datetime import datetime
import uuid
from typing import Optional
from pydantic import BaseModel, ConfigDict, Field

from app.models.version_types import ChangeType

class CategoryVersionBase(BaseModel):
    category_id: uuid.UUID
    name: str
    tutor_id: uuid.UUID
    change_type: ChangeType

class CategoryVersionCreate(CategoryVersionBase):
    pass

class CategoryVersionResponse(CategoryVersionBase):
    model_config = ConfigDict(from_attributes=True)
    
    id: uuid.UUID
    created_at: datetime
