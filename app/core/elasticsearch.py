import logging
from typing import Any

from elasticsearch import AsyncElasticsearch
from elasticsearch.exceptions import NotFoundError
from fastapi import HTTPException, status
from sqlalchemy import select

from app.core.config import settings
from app.models.knowledge_item import KnowledgeItem

logger = logging.getLogger(__name__)

# Создаем клиент Elasticsearch
es_client = AsyncElasticsearch(
    hosts=[settings.es.url],
    basic_auth=(settings.es.username, settings.es.password),
    verify_certs=settings.es.verify_certs,
)

# Имя индекса для knowledge_items
KNOWLEDGE_ITEMS_INDEX = "knowledge_items"

# Маппинг для индекса knowledge_items
KNOWLEDGE_ITEMS_MAPPING = {
    "mappings": {
        "properties": {
            "id": {"type": "integer"},
            "question": {
                "type": "text",
                "analyzer": "russian",
                "fields": {
                    "keyword": {"type": "keyword"},
                    "english": {"type": "text", "analyzer": "english"},
                },
            },
            "answer": {
                "type": "text",
                "analyzer": "russian",
                "fields": {
                    "keyword": {"type": "keyword"},
                    "english": {"type": "text", "analyzer": "english"},
                },
            },
            "tutor_id": {"type": "integer"},
            "category_id": {"type": "integer"},
            "created_at": {"type": "date"},
            "updated_at": {"type": "date"},
        }
    },
    "settings": {
        "analysis": {
            "analyzer": {
                "russian": {
                    "tokenizer": "standard",
                    "filter": ["lowercase", "russian_stop", "russian_stemmer"],
                },
            },
            "filter": {
                "russian_stop": {
                    "type": "stop",
                    "stopwords": "_russian_",
                },
                "russian_stemmer": {
                    "type": "stemmer",
                    "language": "russian",
                },
            },
        }
    },
}


async def init_elasticsearch() -> None:
    """Инициализация Elasticsearch: создание индексов и маппингов."""
    try:
        # Проверяем, существует ли индекс
        exists = await es_client.indices.exists(index=KNOWLEDGE_ITEMS_INDEX)
        if not exists:
            # Создаем индекс с маппингом
            await es_client.indices.create(
                index=KNOWLEDGE_ITEMS_INDEX,
                body=KNOWLEDGE_ITEMS_MAPPING,
            )
            logger.info("Created index %s", KNOWLEDGE_ITEMS_INDEX)
        else:
            logger.info("Index %s already exists", KNOWLEDGE_ITEMS_INDEX)
    except Exception as e:
        logger.error("Error initializing Elasticsearch: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error initializing Elasticsearch: {str(e)}",
        ) from e


async def index_knowledge_item(knowledge_item: dict[str, Any]) -> None:
    """Индексирует элемент знаний в Elasticsearch."""
    try:
        await es_client.index(
            index=KNOWLEDGE_ITEMS_INDEX,
            id=str(knowledge_item["id"]),
            document=knowledge_item,
        )
        # Принудительно обновляем индекс, чтобы изменения были видны сразу
        await es_client.indices.refresh(index=KNOWLEDGE_ITEMS_INDEX)
    except Exception as e:
        logger.error("Error indexing knowledge item: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error indexing knowledge item: {str(e)}",
        ) from e


async def delete_knowledge_item_from_index(knowledge_item_id: int) -> None:
    """Удаляет элемент знаний из индекса Elasticsearch."""
    try:
        await es_client.delete(
            index=KNOWLEDGE_ITEMS_INDEX,
            id=str(knowledge_item_id),
        )
        # Принудительно обновляем индекс, чтобы изменения были видны сразу
        await es_client.indices.refresh(index=KNOWLEDGE_ITEMS_INDEX)
    except NotFoundError:
        # Если документ не найден, просто логируем это
        logger.warning(
            "Knowledge item with id %s not found in Elasticsearch", knowledge_item_id
        )
    except Exception as e:
        logger.error("Error deleting knowledge item from index: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error deleting knowledge item from index: {str(e)}",
        ) from e


async def search_knowledge_items(
    query: str,
    category_id: int | None = None,
    tutor_id: int | None = None,
    from_: int = 0,
    size: int = 10,
) -> dict[str, Any]:
    """
    Поиск элементов знаний в Elasticsearch.

    Args:
        query: Поисковый запрос
        category_id: ID категории для фильтрации (опционально)
        tutor_id: ID тьютора для фильтрации (опционально)
        from_: Начальная позиция для пагинации
        size: Размер страницы для пагинации

    Returns:
        Результаты поиска
    """
    # Базовый поисковый запрос по полям question и answer
    search_query = {
        "query": {
            "bool": {
                "must": [
                    {
                        "multi_match": {
                            "query": query,
                            "fields": [
                                "question^2",  # Повышаем вес поля question
                                "question.english",
                                "answer",
                                "answer.english",
                            ],
                            "type": "best_fields",
                            "fuzziness": "AUTO",
                        }
                    }
                ],
                "filter": [],
            }
        },
        "highlight": {
            "fields": {
                "question": {"number_of_fragments": 1},
                "answer": {"number_of_fragments": 1},
            },
            "pre_tags": ["<strong>"],
            "post_tags": ["</strong>"],
        },
        "from": from_,
        "size": size,
    }

    # Добавляем фильтры, если они указаны
    if category_id is not None:
        search_query["query"]["bool"]["filter"].append(
            {"term": {"category_id": category_id}}
        )

    if tutor_id is not None:
        search_query["query"]["bool"]["filter"].append({"term": {"tutor_id": tutor_id}})

    try:
        result = await es_client.search(
            index=KNOWLEDGE_ITEMS_INDEX,
            body=search_query,
        )
        return result
    except Exception as e:
        logger.error("Error searching knowledge items: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error searching knowledge items: {str(e)}",
        ) from e


async def close_elasticsearch() -> None:
    """Закрывает соединение с Elasticsearch."""
    await es_client.close()


async def reindex_all_knowledge_items(knowledge_items: list[dict[str, Any]]) -> None:
    """
    Переиндексирует все элементы знаний в Elasticsearch.

    Args:
        knowledge_items: Список словарей с элементами знаний
    """
    try:
        # Проверяем, существует ли индекс
        exists = await es_client.indices.exists(index=KNOWLEDGE_ITEMS_INDEX)
        if exists:
            # Удаляем существующий индекс
            await es_client.indices.delete(index=KNOWLEDGE_ITEMS_INDEX)
            logger.info("Deleted index %s", KNOWLEDGE_ITEMS_INDEX)

        # Создаем индекс с маппингом
        await es_client.indices.create(
            index=KNOWLEDGE_ITEMS_INDEX,
            body=KNOWLEDGE_ITEMS_MAPPING,
        )
        logger.info("Created index %s", KNOWLEDGE_ITEMS_INDEX)

        # Индексируем все элементы знаний
        if knowledge_items:
            # Подготавливаем данные для bulk-индексации
            bulk_data = []
            for item in knowledge_items:
                bulk_data.append(
                    {"index": {"_index": KNOWLEDGE_ITEMS_INDEX, "_id": str(item["id"])}}
                )
                bulk_data.append(item)

            # Выполняем bulk-индексацию
            if bulk_data:
                await es_client.bulk(body=bulk_data, refresh=True)
                logger.info("Indexed %s knowledge items", len(knowledge_items))
    except Exception as e:
        logger.error("Error reindexing knowledge items: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error reindexing knowledge items: {str(e)}",
        ) from e


async def index_existing_knowledge_items(session) -> None:
    """
    Получает все существующие элементы знаний из базы данных и индексирует их в Elasticsearch.

    Args:
        session: Асинхронная сессия базы данных
    """
    try:
        # Получаем все существующие элементы знаний
        stmt = select(KnowledgeItem)
        result = await session.execute(stmt)
        knowledge_items = list(result.scalars().all())

        # Преобразуем объекты KnowledgeItem в словари для индексации
        knowledge_items_dicts = [
            knowledge_item_to_dict(item) for item in knowledge_items
        ]

        # Индексируем все элементы знаний
        if knowledge_items_dicts:
            await reindex_all_knowledge_items(knowledge_items_dicts)

        logger.info("Indexed all existing knowledge items")
    except Exception as e:
        logger.error("Error indexing existing knowledge items: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error indexing existing knowledge items: {str(e)}",
        ) from e


def knowledge_item_to_dict(knowledge_item) -> dict[str, Any]:
    """Преобразует объект KnowledgeItem в словарь для индексации в Elasticsearch."""
    return {
        "id": knowledge_item.id,
        "question": knowledge_item.question,
        "answer": knowledge_item.answer,
        "tutor_id": knowledge_item.tutor_id,
        "category_id": knowledge_item.category_id,
        "created_at": knowledge_item.created_at.isoformat(),
        "updated_at": knowledge_item.updated_at.isoformat(),
    }
