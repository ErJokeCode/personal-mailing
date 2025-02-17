import logging
from fastapi import APIRouter, HTTPException, UploadFile
from datetime import datetime

from models.upload_files.resp_upload import ResponseUpload, TypeFile
from models.upload_files.filters import FilterModeus, FilterOnline, FilterOnlineStudent, FilterStudent
from models.student.db_student import Student, StudentInDB
from models.student.db_group import InfoGroupInStudent
from models.upload_files.resp_student import ResponseStudent
from upload_file.u_file import UFileOnline, UFileModeus, UFileStudent
from upload_file.manager import manager_files, ManagerFiles
from database import db


_log = logging.getLogger(__name__)

router_upload = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)


@router_upload.post("/file")
async def upload_file(file: UploadFile) -> ResponseUpload:
    _log.info("Upload file")
    key = await manager_files.add(file)

    return manager_files.get_info(key)


@router_upload.post("/student/{key}")
async def get_info_student(
    key: str,
    filter: FilterStudent,
) -> ResponseStudent:

    _log.info("Get info about student from file %s", key)
    file = manager_files.get(key)

    if isinstance(file, UFileStudent):
        response = file.filter(filter)

        if response is None:
            raise HTTPException(
                status_code=404,
                detail="Sheet not found"
            )

        return response
    else:
        raise HTTPException(
            status_code=404,
            detail="Could not determine file type"
        )


@router_upload.post("/student/{key}/save")
async def get_info_student(
    key: str,
    ids: list[int | str],
):
    _log.info("Save info about student from file %s", key)

    file = manager_files.get(key)

    if isinstance(file, UFileStudent):
        response = file.save(ids)

        return response
    else:
        raise HTTPException(
            status_code=404,
            detail="Could not determine file type"
        )


@router_upload.post("/modeus/{key}")
async def get_info_modeus(
    key: str,
    filter: FilterModeus,
) -> dict:

    _log.info("Get info about subject from file %s", key)
    file = manager_files.get(key)

    if isinstance(file, UFileModeus):
        response = file.filter(filter)

        if response is None:
            raise HTTPException(
                status_code=404,
                detail="Sheet not found"
            )

        return response
    else:
        raise HTTPException(
            status_code=404,
            detail="Could not determine file type"
        )


@router_upload.post("/online/{key}")
async def get_info_online(
    key: str,
    filter: FilterOnline,
) -> dict:

    _log.info("Get info about online course from file %s", key)
    file = manager_files.get(key)

    if isinstance(file, UFileOnline):
        response = file.filter_course(filter)

        if response is None:
            raise HTTPException(
                status_code=404,
                detail="Sheet not found"
            )

        return response
    else:
        raise HTTPException(
            status_code=404,
            detail="Could not determine file type"
        )


@router_upload.post("/online/{key}/group_by")
async def get_info_online_student(
    key: str,
    filter: FilterOnlineStudent,
    ids_table: list[int] = [-1],
) -> dict:

    _log.info("Get info about student in online course from file %s", key)
    file = manager_files.get(key)

    if isinstance(file, UFileOnline):
        response = file.group_by(ids_table, filter)

        return response
    else:
        raise HTTPException(
            status_code=404,
            detail="Could not determine file type"
        )


@router_upload.post("/test")
async def get_info_online_stsedent() -> StudentInDB:
    st = Student(
        personal_number="test",
        name="tessst",
        surname="test",
        date_of_birth="test",
        group=InfoGroupInStudent(
            number="test",
            number_course=1
        )
    )
    st_db = db.student.insert_auto(st)
    if st_db is None:
        raise HTTPException(
            status_code=404,
            detail="Error add student"
        )
    return st_db


# @router_upload.post("/online/{key}/save")
# async def save_online_student(
#     key: str,
#     ids_student: list[int] = [-1],
# ) -> dict:

#     _log.info("Save info about student in online course from file %s", key)
#     file = manager_files.get(key)

#     if isinstance(file, UFileOnline):
#         response = file.group_by(ids_student, filter)

#         return response
#     else:
#         raise HTTPException(
#             status_code=404,
#             detail="Could not determine file type"
#         )
