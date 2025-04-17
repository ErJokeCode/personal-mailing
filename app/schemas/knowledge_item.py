from datetime import datetime
import uuid
from typing import List, Optional

from pydantic import BaseModel, ConfigDict, Field


class KnowledgeItemBase(BaseModel):
    question: str = Field(description="Question", example="What is Python?")
    answer: str = Field(
        description="Answer", example="Python is a programming language"
    )
    tutor_id: uuid.UUID = Field(
        default_factory=uuid.uuid4,
        description="Tutor id. Required to fill in"
    )
    category_id: uuid.UUID = Field(
        default_factory=uuid.uuid4,
        description="Category id"
    )
    question_tags: List[str] = Field(
        default=[],
        description="Question tags",
        example=["python", "programming language"]
    )
    answer_tags: List[str] = Field(
        default=[],
        description="Answer tags",
        example=["definition", "programming"]
    )


class KnowledgeItemCreate(KnowledgeItemBase):
    model_config = ConfigDict(
        strict=True,
        json_schema_extra={
            "example": {
                "question": "Как скомпилировать Go-приложение в исполняемый файл?",
                "answer": "Для компиляции Go-программы используйте команду `go build`.",
                "tutor_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                "category_id": "4170c8da-d232-43cd-a96c-fa2ca2a6c8e4",
                "question_tags": [],
                "answer_tags": []
            }
        }
    )
    
    tutor_id: str = Field(description="Tutor id в формате UUID")
    category_id: str = Field(description="Category id в формате UUID")
    
    question_tags: List[str] = Field(
        default_factory=list,
        description="Question tags",
        example=["python", "programming language"]
    )
    answer_tags: List[str] = Field(
        default_factory=list,
        description="Answer tags",
        example=["definition", "programming"]
    )

    def model_dump(self, **kwargs):
        data = super().model_dump(**kwargs)
        if 'tutor_id' in data:
            data['tutor_id'] = uuid.UUID(data['tutor_id'])
        if 'category_id' in data:
            data['category_id'] = uuid.UUID(data['category_id'])
        if 'question_tags' not in data or data['question_tags'] is None:
            data['question_tags'] = []
        if 'answer_tags' not in data or data['answer_tags'] is None:
            data['answer_tags'] = []
        return data


class KnowledgeItemUpdate(BaseModel):
    question: Optional[str] = Field(
        default=None, description="Question", example="What is Python?"
    )
    answer: Optional[str] = Field(
        default=None, description="Answer", example="Python is a programming language"
    )
    tutor_id: Optional[uuid.UUID] = Field(
        default=None,
        description="Tutor id"
    )
    category_id: Optional[uuid.UUID] = Field(
        default=None,
        description="Category id"
    )
    question_tags: Optional[List[str]] = Field(
        default=None,
        description="Question tags",
        example=["python", "programming language"]
    )
    answer_tags: Optional[List[str]] = Field(
        default=None,
        description="Answer tags",
        example=["definition", "programming"]
    )


class KnowledgeItemTagsUpdate(BaseModel):
    question_tags: Optional[List[str]] = Field(
        default=None, 
        description="Question tags", 
        example=["python", "programming language"]
    )
    answer_tags: Optional[List[str]] = Field(
        default=None, 
        description="Answer tags", 
        example=["definition", "programming"]
    )


class KnowledgeItemResponse(KnowledgeItemBase):
    model_config = ConfigDict(from_attributes=True)

    id: uuid.UUID
    created_at: datetime = Field(
        description="Created at", default_factory=datetime.utcnow
    )
    updated_at: datetime = Field(
        description="Updated at", default_factory=datetime.utcnow
    )
