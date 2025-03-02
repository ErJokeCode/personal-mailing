from contextlib import asynccontextmanager
import uvicorn
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
import logging

from config import settings
from upload_file.router import router_upload
from test_pages.router import router as router_test
# from src.student.router import router_user
# from src.online_course.router import router_course
# from src.subject.router import router_subject
# from src.bot.router_onboard import router_bot_onboard
# from src.bot.router_faq import router_bot_faq


_log = logging.getLogger(__name__)


@asynccontextmanager
async def lifespan(app: FastAPI):
    _log.info("Start server")
    yield
    _log.info("Stop server")

app = FastAPI(
    lifespan=lifespan,
)

origins = [
    "http://localhost",
    "http://127.0.0.1"
]

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(router_upload)
app.include_router(router_test)
# app.include_router(router_user)
# app.include_router(router_course)
# app.include_router(router_subject)
# app.include_router(router_bot_onboard)
# app.include_router(router_bot_faq)

if __name__ == "__main__":
    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)
