from datetime import datetime
import uuid

from pydantic import BaseModel, ConfigDict, Field


class CategoryBase(BaseModel):
    name: str = Field(
        min_length=1,
        max_length=150,
        description="Category name",
        example="Python",
    )
    tutor_id: uuid.UUID = Field(
        default_factory=uuid.uuid4,
        description="Tutor id. Required to fill in"
    )


class CategoryCreate(CategoryBase):
    pass


class CategoryUpdate(BaseModel):
    name: str | None = Field(
        min_length=1,
        max_length=150,
        description="Category name",
        example="Python",
        default=None,
    )
    tutor_id: uuid.UUID | None = Field(
        default=None,
        description="Tutor id"
    )


class CategoryResponse(CategoryBase):
    model_config = ConfigDict(from_attributes=True)

    id: uuid.UUID
    created_at: datetime = Field(
        description="Created at", default_factory=datetime.utcnow
    )
    updated_at: datetime = Field(
        description="Updated at", default_factory=datetime.utcnow
    )
