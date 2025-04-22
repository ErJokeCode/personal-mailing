from typing import Annotated
import uuid

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.session import db_session
from app.crud import knowledge_item_version as knowledge_item_version_crud
from app.crud import knowledge_item as knowledge_item_crud
from app.models.knowledge_item_version import KnowledgeItemVersion
from app.models.knowledge_item import KnowledgeItem
from app.schemas.knowledge_item_version import KnowledgeItemVersionResponse
from app.schemas.knowledge_item import KnowledgeItemResponse

router = APIRouter(tags=["versions 🔁"])

@router.get("/", response_model=list[KnowledgeItemVersionResponse])
async def get_knowledge_item_versions(
    session: Annotated[AsyncSession, Depends(db_session.session_getter)],
    skip: int = 0,
    limit: int = 100,
) -> list[KnowledgeItemVersion]:
    knowledge_item_versions = await knowledge_item_version_crud.get_knowledge_item_versions(
        session, skip, limit
    )
    return knowledge_item_versions

@router.post("/{version_id}/apply", response_model=KnowledgeItemResponse)
async def apply_knowledge_item_version(
    version_id: uuid.UUID,
    tutor_id: uuid.UUID,
    session: Annotated[AsyncSession, Depends(db_session.session_getter)],
) -> KnowledgeItem:
    """Применяет указанную версию элемента знаний"""

    version = await knowledge_item_version_crud.get_knowledge_item_version(session, version_id)
    if not version:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item version with id {version_id} not found",
        )


    knowledge_item = await knowledge_item_crud.get_knowledge_item(session, version.knowledge_item_id)
    if not knowledge_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {version.knowledge_item_id} not found",
        )


    knowledge_item = await knowledge_item_version_crud.apply_knowledge_item_version(
        session, version, tutor_id, knowledge_item
    )

    return knowledge_item
