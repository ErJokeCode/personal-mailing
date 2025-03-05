from datetime import datetime
from io import BytesIO
from fastapi import UploadFile
from fastapi.exceptions import HTTPException
import pandas as pd
import logging
import asyncio

from database import db
from models.upload_files.resp_upload import ResponseUpload, TypeFile
from models.file.stucture import StuctureExcelInDB
from upload_file.u_file import UFileModeus, UFileOnline, UFileStudent, UpFile


_log = logging.getLogger(__name__)


class ManagerFiles:
    __files: dict[str, UpFile]
    __structures: dict[str, list[StuctureExcelInDB]]

    def __init__(self):
        self.__files = {}
        self.__structures = {}

    async def add(self, file: UploadFile) -> str:
        # key
        time = str(datetime.now().strftime("%Y_%m_%d_%H_%M_%S"))

        # type file is true
        filename = file.filename
        if filename is None or (".xls" not in filename and ".xlsx" not in filename):
            _log.warning("File type not supported %s", filename)
            raise HTTPException(
                status_code=400, detail="File type not supported")

        try:
            excel = pd.ExcelFile(BytesIO(await file.read()))
            sheet_names = excel.sheet_names

            dfs = {}
            str_hash = ""
            for shn in sheet_names:
                df = excel.parse(shn)
                dfs[shn] = df

                columns = df.columns.to_list()
                int_hash = 0
                for i in range(len(columns)):
                    for ch in columns[i]:
                        int_hash += ord(ch)
                    int_hash += i
                str_hash += str(int_hash)

            structures = db.structure.find({"hash": str_hash})

            _log.info(structures)

            u_file = UpFile(dfs)

            self.__files[time] = u_file
            self.__structures[time] = structures

            asyncio.create_task(self.__delete_file(time))

            _log.info("Add file %s", filename)

            return time
        except Exception as e:
            _log.error(e)
            raise HTTPException(status_code=500, detail=str(e))

    def get_structures(self, key: str) -> list[StuctureExcelInDB] | None:
        return self.__structures.get(key)

    def __get_type(self, df: pd.DataFrame) -> TypeFile:
        if "Название онлайн-курса" in df.columns:
            return TypeFile.ONLINE_COURSE
        if "Дата рождения" in df.columns:
            return TypeFile.STUDENT
        else:
            return TypeFile.MODEUS

    def get_info(self, key: str) -> ResponseUpload:
        u_file = self.get(key)

        sh = u_file.df

        if sh is None:
            raise HTTPException(
                status_code=404,
                detail="Error get main sheet"
            )

        return ResponseUpload(
            type_file=u_file.type,
            key=key,
            count_rows=len(sh)
        )

    def get(self, key: str) -> UpFile:
        u_file = self.__files.get(key)

        if u_file is None:
            raise HTTPException(
                status_code=404,
                detail="File not found"
            )

        return u_file

    async def __delete_file(self, key: str, time_delete: int = 600) -> None:
        _log.info("Start timer for delete file %s", key)
        await asyncio.sleep(time_delete)
        self.__files.pop(key)
        self.__structures.pop(key)
        _log.info("Delete file %s", key)


manager_files = ManagerFiles()
