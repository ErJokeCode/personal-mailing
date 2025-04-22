from typing import Annotated, Optional
import uuid

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.session import db_session
from app.crud import knowledge_item as knowledge_item_crud
from app.models.knowledge_item import KnowledgeItem
from app.schemas.knowledge_item import (
    KnowledgeItemCreate,
    KnowledgeItemResponse,
    KnowledgeItemUpdate,
    KnowledgeItemTagsUpdate,
)
from app.crud.knowledge_item_version import create_knowledge_item_version
from app.models.version_types import ChangeType
from app.utils.tag_generator import get_top_tags

router = APIRouter(tags=["knowledge-items 📖"])


@router.get("/", response_model=list[KnowledgeItemResponse])
async def get_knowledge_items(
    session: Annotated[
        AsyncSession,
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
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    category_exists = await knowledge_item_crud.category_exists(
        uuid.UUID(knowledge_item_in.category_id), session
    )
    if not category_exists:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {knowledge_item_in.category_id} not found",
        )
    
    knowledge_item = KnowledgeItem()
    knowledge_item.question = knowledge_item_in.question
    knowledge_item.answer = knowledge_item_in.answer
    knowledge_item.tutor_id = uuid.UUID(knowledge_item_in.tutor_id)
    knowledge_item.category_id = uuid.UUID(knowledge_item_in.category_id)
    
    if not knowledge_item_in.question_tags:
        knowledge_item.question_tags = get_top_tags(knowledge_item.question)
    else:
        knowledge_item.question_tags = list(knowledge_item_in.question_tags)
        
    if not knowledge_item_in.answer_tags:
        knowledge_item.answer_tags = get_top_tags(knowledge_item.answer)
    else:
        knowledge_item.answer_tags = list(knowledge_item_in.answer_tags)
    
    print(f"POST: Creating knowledge item with tags={knowledge_item.question_tags}, {knowledge_item.answer_tags}")
    
    session.add(knowledge_item)
    await session.flush()
    
    await create_knowledge_item_version(
        session=session,
        knowledge_item=knowledge_item,
        change_type=ChangeType.CREATE,
        tutor_id=knowledge_item.tutor_id,
    )
    
    await session.commit()
    await session.refresh(knowledge_item)
    
    print(f"After commit: question_tags={knowledge_item.question_tags}, answer_tags={knowledge_item.answer_tags}")
    
    return knowledge_item


@router.get("/{knowledge_item_id}", response_model=KnowledgeItemResponse)
async def get_knowledge_item(
    knowledge_item_id: uuid.UUID,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    knowledge_item: KnowledgeItem | None = await knowledge_item_crud.get_knowledge_item(
        session, knowledge_item_id
    )
    if not knowledge_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {knowledge_item_id} not found",
        )
    return knowledge_item


@router.patch("/{knowledge_item_id}", response_model=KnowledgeItemResponse)
async def patch_knowledge_item(
    knowledge_item_id: uuid.UUID,
    knowledge_item_in: KnowledgeItemUpdate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    """
    Обновляет элемент знаний по указанному ID.
    Поддерживает как полное, так и частичное обновление.
    При полном обновлении все обязательные поля должны быть указаны.
    При частичном обновлении можно указать только те поля, которые нужно изменить.
    """
    knowledge_item: KnowledgeItem | None = await knowledge_item_crud.get_knowledge_item(
        session, knowledge_item_id
    )
    if not knowledge_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {knowledge_item_id} not found",
        )
    
    tutor_id = knowledge_item_in.tutor_id if knowledge_item_in.tutor_id else knowledge_item.tutor_id
    
    if knowledge_item_in.question is not None:
        knowledge_item.question = knowledge_item_in.question
    if knowledge_item_in.answer is not None:
        knowledge_item.answer = knowledge_item_in.answer
    if knowledge_item_in.category_id:
        knowledge_item.category_id = knowledge_item_in.category_id
    
    if knowledge_item_in.question_tags is not None:
        if knowledge_item_in.question_tags:
            knowledge_item.question_tags = list(knowledge_item_in.question_tags)
        else:
            knowledge_item.question_tags = get_top_tags(knowledge_item.question)
    
    if knowledge_item_in.answer_tags is not None:
        if knowledge_item_in.answer_tags:
            knowledge_item.answer_tags = list(knowledge_item_in.answer_tags)
        else:
            knowledge_item.answer_tags = get_top_tags(knowledge_item.answer)
    
    print(f"PATCH with tags: question_tags={knowledge_item.question_tags}, answer_tags={knowledge_item.answer_tags}")
    
    await create_knowledge_item_version(
        session=session,
        knowledge_item=knowledge_item,
        change_type=ChangeType.UPDATE,
        tutor_id=tutor_id,
    )
    
    await session.commit()
    await session.refresh(knowledge_item)
    
    return knowledge_item


@router.patch("/{knowledge_item_id}/tags", response_model=KnowledgeItemResponse)
async def update_knowledge_item_tags(
    knowledge_item_id: uuid.UUID,
    tags_update: KnowledgeItemTagsUpdate,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> KnowledgeItem:
    """
    Обновляет теги элемента знаний по указанному ID.
    Можно обновлять как теги вопроса (question_tags), так и теги ответа (answer_tags).
    Если поле не указано, оно остается без изменений.
    """
    knowledge_item: KnowledgeItem | None = await knowledge_item_crud.get_knowledge_item(
        session, knowledge_item_id
    )
    if not knowledge_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {knowledge_item_id} not found",
        )
    
    data = tags_update.model_dump(exclude_unset=True)
    print(f"Tags update data: {data}")
    
    if 'question_tags' in data and data['question_tags'] is None:
        data['question_tags'] = []
    if 'answer_tags' in data and data['answer_tags'] is None:
        data['answer_tags'] = []
        
    if 'question_tags' in data:
        knowledge_item.question_tags = data['question_tags']
    if 'answer_tags' in data:
        knowledge_item.answer_tags = data['answer_tags']
    
    await session.commit()
    await session.refresh(knowledge_item)
    
    print(f"After update: knowledge_item.question_tags={knowledge_item.question_tags}, knowledge_item.answer_tags={knowledge_item.answer_tags}")
    
    return knowledge_item


@router.delete("/{knowledge_item_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_knowledge_item(
    knowledge_item_id: uuid.UUID,
    session: Annotated[
        AsyncSession,
        Depends(db_session.session_getter),
    ],
) -> None:
    knowledge_item: KnowledgeItem | None = await knowledge_item_crud.get_knowledge_item(
        session, knowledge_item_id
    )
    if not knowledge_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Knowledge item with id {knowledge_item_id} not found",
        )
    
    await create_knowledge_item_version(
        session=session,
        knowledge_item=knowledge_item,
        change_type=ChangeType.DELETE,
        tutor_id=knowledge_item.tutor_id,
    )
    
    await knowledge_item_crud.delete_knowledge_item(session, knowledge_item_id)
