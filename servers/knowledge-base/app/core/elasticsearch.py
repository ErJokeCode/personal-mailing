import logging
from typing import Any
import uuid

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
            "id": {"type": "keyword"},
            "question": {
                "type": "text",
                "analyzer": "russian",
                "fields": {
                    "keyword": {"type": "keyword", "ignore_above": 256},
                    "english": {"type": "text", "analyzer": "english"},
                },
            },
            "answer": {
                "type": "text",
                "analyzer": "russian",
                "fields": {
                    "keyword": {"type": "keyword", "ignore_above": 256},
                    "english": {"type": "text", "analyzer": "english"},
                },
            },
            "tutor_id": {"type": "keyword"},
            "category_id": {"type": "keyword"},
            "question_tags": {"type": "keyword"},
            "answer_tags": {"type": "keyword"},
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
    """Инициализация Elasticsearch."""
    try:

        info = await es_client.info()
        logger.info(f"Подключение к Elasticsearch установлено. Версия: {info.get('version', {}).get('number', 'неизвестно')}")


        index_exists = await es_client.indices.exists(index=KNOWLEDGE_ITEMS_INDEX)

        if not index_exists:

            await es_client.indices.create(
                index=KNOWLEDGE_ITEMS_INDEX,
                body=KNOWLEDGE_ITEMS_MAPPING,
            )
            logger.info(f"Индекс {KNOWLEDGE_ITEMS_INDEX} создан")
        else:

            try:
                count_result = await es_client.count(index=KNOWLEDGE_ITEMS_INDEX)
                doc_count = count_result.get("count", 0)
                logger.info(f"Индекс {KNOWLEDGE_ITEMS_INDEX} уже существует, документов: {doc_count}")


                mapping = await es_client.indices.get_mapping(index=KNOWLEDGE_ITEMS_INDEX)
                logger.info(f"Текущий маппинг индекса: {mapping}")

            except Exception as count_error:
                logger.warning(f"Не удалось получить информацию об индексе: {count_error}")
    except Exception as e:
        logger.error(f"Ошибка при инициализации Elasticsearch: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при инициализации Elasticsearch: {str(e)}",
        )


async def index_knowledge_item(knowledge_item: dict[str, Any]) -> None:
    """Индексирование элемента знаний в Elasticsearch."""
    try:

        logger.debug(f"Индексирование элемента знаний: {knowledge_item}")


        if "id" not in knowledge_item:
            logger.error("Попытка индексации элемента без id")
            raise ValueError("Элемент знаний должен содержать id")

        item_id = knowledge_item["id"]


        response = await es_client.index(
            index=KNOWLEDGE_ITEMS_INDEX,
            id=item_id,
            document=knowledge_item,
            refresh=True
        )

        logger.info(f"Элемент знаний {item_id} проиндексирован, результат: {response}")


        try:
            await es_client.get(index=KNOWLEDGE_ITEMS_INDEX, id=item_id)
        except Exception as e:
            logger.warning(f"Не удалось проверить индексацию документа {item_id}: {e}")

    except Exception as e:
        logger.error(f"Ошибка при индексировании элемента знаний: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при индексировании элемента знаний: {str(e)}",
        )


async def delete_knowledge_item_from_index(knowledge_item_id: uuid.UUID) -> None:
    """Удаление элемента знаний из индекса Elasticsearch."""
    try:
        await es_client.delete(
            index=KNOWLEDGE_ITEMS_INDEX,
            id=knowledge_item_id,
        )
        logger.info(f"Элемент знаний {knowledge_item_id} удален из индекса")
    except NotFoundError:
        logger.warning(f"Элемент знаний {knowledge_item_id} не найден в индексе")
    except Exception as e:
        logger.error(f"Ошибка при удалении элемента знаний из индекса: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail="Ошибка при удалении элемента знаний из индекса",
        )


async def search_knowledge_items(
    query: str,
    category_id: uuid.UUID | None = None,
    tutor_id: uuid.UUID | None = None,
    from_: int = 0,
    size: int = 10,
) -> dict[str, Any]:
    """Поиск элементов знаний в Elasticsearch."""
    try:
        # Формируем запрос
        search_body = {
            "query": {
                "bool": {
                    "must": [
                        {
                            "multi_match": {
                                "query": query,
                                "fields": ["question^3", "answer^2"],
                                "type": "best_fields",
                                "tie_breaker": 0.3,
                            }
                        }
                    ],
                    "filter": []
                }
            },
            "from": from_,
            "size": size
        }


        if category_id:
            search_body["query"]["bool"]["filter"].append({"term": {"category_id": category_id}})

        if tutor_id:
            search_body["query"]["bool"]["filter"].append({"term": {"tutor_id": tutor_id}})


        try:
            response = await es_client.search(
                index=KNOWLEDGE_ITEMS_INDEX,
                body=search_body
            )


            if "hits" not in response:
                logger.error(f"В ответе Elasticsearch отсутствует hits: {response}")
                return {"total": 0, "results": []}


            hits = response["hits"]["hits"]
            total = response["hits"]["total"]["value"]

            results = []
            for hit in hits:
                source = hit["_source"]
                source_id = source["id"]
                source_tutor_id = source["tutor_id"]
                source_category_id = source["category_id"]


                if isinstance(source_id, str):
                    source_id = uuid.UUID(source_id)
                if isinstance(source_tutor_id, str):
                    source_tutor_id = uuid.UUID(source_tutor_id)
                if isinstance(source_category_id, str):
                    source_category_id = uuid.UUID(source_category_id)

                results.append(
                    {
                        "id": source_id,
                        "question": source["question"],
                        "answer": source["answer"],
                        "tutor_id": source_tutor_id,
                        "category_id": source_category_id,
                        "question_tags": source.get("question_tags", []),
                        "answer_tags": source.get("answer_tags", []),
                        "created_at": source["created_at"],
                        "updated_at": source["updated_at"],
                        "score": hit["_score"],
                    }
                )

            return {
                "total": total,
                "results": results,
            }

        except Exception as e:
            logger.error(f"Ошибка при выполнении поиска в Elasticsearch: {e}")

            return {"error": str(e), "raw_query": search_body}

    except Exception as e:
        logger.error(f"Ошибка при поиске элементов знаний: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при поиске элементов знаний: {str(e)}",
        )


async def close_elasticsearch() -> None:
    """Закрытие соединения с Elasticsearch."""
    await es_client.close()


async def reindex_all_knowledge_items(knowledge_items: list[dict[str, Any]]) -> None:
    """Переиндексация всех элементов знаний."""
    try:

        if await es_client.indices.exists(index=KNOWLEDGE_ITEMS_INDEX):
            await es_client.indices.delete(index=KNOWLEDGE_ITEMS_INDEX)
            logger.info(f"Индекс {KNOWLEDGE_ITEMS_INDEX} удален")


        await es_client.indices.create(
            index=KNOWLEDGE_ITEMS_INDEX,
            body=KNOWLEDGE_ITEMS_MAPPING,
        )
        logger.info(f"Индекс {KNOWLEDGE_ITEMS_INDEX} создан")


        if knowledge_items:

            bulk_operations = []
            for item in knowledge_items:

                bulk_operations.append({"index": {"_index": KNOWLEDGE_ITEMS_INDEX, "_id": item["id"]}})
                bulk_operations.append(item)

            if bulk_operations:

                await es_client.bulk(operations=bulk_operations, refresh=True)


            await es_client.indices.refresh(index=KNOWLEDGE_ITEMS_INDEX)

        logger.info(f"Переиндексация завершена, проиндексировано {len(knowledge_items)} элементов")
    except Exception as e:
        logger.error(f"Ошибка при переиндексации элементов знаний: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при переиндексации элементов знаний: {str(e)}",
        )


async def index_existing_knowledge_items(session) -> None:
    """Индексирование существующих элементов знаний."""
    try:

        stmt = select(KnowledgeItem)
        result = await session.execute(stmt)
        knowledge_items = result.scalars().all()


        knowledge_items_dict = [knowledge_item_to_dict(item) for item in knowledge_items]


        await reindex_all_knowledge_items(knowledge_items_dict)

        logger.info(f"Переиндексация завершена, проиндексировано {len(knowledge_items)} элементов")
    except Exception as e:
        logger.error(f"Ошибка при индексировании существующих элементов знаний: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при индексировании существующих элементов знаний: {str(e)}",
        )


def knowledge_item_to_dict(knowledge_item) -> dict[str, Any]:
    """Конвертация элемента знаний в dict для индексирования."""
    return {
        "id": knowledge_item.id,
        "question": knowledge_item.question,
        "answer": knowledge_item.answer,
        "tutor_id": knowledge_item.tutor_id,
        "category_id": knowledge_item.category_id,
        "question_tags": knowledge_item.question_tags,
        "answer_tags": knowledge_item.answer_tags,
        "created_at": knowledge_item.created_at.isoformat(),
        "updated_at": knowledge_item.updated_at.isoformat(),
    }
