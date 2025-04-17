from typing import Annotated
import uuid

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.session import db_session
from app.crud import category as category_crud
from app.models.category import Category
from app.schemas.category import CategoryCreate, CategoryResponse, CategoryUpdate

router = APIRouter(tags=["categories 📚"])


@router.get("/", response_model=list[CategoryResponse])
async def get_categories(
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
    skip: int = 0,
    limit: int = 100,
) -> list[Category]:
    categories: list[Category] = await category_crud.get_categories(
        session, skip, limit
    )
    return categories


@router.post("/", response_model=CategoryResponse, status_code=status.HTTP_201_CREATED)
async def create_category(
    category_in: CategoryCreate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> Category:
    category: Category = await category_crud.create_category(
        session=session,
        name=category_in.name,
        tutor_id=category_in.tutor_id
    )
    return category


@router.get("/{category_id}", response_model=CategoryResponse)
async def get_category(
    category_id: uuid.UUID,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> Category:
    category: Category | None = await category_crud.get_category(
        session, category_id
    )
    if not category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found",
        )
    return category


@router.patch("/{category_id}", response_model=CategoryResponse)
async def patch_category(
    category_id: uuid.UUID,
    category_in: CategoryUpdate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> Category:
    """
    Обновляет категорию по указанному ID.
    Поддерживает как полное, так и частичное обновление.
    При полном обновлении все обязательные поля должны быть указаны.
    При частичном обновлении можно указать только те поля, которые нужно изменить.
    """
    category: Category | None = await category_crud.get_category(
        session, category_id
    )
    if not category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found",
        )
    
    tutor_id = category_in.tutor_id if category_in.tutor_id else category.tutor_id
    
    category = await category_crud.update_category(
        session=session, 
        category_id=category_id,
        name=category_in.name or category.name,
        tutor_id=tutor_id
    )
    
    if not category:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to update category with id {category_id}",
        )
    
    return category


@router.delete("/{category_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_category(
    category_id: uuid.UUID,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> None:
    category: Category | None = await category_crud.get_category(
        session, category_id
    )
    if not category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found",
        )
    
    result = await category_crud.delete_category(
        session=session,
        category_id=category_id,
        tutor_id=category.tutor_id
    )
    
    if not result:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to delete category with id {category_id}",
        )
