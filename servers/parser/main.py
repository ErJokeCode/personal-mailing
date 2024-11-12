from fastapi.responses import HTMLResponse
import uvicorn
from fastapi import FastAPI, WebSocket, WebSocketDisconnect

from src.upload.router import router_data
from src.user.router import router_user
from src.course.router import router_course
from src.subject.router import router_subject
from database import Database

app = FastAPI()

app.include_router(router=router_data)
app.include_router(router=router_user)
app.include_router(router=router_course)
app.include_router(router=router_subject)

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
