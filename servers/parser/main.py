from fastapi.responses import HTMLResponse
import uvicorn
from fastapi import FastAPI, WebSocket, WebSocketDisconnect

from src.upload.router import router_data
from src.user.router import router_user
from src.user_info.router import router_user_info
from src.course.router import router_course
from src.test_work_with_bot.router import router_bot
from database import Database

app = FastAPI()

app.include_router(router=router_data)
app.include_router(router=router_user)
#app.include_router(router=router_user_info)
app.include_router(router=router_course)
app.include_router(router=router_bot)

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
