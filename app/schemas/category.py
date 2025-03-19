from datetime import datetime

from pydantic import BaseModel, ConfigDict, Field


class CategoryBase(BaseModel):
    name: str = Field(
        min_length=5,
        max_length=150,
        description="Category name",
        example="Python",
    )
    tutor_id: int = Field(default=1, gt=0, description="Tutor id. Required to fill in")


class CategoryCreate(CategoryBase):
    pass


class CategoryUpdate(BaseModel):
    name: str | None = Field(
        min_length=5,
        max_length=150,
        description="Category name",
        example="Python",
        default=None,
    )
    tutor_id: int | None = Field(default=None, gt=0, description="Tutor id")


class CategoryResponse(CategoryBase):
    model_config = ConfigDict(from_attributes=True)

    id: int
    created_at: datetime = Field(
        description="Created at", default_factory=datetime.utcnow
    )
    updated_at: datetime = Field(
        description="Updated at", default_factory=datetime.utcnow
    )
