from aiogram import Bot
from fastapi import FastAPI
import uvicorn

from src.chat_student import router_chat_student
from src.notifications import router_send

app = FastAPI()
app.include_router(router_send)
app.include_router(router_chat_student)

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
    
