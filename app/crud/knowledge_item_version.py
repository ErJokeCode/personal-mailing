import uuid
from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession

from app.models.knowledge_item import KnowledgeItem
from app.models.knowledge_item_version import KnowledgeItemVersion
from app.models.version_types import ChangeType

async def create_knowledge_item_version(
    session: AsyncSession,
    knowledge_item: KnowledgeItem,
    change_type: ChangeType,
    tutor_id: uuid.UUID,
    deleted_with_category_version_id: uuid.UUID = None,
) -> KnowledgeItemVersion:
    """Создает запись версии элемента знаний"""
    knowledge_item_version = KnowledgeItemVersion(
        knowledge_item_id=knowledge_item.id,
        question=knowledge_item.question,
        answer=knowledge_item.answer,
        question_tags=knowledge_item.question_tags,
        answer_tags=knowledge_item.answer_tags,
        category_id=knowledge_item.category_id,
        tutor_id=tutor_id,
        change_type=change_type,
        deleted_with_category_version_id=deleted_with_category_version_id,
    )
    session.add(knowledge_item_version)
    await session.flush()
    return knowledge_item_version

async def get_knowledge_item_versions(
    session: AsyncSession,
    skip: int,
    limit: int,
) -> list[KnowledgeItemVersion]:
    stmt = (
        select(KnowledgeItemVersion)
        .offset(skip)
        .limit(limit)
        .order_by(KnowledgeItemVersion.created_at.desc())
    )
    result = await session.execute(stmt)
    return list(result.scalars().all())

async def get_knowledge_item_version(
    session: AsyncSession,
    version_id: uuid.UUID,
) -> KnowledgeItemVersion | None:
    stmt = select(KnowledgeItemVersion).where(KnowledgeItemVersion.id == version_id)
    result = await session.execute(stmt)
    return result.scalar_one_or_none()

async def apply_knowledge_item_version(
    session: AsyncSession,
    version: KnowledgeItemVersion,
    tutor_id: uuid.UUID,
    knowledge_item: KnowledgeItem,
) -> KnowledgeItem:
    """Применяет старую версию к текущему элементу знаний"""

    knowledge_item.question = version.question
    knowledge_item.answer = version.answer
    knowledge_item.question_tags = version.question_tags
    knowledge_item.answer_tags = version.answer_tags
    knowledge_item.category_id = version.category_id


    await create_knowledge_item_version(
        session=session,
        knowledge_item=knowledge_item,
        change_type=ChangeType.RESTORE,
        tutor_id=tutor_id,
    )

    await session.commit()
    await session.refresh(knowledge_item)
    return knowledge_item
