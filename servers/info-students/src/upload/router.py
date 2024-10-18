from fastapi import APIRouter, UploadFile

from src.upload.parsers.auth import update_auth
from src.upload.parsers.excel_statement import update_report
from src.upload.parsers.parser_from_inf import parse_courses
from src.upload.parsers.choice_in_modeus import choice_in_modeus


router_data = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)

@router_data.post("/report_ok")
async def post_online_course_report(file: UploadFile):
    parse_courses()
    return update_report(file)

@router_data.post("/auth_student")
async def post_auth_student(file: UploadFile):
    return update_auth(file)

@router_data.post("/choice_in_modeus")
async def post_choice_in_modeus(file: UploadFile):
    return choice_in_modeus(file)