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
from app.core.minio import init_minio
from app.core.session import db_session


@asynccontextmanager
async def lifespan(app: FastAPI) -> AsyncGenerator[None, None]:
    await init_elasticsearch()
    await init_minio()

    async with db_session.session_factory() as session:
        await index_existing_knowledge_items(session)

    yield
    
    await db_session.dispose()
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
