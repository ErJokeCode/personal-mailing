#!/usr/bin/env python3
"""
Скрипт для загрузки тестовых данных в базу знаний.
Использует существующие модели и схемы для создания категорий и элементов знаний.
"""

import asyncio
import json
import logging
from pathlib import Path
import uuid
from typing import Any

from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.config import settings
from app.core.elasticsearch import close_elasticsearch, es_client, init_elasticsearch
from app.core.session import db_session
from app.crud.category import create_category
from app.crud.knowledge_item import create_knowledge_item
from app.models.category import Category
from app.models.knowledge_item import KnowledgeItem
from app.schemas.category import CategoryCreate
from app.schemas.knowledge_item import KnowledgeItemCreate

# Настройка логирования
logging.basicConfig(
    level=settings.logging.log_level_value,
    format=settings.logging.log_format,
)
logger = logging.getLogger(__name__)


async def get_category_id_by_name(session: AsyncSession, name: str) -> uuid.UUID | None:
    """Получение ID категории по имени."""
    stmt = select(Category).where(Category.name == name)
    result = await session.execute(stmt)
    category = result.scalar_one_or_none()
    return category.id if category else None


async def get_knowledge_item_by_question(
    session: AsyncSession, question: str, category_id: uuid.UUID
) -> uuid.UUID | None:
    """Получение ID элемента знаний по вопросу и категории."""
    stmt = select(KnowledgeItem).where(
        KnowledgeItem.question == question, KnowledgeItem.category_id == category_id
    )
    result = await session.execute(stmt)
    item = result.scalar_one_or_none()
    return item.id if item else None


async def seed_data() -> None:
    """Загрузка тестовых данных в базу данных и Elasticsearch."""

    data_file = Path(__file__).parent / "test_data.json"

    if not data_file.exists():
        logger.error("Файл с тестовыми данными не найден: %s", data_file)
        return


    with open(data_file, "r", encoding="utf-8") as f:
        data = json.load(f)

    try:

        await init_elasticsearch()


        async with db_session.session_factory() as session:

            category_map = {}
            for cat_data in data.get("categories", []):

                existing_id = await get_category_id_by_name(session, cat_data["name"])
                if existing_id:
                    logger.info(
                        "Категория уже существует: %s (ID: %s)",
                        cat_data["name"],
                        existing_id,
                    )
                    category_map[cat_data["name"]] = existing_id
                    continue

                cat_schema = CategoryCreate(
                    name=cat_data["name"], tutor_id=cat_data["tutor_id"]
                )
                category = await create_category(cat_schema, session)
                category_map[cat_data["name"]] = category.id
                logger.info(
                    "Создана категория: %s (ID: %s)", cat_data["name"], category.id
                )


            for item_data in data.get("knowledge_items", []):

                category_name = item_data["category_name"]
                category_id = category_map.get(category_name)
                if not category_id:
                    logger.error(
                        "Категория не найдена: %s", category_name
                    )
                    continue


                existing_id = await get_knowledge_item_by_question(
                    session, item_data["question"], category_id
                )
                if existing_id:
                    logger.info(
                        "Элемент знаний уже существует: %s (ID: %s)",
                        item_data["question"],
                        existing_id,
                    )
                    continue


                item_schema = KnowledgeItemCreate(
                    question=item_data["question"],
                    answer=item_data["answer"],
                    tutor_id=uuid.UUID(item_data["tutor_id"]),
                    category_id=category_id,
                    question_tags=item_data.get("question_tags", []),
                    answer_tags=item_data.get("answer_tags", []),
                )
                item = await create_knowledge_item(item_schema, session)
                logger.info(
                    "Создан элемент знаний: %s (ID: %s)",
                    item_data["question"],
                    item.id,
                )


            await es_client.indices.refresh(index="knowledge_items")

            logger.info("Загрузка тестовых данных завершена успешно!")
    except Exception as e:
        logger.error("Ошибка при загрузке данных: %s", e)
        raise
    finally:

        await close_elasticsearch()


async def search_knowledge_items(
    query: str,
    category_id: uuid.UUID | None = None,
    tutor_id: uuid.UUID | None = None,
    from_: int = 0,
    size: int = 10,
) -> dict[str, Any]:
    """Поиск элементов знаний в Elasticsearch."""
    try:


        response = await es_client.search(
            index=KNOWLEDGE_ITEMS_INDEX,
            query=search_query,
            from_=from_,
            size=size,
        )


        return response
    except Exception as e:
        logger.error(f"Ошибка при поиске элементов знаний: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail="Ошибка при поиске элементов знаний",
        )


if __name__ == "__main__":
    asyncio.run(seed_data())
