from fastapi import APIRouter, UploadFile

from src.upload.parsers.auth import upload_student
from src.upload.parsers.excel_statement import update_report
from src.upload.parsers.parser_from_inf import parse_courses
from src.upload.parsers.choice_in_modeus import choice_in_modeus
from src.upload.parsers.dict_modeus_inf import dict_names


router_data = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)

@router_data.post("/site_urfu_inf")
async def site_urfu_inf():
    return parse_courses()

@router_data.post("/student")
async def upload_info_student(file: UploadFile):
    return upload_student(file)

@router_data.post("/choice_in_modeus")
async def post_choice_in_modeus(file: UploadFile):
    return choice_in_modeus(file)

@router_data.post("/report_online_course")
async def post_online_course_report(file: UploadFile):
    res = parse_courses()
    if res["status"] == "success":
        return update_report(file)
    else:
        return res

@router_data.post("/dict_names")
async def post_dict_modeus_inf(modeus:str = None, site_inf: str = None, file_course: str = None):
    return dict_names(modeus, site_inf, file_course)


