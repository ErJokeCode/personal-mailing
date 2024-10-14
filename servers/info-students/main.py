import uvicorn
from fastapi import FastAPI

from src.upload.router import router_data
from src.user.router import router_user
from src.user_info.router import router_user_info
from src.course.router import router_course
from database import Database

app = FastAPI()

app.include_router(router=router_data)
app.include_router(router=router_user)
app.include_router(router=router_user_info)
app.include_router(router=router_course)

if __name__ == "__main__":
    uvicorn.run(app, host="localhost", port=8000)
