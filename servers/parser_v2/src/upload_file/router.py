import logging
from bson import ObjectId
from fastapi import APIRouter, HTTPException, UploadFile
from datetime import datetime

from models.upload_files.resp_upload import ResponseUpload, TypeFile
from models.upload_files.filters import FilterModeus, FilterOnline, FilterOnlineStudent, FilterStudent
from models.student.db_student import Student, StudentInDB
from models.student.db_group import InfoGroupInStudent
from models.upload_files.resp_student import RespStudent
from models.upload_files.resp_modeus import RespModeus
from models.file.stucture import ColsExcel, ListExcel, StuctureExcel, StuctureExcelInDB

from upload_file.u_file import UFileOnline, UFileModeus, UFileStudent
from upload_file.manager import manager_files, ManagerFiles
from database import db


_log = logging.getLogger(__name__)

router_upload = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)


@router_upload.post("/file")
async def upload_file(file: UploadFile) -> list[StuctureExcelInDB] | StuctureExcelInDB | StuctureExcel:
    _log.info("Upload file")
    key = await manager_files.add(file)

    structures = manager_files.get_structures(key)

    if structures is None:
        raise HTTPException(
            status_code=404,
            detail="File not found"
        )

    if len(structures) == 0:
        return manager_files.get(key).get_base_structure()
    elif len(structures) == 1:
        return structures[0]

    return structures


@router_upload.post("/student/{key}")
async def get_info_student(
    key: str,
    filter: FilterStudent,
) -> list[RespStudent]:

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
async def save_students(
    key: str,
    ids: list[int | str] = [-1],
):
    _log.info("Save info about student from file %s", key)

    file = manager_files.get(key)

    if isinstance(file, UFileStudent):
        file.save(ids)

        return {"status": "success"}
    else:
        raise HTTPException(
            status_code=404,
            detail="Could not determine file type"
        )


@router_upload.post("/modeus/{key}")
async def get_info_modeus(
    key: str,
    filter: FilterModeus,
) -> list[RespModeus]:

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


@router_upload.post("/modeus/{key}/save")
async def save_modeus(
    key: str,
    ids: list[int | str] = [-1],
):
    _log.info("Save modeus info from file %s", key)

    file = manager_files.get(key)

    if isinstance(file, UFileModeus):
        resp = file.save(ids)

        return resp
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


@router_upload.post("/structure")
async def create_structure(structure: StuctureExcel) -> StuctureExcelInDB:
    _log.info("Create structure")

    structure = StuctureExcel(
        type_file=TypeFile.STUDENT,
        lists=[
            ListExcel(
                cols=[
                    ColsExcel(
                        number_col=0,
                        name_col_excel="№"
                    ),
                    ColsExcel(
                        number_col=1,
                        name_col_excel="Фамилия, имя, отчество",
                        name_col_db=["full_name"]
                    ),
                    # ColsExcel(
                    #     number_col=1,
                    #     name_col_excel="Фамилия, имя, отчество",
                    #     name_col_db=["surname", "name", "patronymic"],
                    #     split=" "
                    # ),
                    ColsExcel(
                        number_col=2,
                        name_col_excel="Форм. факультет"
                    ),
                    ColsExcel(
                        number_col=3,
                        name_col_excel="Территориальное подразделение"
                    ),
                    ColsExcel(
                        number_col=4,
                        name_col_excel="Кафедра"
                    ),
                    ColsExcel(
                        number_col=5,
                        name_col_excel="Курс",
                        name_col_db=["number_course"]
                    ),
                    ColsExcel(
                        number_col=6,
                        name_col_excel="Группа",
                        name_col_db=["number_group"]
                    ),
                    ColsExcel(
                        number_col=7,
                        name_col_excel="Состояние",
                        name_col_db=["status"]
                    ),
                    ColsExcel(
                        number_col=8,
                        name_col_excel="Вид возм. затрат",
                        name_col_db=["type_of_cost"]
                    ),
                    ColsExcel(
                        number_col=9,
                        name_col_excel="Форма освоения",
                        name_col_db=["type_of_education"]
                    ),
                    ColsExcel(
                        number_col=10,
                        name_col_excel="Дата рождения",
                        name_col_db=["date_of_birth"]
                    ),
                    ColsExcel(
                        number_col=11,
                        name_col_excel="Личный №",
                        name_col_db=["personal_number"]
                    )
                ]
            ),

        ]
    )

    try:
        structure_db = db.structure.insert_one(structure, look_for=True)
    except Exception as e:
        _log.warning(e)
        raise HTTPException(
            status_code=400,
            detail=e.__str__()
        )

    if structure_db is None:
        raise HTTPException(
            status_code=404,
            detail="Error add structure"
        )

    return structure_db


@router_upload.put("/structure/{id_structure}")
async def update_structure(id_structure: str, structure: StuctureExcel) -> StuctureExcelInDB:
    _log.info("Update structure")
    try:
        structure_db = db.structure.update_one(
            structure,
            filter={"_id": ObjectId(id_structure)},
            query={"$inc": {"version": 1}},
            look_for=True)
    except Exception as e:
        _log.warning(e)
        raise HTTPException(
            status_code=400,
            detail=e.__str__()
        )

    if structure_db is None:
        raise HTTPException(
            status_code=404,
            detail="Error add structure"
        )

    return structure_db


@router_upload.delete("/structure/{id_structure}")
async def delete_structure(id_structure: str) -> dict[str, str]:
    _log.info("Delete structure")

    return db.structure.delete_one(id=id_structure)
