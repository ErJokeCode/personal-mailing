from contextlib import asynccontextmanager
from typing import AsyncGenerator

from fastapi import FastAPI
from fastapi.responses import ORJSONResponse

from app.api import router as api_router
from app.core.elasticsearch import (
    close_elasticsearch,
    index_existing_knowledge_items,
    init_elasticsearch,
)
from app.core.session import db_session


@asynccontextmanager
async def lifespan(app: FastAPI) -> AsyncGenerator[None, None]:
    # startup

    # Инициализация Elasticsearch
    await init_elasticsearch()

    # Индексация всех существующих элементов знаний
    async with db_session.session_factory() as session:
        await index_existing_knowledge_items(session)

    yield
    # shutdown

    # Закрываем пул соединений с базой данных и освобождаем все ресурсы,
    # чтобы предотвратить утечки соединений с БД.
    await db_session.dispose()

    # Закрытие соединения с Elasticsearch
    await close_elasticsearch()


app = FastAPI(
    debug=True,
    title="Knowledge base for IT university tutors 🔖",
    description="""
    Knowledge base for IT university tutors. 🚀
    You can:
    ## Categories
    * **Read categories**.
    * **Create categories**.
    * **Update categories**.
    * **Delete categories**.
    ## Knowledge items
    * **Read knowledge-items**.
    * **Create knowledge-items**.
    * **Update knowledge-items**.
    * **Delete knowledge-items**.
    ## Elasticsearch
    * **Search the knowledge base**.
    """,
    version="0.1.0",
    default_response_class=ORJSONResponse,
    lifespan=lifespan,
)

app.include_router(api_router, prefix="/api")
