from pydantic import BaseModel, Field


class SearchHighlight(BaseModel):
    """Схема для подсветки результатов поиска."""

    question: list[str] | None = None
    answer: list[str] | None = None


class SearchHit(BaseModel):
    """Схема для одного результата поиска."""

    id: int
    question: str
    answer: str
    tutor_id: int
    category_id: int
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
    category_id: int | None = Field(None, description="ID категории для фильтрации")
    tutor_id: int | None = Field(None, description="ID тьютора для фильтрации")
    from_: int = Field(0, description="Начальная позиция для пагинации", ge=0)
    size: int = Field(10, description="Размер страницы для пагинации", ge=1, le=100)
