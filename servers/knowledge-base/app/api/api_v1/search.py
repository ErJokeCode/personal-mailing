from fastapi import APIRouter, HTTPException, status

from app.core.elasticsearch import search_knowledge_items
from app.schemas.search import SearchHighlight, SearchHit, SearchRequest, SearchResponse

router = APIRouter(tags=["search 🔍"])


@router.post("/", response_model=SearchResponse)
async def search(search_request: SearchRequest) -> SearchResponse:
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

        # Преобразуем результаты Elasticsearch в формат ответа API
        hits = []
        for hit in result["hits"]["hits"]:
            source = hit["_source"]
            highlight = None

            if "highlight" in hit:
                highlight = SearchHighlight(
                    question=hit["highlight"].get("question"),
                    answer=hit["highlight"].get("answer"),
                )

            hits.append(
                SearchHit(
                    id=source["id"],
                    question=source["question"],
                    answer=source["answer"],
                    tutor_id=source["tutor_id"],
                    category_id=source["category_id"],
                    created_at=source["created_at"],
                    updated_at=source["updated_at"],
                    score=hit["_score"],
                    highlight=highlight,
                )
            )

        return SearchResponse(
            total=result["hits"]["total"]["value"],
            hits=hits,
            took=result["took"],
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error searching knowledge items: {str(e)}",
        ) from e
