from typing import Any, Optional, List
import uuid

from sqlalchemy import inspect, select
from sqlalchemy.engine import Result
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.elasticsearch import (
    delete_knowledge_item_from_index,
    index_knowledge_item,
    knowledge_item_to_dict,
)
from app.models.category import Category
from app.models.knowledge_item import KnowledgeItem
from app.schemas.knowledge_item import KnowledgeItemCreate, KnowledgeItemUpdate, KnowledgeItemTagsUpdate
from app.utils.tag_generator import get_top_tags
from app.models.version_types import ChangeType
from app.crud.knowledge_item_version import create_knowledge_item_version


async def get_knowledge_items(
    session: AsyncSession,
    skip: int = 0,
    limit: int = 100,
) -> List[KnowledgeItem]:
    stmt = select(KnowledgeItem).offset(skip).limit(limit)
    result = await session.execute(stmt)
    return list(result.scalars().all())


async def get_knowledge_items_by_category(
    session: AsyncSession,
    category_id: uuid.UUID,
    skip: int = 0,
    limit: int = 100,
) -> List[KnowledgeItem]:
    stmt = select(KnowledgeItem).where(
        KnowledgeItem.category_id == category_id
    ).offset(skip).limit(limit)
    result = await session.execute(stmt)
    return list(result.scalars().all())


async def create_knowledge_item(
    session: AsyncSession,
    question: str,
    answer: str,
    question_tags: List[str],
    answer_tags: List[str],
    category_id: uuid.UUID,
    tutor_id: uuid.UUID,
) -> KnowledgeItem:

    if question_tags is None:
        question_tags = []
    if answer_tags is None:
        answer_tags = []


    if not isinstance(question_tags, list):
        question_tags = list(question_tags) if question_tags else []
    if not isinstance(answer_tags, list):
        answer_tags = list(answer_tags) if answer_tags else []


    print(f"Creating knowledge item with tags after processing: question_tags={question_tags}, answer_tags={answer_tags}")


    knowledge_item = KnowledgeItem()
    knowledge_item.question = question
    knowledge_item.answer = answer
    knowledge_item.category_id = category_id
    knowledge_item.tutor_id = tutor_id
    knowledge_item.question_tags = question_tags
    knowledge_item.answer_tags = answer_tags

    session.add(knowledge_item)
    await session.flush()


    print(f"After flush: knowledge_item.question_tags={knowledge_item.question_tags}, knowledge_item.answer_tags={knowledge_item.answer_tags}")


    await create_knowledge_item_version(
        session=session,
        knowledge_item=knowledge_item,
        change_type=ChangeType.CREATE,
        tutor_id=tutor_id,
    )

    await session.commit()
    await session.refresh(knowledge_item)


    print(f"After commit: knowledge_item.question_tags={knowledge_item.question_tags}, knowledge_item.answer_tags={knowledge_item.answer_tags}")

    return knowledge_item


async def get_knowledge_item(
    session: AsyncSession,
    knowledge_item_id: uuid.UUID,
) -> Optional[KnowledgeItem]:
    stmt = select(KnowledgeItem).where(KnowledgeItem.id == knowledge_item_id)
    result = await session.execute(stmt)
    return result.scalar_one_or_none()


async def category_exists(category_id: uuid.UUID, session: AsyncSession) -> bool:
    stmt = select(Category).where(Category.id == category_id)
    result = await session.execute(stmt)
    return result.scalar_one_or_none() is not None


async def update_knowledge_item(
    session: AsyncSession,
    knowledge_item_id: uuid.UUID,
    question: str,
    answer: str,
    question_tags: List[str],
    answer_tags: List[str],
    category_id: uuid.UUID,
    tutor_id: uuid.UUID,
) -> Optional[KnowledgeItem]:
    knowledge_item = await get_knowledge_item(session, knowledge_item_id)
    if not knowledge_item:
        return None


    await create_knowledge_item_version(
        session=session,
        knowledge_item=knowledge_item,
        change_type=ChangeType.UPDATE,
        tutor_id=tutor_id,
    )


    knowledge_item.question = question
    knowledge_item.answer = answer
    knowledge_item.question_tags = question_tags
    knowledge_item.answer_tags = answer_tags
    knowledge_item.category_id = category_id

    await session.commit()
    await session.refresh(knowledge_item)
    return knowledge_item


async def update_knowledge_item_tags(
    session: AsyncSession,
    knowledge_item: KnowledgeItem,
    tags_update: KnowledgeItemTagsUpdate,
) -> KnowledgeItem:
    update_data = tags_update.model_dump(exclude_unset=True)


    for field, value in update_data.items():
        setattr(knowledge_item, field, value)

    await session.commit()
    await session.refresh(knowledge_item)


    await index_knowledge_item(knowledge_item_to_dict(knowledge_item))

    return knowledge_item


async def delete_knowledge_item(
    session: AsyncSession,
    knowledge_item_id: uuid.UUID,
    tutor_id: uuid.UUID,
) -> bool:
    knowledge_item = await get_knowledge_item(session, knowledge_item_id)
    if not knowledge_item:
        return False


    item_data = {
        "knowledge_item_id": knowledge_item.id,
        "question": knowledge_item.question,
        "answer": knowledge_item.answer,
        "question_tags": knowledge_item.question_tags,
        "answer_tags": knowledge_item.answer_tags,
        "category_id": knowledge_item.category_id,
        "tutor_id": tutor_id,
        "change_type": ChangeType.DELETE
    }


    await create_knowledge_item_version(
        session=session,
        knowledge_item=knowledge_item,
        change_type=ChangeType.DELETE,
        tutor_id=tutor_id,
    )

    await session.flush()


    await session.delete(knowledge_item)
    await session.commit()
    return True
