from typing import Annotated

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
        # Depends(db_session.session_getter),
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
        # Depends(db_session.session_getter),
        Depends(db_session.session_getter),
    ],
) -> Category:
    category: Category = await category_crud.create_category(category_in, session)
    return category


@router.get("/{category_id}", response_model=CategoryResponse)
async def get_category(
    category_id: int,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> Category:
    category = await category_crud.get_category_by_id(category_id, session)
    if not category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found",
        )
    return category


@router.put("/{category_id}", response_model=CategoryResponse)
async def update_category(
    category_id: int,
    category_in: CategoryUpdate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> Category:
    try:
        category = await category_crud.update_category(
            category_id,
            category_in,
            session,
            False,
        )
        if not category:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"Category with id {category_id} not found",
            )
        return category
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=str(e),
        ) from e


@router.patch("/{category_id}", response_model=CategoryResponse)
async def patch_category(
    category_id: int,
    category_in: CategoryUpdate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> Category:
    """
    Частично обновляет категорию по указанному ID.
    Обновляются только предоставленные поля.
    """
    category = await category_crud.update_category(
        category_id,
        category_in,
        session,
        True,
    )
    if not category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found",
        )
    return category


@router.delete("/{category_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_category(
    category_id: int,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> None:
    deleted = await category_crud.delete_category(category_id, session)
    if not deleted:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found",
        )
    return None
