from datetime import datetime
from io import BytesIO
from fastapi import UploadFile
from fastapi.exceptions import HTTPException
import pandas as pd
import logging
import asyncio

from models.upload_files.resp_upload import TypeFile

_log = logging.getLogger(__name__)


class ManagerFiles:
    __files: dict[str, list[pd.DataFrame]]

    def __init__(self):
        self.__files = {}

    async def add(self, file: UploadFile) -> str:
        time = str(datetime.now().strftime("%Y-%m-%d%H:%M:%S"))

        filename = file.filename
        if filename is None or (".xls" not in filename and ".xlsx" not in filename):
            _log.warning("File type not supported %s", filename)
            raise HTTPException(
                status_code=400, detail="File type not supported")

        try:
            self.__files[time] = []

            excel = pd.ExcelFile(BytesIO(await file.read()))
            sheet_names = excel.sheet_names

            for sheet_name in sheet_names:
                df = excel.parse(sheet_name)
                self.__files[time].append(df)

            asyncio.create_task(self.__delete_file(time))

            _log.info("Add file %s", filename)

            return time
        except Exception as e:
            _log.error(e)
            raise HTTPException(status_code=500, detail=str(e))

    def get_type(self, key: str) -> TypeFile:
        df = self.get(key)[0]

        if "Название онлайн-курса" in df.columns:
            return TypeFile.ONLINE_COURSE
        if "Дата рождения" in df.columns:
            return TypeFile.STUDENT
        else:
            return TypeFile.MODEUS

    def get(self, key: str) -> list[pd.DataFrame]:
        dfs = self.__files.get(key)

        if dfs is None:
            raise HTTPException(
                status_code=404,
                detail="File not found"
            )

        return dfs

    async def __delete_file(self, key: str, time_delete: int = 60):
        _log.info("Start timer for delete file %s", key)
        await asyncio.sleep(time_delete)
        self.__files.pop(key)
        _log.info("Delete file %s", key)


manager_files = ManagerFiles()
