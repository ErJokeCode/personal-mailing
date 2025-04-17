from fastapi import APIRouter, HTTPException, status, Depends
from sqlalchemy.ext.asyncio import AsyncSession
from typing import Annotated, Any, Dict, Union

from app.core.elasticsearch import search_knowledge_items, index_existing_knowledge_items
from app.core.session import db_session
from app.schemas.search import SearchError, SearchHighlight, SearchHit, SearchRequest, SearchResponse

router = APIRouter(tags=["search 🔍"])


@router.post("/", response_model=Union[SearchResponse, SearchError], status_code=status.HTTP_200_OK)
async def search(search_request: SearchRequest) -> Union[SearchResponse, Dict[str, Any]]:
    """
    Поиск элементов знаний по запросу.

    Поиск осуществляется по полям question и answer.
    Можно фильтровать результаты по категории и тьютору.
    """
    try:
        result = await search_knowledge_items(
            query=search_request.query,
            category_id=search_request.category_id,
            tutor_id=search_request.tutor_id,
            from_=search_request.from_,
            size=search_request.size,
        )
        
        if "error" in result:
            return SearchError(
                detail=f"Ошибка поиска: {result.get('error', 'Неизвестная ошибка')}",
                error_type="elasticsearch_error",
                raw_query=result.get("raw_query")
            )

        hits = []
        for item in result.get("results", []):
            hits.append(
                SearchHit(
                    id=item["id"],
                    question=item["question"],
                    answer=item["answer"],
                    tutor_id=item["tutor_id"],
                    category_id=item["category_id"],
                    created_at=item["created_at"],
                    updated_at=item["updated_at"],
                    score=item["score"],
                    highlight=None,
                )
            )

        return SearchResponse(
            total=result.get("total", 0),
            hits=hits,
            took=0,
        )
    except Exception as e:
        error_message = str(e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка поиска элементов знаний: {error_message}",
        ) from e

@router.post("/reindex", status_code=status.HTTP_200_OK)
async def reindex_knowledge_items(
    session: Annotated[AsyncSession, Depends(db_session.session_getter)],
) -> dict:
    """
    Переиндексирует все элементы знаний в Elasticsearch.
    
    Этот эндпоинт полезен, если возникли проблемы с поиском или
    после изменения структуры индекса.
    """
    try:
        await index_existing_knowledge_items(session)
        return {"status": "success", "message": "Все элементы знаний переиндексированы"}
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Ошибка при переиндексации: {str(e)}",
        ) from e
