from typing import Annotated
import uuid

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.session import db_session
from app.crud import category_version as category_version_crud
from app.crud import category as category_crud
from app.models.category import CategoryVersion, Category
from app.schemas.category_version import CategoryVersionResponse
from app.schemas.category import CategoryResponse

router = APIRouter(tags=["versions 🔁"])

@router.get("/", response_model=list[CategoryVersionResponse])
async def get_category_versions(
    session: Annotated[AsyncSession, Depends(db_session.session_getter)],
    skip: int = 0,
    limit: int = 100,
) -> list[CategoryVersion]:
    category_versions = await category_version_crud.get_category_versions(
        session, skip, limit
    )
    return category_versions

@router.post("/{version_id}/apply", response_model=CategoryResponse)
async def apply_category_version(
    version_id: uuid.UUID,
    tutor_id: uuid.UUID,
    session: Annotated[AsyncSession, Depends(db_session.session_getter)],
) -> Category:
    """Применяет указанную версию категории"""
    # Получаем версию
    version = await category_version_crud.get_category_version(session, version_id)
    if not version:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category version with id {version_id} not found",
        )
    
    # Проверяем существование категории
    category = await category_crud.get_category(session, version.category_id)
    if not category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {version.category_id} not found",
        )
    
    # Применяем версию
    category = await category_version_crud.apply_category_version(
        session, version, tutor_id, category
    )
    
    return category
