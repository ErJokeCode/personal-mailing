__all__ = ["router"]

from fastapi import APIRouter

from app.api.api_v1.categories import router as categories_router
from app.api.api_v1.files import router as files_router
from app.api.api_v1.knowledge_items import router as knowledge_items_router
from app.api.api_v1.search import router as search_router
from app.api.api_v1.category_versions import router as category_versions_router
from app.api.api_v1.knowledge_item_versions import router as knowledge_item_versions_router

router = APIRouter()
router.include_router(categories_router, prefix="/categories")
router.include_router(files_router, prefix="/files")
router.include_router(knowledge_items_router, prefix="/knowledge-items")
router.include_router(search_router, prefix="/search")
router.include_router(category_versions_router, prefix="/category-versions")
router.include_router(knowledge_item_versions_router, prefix="/knowledge-item-versions")
