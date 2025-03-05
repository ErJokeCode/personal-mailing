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
from models.file.stucture import ColsExcel, ConstructorStuctureExcel, ListExcel, Split, StuctureExcel, StuctureExcelInDB

from upload_file.u_file import UFileOnline, UFileModeus, UFileStudent
from upload_file.manager import manager_files, ManagerFiles
from database import db


_log = logging.getLogger(__name__)

router_upload = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)


@router_upload.post("/file")
async def upload_file(file: UploadFile) -> dict[str, list[StuctureExcelInDB] | StuctureExcel | str]:
    _log.info("Upload file")
    key = await manager_files.add(file)

    structures = manager_files.get_structures(key)

    if structures is None:
        raise HTTPException(
            status_code=404,
            detail="File not found"
        )

    if len(structures) == 0:
        return {"key": key, "structures": manager_files.get(key).get_base_structure()}

    return {"key": key, "structures": structures}


@router_upload.post("/file/{key}")
async def get_info_file(key: str, id_structure: str, start_index: int = 0, lenght: int = 10) -> dict:
    _log.info("Get info about file %s", key)

    file = manager_files.get(key)
    structure = db.structure.find_one(id=id_structure)

    if structure is None:
        raise HTTPException(
            status_code=404,
            detail="Structure not found"
        )

    file.add_structure(structure)

    return file.get_info_with_group(start_index, lenght)


@router_upload.post("/file/{key}/add_structure")
async def add_structure(key: str, structure: ConstructorStuctureExcel) -> StuctureExcelInDB:
    file = manager_files.get(key)
    struct_from_file = file.get_base_structure()

    new_struct = structure.to_structure(struct_from_file)
    hash = new_struct.hash

    structure_db = db.structure.find_one(hash=hash)

    if structure_db is None:
        structure_db = db.structure.insert_one(new_struct, look_for=True)
    else:
        structure_db = db.structure.update_one(
            new_struct,
            filter={"_id": ObjectId(structure_db.id)},
            query={"$inc": {"version": 1}},
            look_for=True
        )

    if structure_db is None:
        raise HTTPException(
            status_code=404,
            detail="Error add structure"
        )

    return structure_db


@router_upload.post("/file/{key}/save")
async def save_file(key: str, ids: list[int] = [-1]):
    _log.info("Get info about file %s", key)

    file = manager_files.get(key)

    return file.save(ids)
