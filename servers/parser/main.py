import uvicorn
from fastapi import FastAPI

from src.upload.router import router_data
from src.student.router import router_user
from src.online_course.router import router_course
from src.subject.router import router_subject
from src.bot.router_onboard import router_bot_onboard
from src.bot.router_faq import router_bot_faq


app = FastAPI(
    openapi_url=f"/openapi.json",
    docs_url=f"/docs",
    redoc_url=f"/redoc",
    root_path="/parser"
)


@app.get("/ping")
async def ping():
    return "ok"

app.include_router(router_data)
app.include_router(router_user)
app.include_router(router_course)
app.include_router(router_subject)
app.include_router(router_bot_onboard)
app.include_router(router_bot_faq)


if __name__ == "__main__":
    uvicorn.run("main:app", host="0.0.0.0", port=8000, workers=5)
