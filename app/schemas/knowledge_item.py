from datetime import datetime

from pydantic import BaseModel, ConfigDict, Field


class KnowledgeItemBase(BaseModel):
    question: str = Field(description="Question", example="What is Python?")
    answer: str = Field(
        description="Answer", example="Python is a programming language"
    )
    tutor_id: int = Field(default=1, gt=0, description="Tutor id. Required to fill in")
    category_id: int = Field(default=1, description="Category id")


class KnowledgeItemCreate(KnowledgeItemBase):
    model_config = ConfigDict(strict=True)


class KnowledgeItemUpdate(BaseModel):
    question: str | None = Field(
        default=None, description="Question", example="What is Python?"
    )
    answer: str | None = Field(
        default=None, description="Answer", example="Python is a programming language"
    )
    tutor_id: int | None = Field(default=None, gt=0, description="Tutor id")
    category_id: int | None = Field(default=None, description="Category id")


class KnowledgeItemResponse(KnowledgeItemBase):
    model_config = ConfigDict(from_attributes=True)

    id: int
    created_at: datetime = Field(
        description="Created at", default_factory=datetime.utcnow
    )
    updated_at: datetime = Field(
        description="Updated at", default_factory=datetime.utcnow
    )
