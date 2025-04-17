from datetime import datetime
import uuid
from typing import List, Optional
from pydantic import BaseModel, ConfigDict, Field

from app.models.version_types import ChangeType

class KnowledgeItemVersionBase(BaseModel):
    knowledge_item_id: uuid.UUID
    question: str
    answer: str
    question_tags: List[str]
    answer_tags: List[str]
    category_id: uuid.UUID
    tutor_id: uuid.UUID
    change_type: ChangeType
    deleted_with_category_version_id: Optional[uuid.UUID] = None

class KnowledgeItemVersionCreate(KnowledgeItemVersionBase):
    pass

class KnowledgeItemVersionResponse(KnowledgeItemVersionBase):
    model_config = ConfigDict(from_attributes=True)
    
    id: uuid.UUID
    created_at: datetime
