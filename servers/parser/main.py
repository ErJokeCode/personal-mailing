from fastapi.responses import HTMLResponse
import uvicorn
from fastapi import FastAPI, WebSocket, WebSocketDisconnect

from src.upload.router import router_data
from src.user.router import router_user
from src.course.router import router_course
from src.subject.router import router_subject
from src.bot.router_onboard import router_bot_onboard
from src.bot.router_faq import router_bot_faq


app = FastAPI(
    openapi_url=f"/openapi.json",
    docs_url=f"/docs",
    redoc_url=f"/redoc",
    root_path="/parser"
)

app.include_router(router=router_data)
app.include_router(router=router_user)
app.include_router(router=router_course)
app.include_router(router=router_subject)
app.include_router(router=router_bot_onboard)
app.include_router(router=router_bot_faq)



if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
