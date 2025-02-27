from fastapi import APIRouter
from fastapi.responses import HTMLResponse

router = APIRouter(
    prefix="/test",
    tags=["Test"],
)


@router.get("/")
async def index():
    return HTMLResponse()
