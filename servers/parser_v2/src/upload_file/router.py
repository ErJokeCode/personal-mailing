import logging
from fastapi import APIRouter, UploadFile
from datetime import datetime

from models.upload_files.resp_upload import ResponseUpload
from upload_file.upload import manager_files, ManagerFiles


_log = logging.getLogger(__name__)

router_upload = APIRouter(
    prefix="/upload",
    tags=["Upload Files"],
)


@router_upload.post("/file")
async def upload_file(file: UploadFile) -> ResponseUpload:
    _log.info("Upload file")
    key = await manager_files.add(file)

    return ResponseUpload(
        type_file=manager_files.get_type(key),
        key=key,
        count_rows=len(manager_files.get(key)[0])
    )


@router_upload.get("/{key}")
async def get_info_from_file(
    key: str,
    filter: str | None = None,
    lenght: int = 10,
    page: int = 1
) -> dict:

    _log.info("Get info from file %s", key)
    dfs = manager_files.get(key)
    df = dfs[0]

    start = (page - 1) * 10
    end = page * 10

    return df.iloc[start:end].fillna("").to_dict(orient="index")
