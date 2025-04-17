from pydantic import BaseModel, Field
import uuid
from typing import Any, Dict, List, Optional


class SearchHighlight(BaseModel):
    """Схема для подсветки результатов поиска."""

    question: list[str] | None = None
    answer: list[str] | None = None


class SearchHit(BaseModel):
    """Схема для одного результата поиска."""

    id: uuid.UUID
    question: str
    answer: str
    tutor_id: uuid.UUID
    category_id: uuid.UUID
    created_at: str
    updated_at: str
    score: float
    highlight: SearchHighlight | None = None

    class Config:
        from_attributes = True


class SearchResponse(BaseModel):
    """Схема для ответа на поисковый запрос."""

    total: int = Field(..., description="Общее количество найденных результатов")
    hits: list[SearchHit] = Field(..., description="Список найденных элементов")
    took: int = Field(..., description="Время выполнения запроса в миллисекундах")


class SearchRequest(BaseModel):
    """Схема для поискового запроса."""

    query: str = Field(..., description="Поисковый запрос")
    category_id: uuid.UUID | None = Field(None, description="ID категории для фильтрации")
    tutor_id: uuid.UUID | None = Field(None, description="ID тьютора для фильтрации")
    from_: int = Field(0, description="Начальная позиция для пагинации", ge=0)
    size: int = Field(10, description="Размер страницы для пагинации", ge=1, le=100)


class SearchError(BaseModel):
    """Схема для ошибок поиска."""
    
    detail: str = Field(..., description="Описание ошибки")
    error_type: str | None = Field(None, description="Тип ошибки")
    raw_query: dict[str, Any] | None = Field(None, description="Исходный запрос к Elasticsearch")
