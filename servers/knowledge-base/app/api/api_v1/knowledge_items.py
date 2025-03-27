from typing import Annotated

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.session import db_session
from app.crud import knowledge_item as knowledge_item_crud
from app.models.knowledge_item import KnowledgeItem
from app.schemas.knowledge_item import (
    KnowledgeItemCreate,
    KnowledgeItemResponse,
    KnowledgeItemUpdate,
)

router = APIRouter(tags=["knowledge-items 📖"])


@router.get("/", response_model=list[KnowledgeItemResponse])
async def get_knowledge_items(
    session: Annotated[
        AsyncSession,
        # Depends(db_session.session_getter),
        Depends(db_session.session_getter),
    ],
    skip: int = 0,
    limit: int = 100,
) -> list[KnowledgeItem]:
    knowledge_items = await knowledge_item_crud.get_knowledge_items(
        session, skip, limit
    )
    return knowledge_items


@router.post(
    "/", response_model=KnowledgeItemResponse, status_code=status.HTTP_201_CREATED
)
async def create_knowledge_item(
    knowledge_item_in: KnowledgeItemCreate,
    session: Annotated[
        AsyncSession,
        # Depends(db_session.session_getter),
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    knowledge_item: KnowledgeItem = await knowledge_item_crud.create_knowledge_item(
        knowledge_item_in, session
    )
    return knowledge_item


@router.get("/{knowledge_item_id}", response_model=KnowledgeItemResponse)
async def get_knowledge_item(
    knowledge_item_id: int,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    knowledge_item = await knowledge_item_crud.get_knowledge_item_by_id(
        knowledge_item_id, session
    )
    if not knowledge_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {knowledge_item_id} not found",
        )
    return knowledge_item


@router.put("/{knowledge_item_id}", response_model=KnowledgeItemResponse)
async def update_knowledge_item(
    knowledge_item_id: int,
    knowledge_item_in: KnowledgeItemUpdate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    try:
        knowledge_item = await knowledge_item_crud.update_knowledge_item(
            knowledge_item_id,
            knowledge_item_in,
            session,
            False,
        )
        if not knowledge_item:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"Knowledge item with id {knowledge_item_id} not found",
            )
        return knowledge_item
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=str(e),
        ) from e


@router.patch("/{knowledge_item_id}", response_model=KnowledgeItemResponse)
async def patch_knowledge_item(
    knowledge_item_id: int,
    knowledge_item_in: KnowledgeItemUpdate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    """
    Частично обновляет элемент знаний по указанному ID.
    Обновляются только предоставленные поля.
    """
    knowledge_item = await knowledge_item_crud.update_knowledge_item(
        knowledge_item_id,
        knowledge_item_in,
        session,
        True,
    )
    if not knowledge_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {knowledge_item_id} not found",
        )
    return knowledge_item


@router.delete("/{knowledge_item_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_knowledge_item(
    knowledge_item_id: int,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> None:
    deleted = await knowledge_item_crud.delete_knowledge_item(
        knowledge_item_id, session
    )
    if not deleted:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {knowledge_item_id} not found",
        )
    return None
