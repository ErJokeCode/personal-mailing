from fastapi import APIRouter, Request
from fastapi.responses import HTMLResponse
from fastapi.templating import Jinja2Templates

router = APIRouter(
    prefix="/test",
    tags=["Test"],
)

templates = Jinja2Templates(directory="src/templates")


@router.get("/")
async def index(request: Request):
    return templates.TemplateResponse(
        request=request, name="index.html"
    )
