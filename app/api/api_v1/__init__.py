__all__ = ["router"]

from fastapi import APIRouter

from app.api.api_v1.categories import router as categories_router
from app.api.api_v1.knowledge_items import router as knowledge_items_router
from app.api.api_v1.search import router as search_router

router = APIRouter()
router.include_router(categories_router, prefix="/categories")
router.include_router(knowledge_items_router, prefix="/knowledge-items")
router.include_router(search_router, prefix="/search")
