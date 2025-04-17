from pydantic import BaseModel, Field


class FileResponse(BaseModel):
    """Схема ответа с URL загруженного файла"""
    url: str = Field(description="URL загруженного файла") 