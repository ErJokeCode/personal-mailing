from typing import Any

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
from app.schemas.knowledge_item import KnowledgeItemCreate, KnowledgeItemUpdate


async def get_knowledge_items(
    session: AsyncSession,
    skip: int,
    limit: int,
) -> list[KnowledgeItem]:
    stmt = select(KnowledgeItem).offset(skip).limit(limit).order_by(KnowledgeItem.id)
    result: Result = await session.execute(stmt)
    return list(result.scalars().all())


async def create_knowledge_item(
    knowledge_item_in: KnowledgeItemCreate,
    session: AsyncSession,
) -> KnowledgeItem:
    knowledge_item = KnowledgeItem(**knowledge_item_in.model_dump())
    session.add(knowledge_item)
    await session.commit()
    await session.refresh(knowledge_item)

    # Индексируем в Elasticsearch
    await index_knowledge_item(knowledge_item_to_dict(knowledge_item))

    return knowledge_item


async def get_knowledge_item_by_id(
    knowledge_item_id: int,
    session: AsyncSession,
) -> KnowledgeItem | None:
    stmt = select(KnowledgeItem).where(KnowledgeItem.id == knowledge_item_id)
    result: Result = await session.execute(stmt)
    return result.scalar_one_or_none()


async def category_exists(category_id: int, session: AsyncSession) -> bool:
    if category_id == 0:
        return False
    stmt = select(Category).where(Category.id == category_id)
    result = await session.execute(stmt)
    return result.scalar_one_or_none() is not None


async def update_knowledge_item(
    knowledge_item_id: int,
    knowledge_item_in: KnowledgeItemUpdate,
    session: AsyncSession,
    exclude_unset: bool,
) -> KnowledgeItem | None:
    knowledge_item = await get_knowledge_item_by_id(knowledge_item_id, session)
    if not knowledge_item:
        return None

    update_data = knowledge_item_in.model_dump(exclude_unset=exclude_unset)

    # Получаем информацию о колонках модели
    mapper = inspect(KnowledgeItem)
    non_nullable_columns = [
        c.key for c in mapper.columns if not c.nullable and c.key != "id"
    ]

    # Проверка существования категории
    if "category_id" in update_data and update_data["category_id"] is not None:
        category_id = update_data["category_id"]
        if not await category_exists(category_id, session):
            raise ValueError(f"Категория с id={category_id} не существует")

    # Проверка tutor_id = 0
    if "tutor_id" in update_data and update_data["tutor_id"] == 0:
        raise ValueError("tutor_id не может быть равен 0")

    # Для обязательных полей с NULL значениями:
    for field in non_nullable_columns:
        if field in update_data and update_data[field] is None:
            if exclude_unset:
                # Для PATCH - пропускаем NULL значения
                del update_data[field]
            else:
                # Для PUT - выдаем ошибку, т.к. обязательные поля не могут быть NULL
                raise ValueError(
                    f"Поле '{field}' не может быть NULL при полном обновлении (PUT)"
                )

    for field, value in update_data.items():
        setattr(knowledge_item, field, value)

    await session.commit()
    await session.refresh(knowledge_item)

    # Обновляем в Elasticsearch
    await index_knowledge_item(knowledge_item_to_dict(knowledge_item))

    return knowledge_item


async def delete_knowledge_item(
    knowledge_item_id: int,
    session: AsyncSession,
) -> bool:
    knowledge_item = await get_knowledge_item_by_id(knowledge_item_id, session)
    if not knowledge_item:
        return False

    await session.delete(knowledge_item)
    await session.commit()

    # Удаляем из Elasticsearch
    await delete_knowledge_item_from_index(knowledge_item_id)

    return True
