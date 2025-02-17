from datetime import datetime
from io import BytesIO
from fastapi import UploadFile
from fastapi.exceptions import HTTPException
import pandas as pd
import logging
import asyncio

from models.upload_files.resp_upload import ResponseUpload, TypeFile
from upload_file.u_file import UFileModeus, UFileOnline, UFileStudent


_log = logging.getLogger(__name__)


class ManagerFiles:
    __files: dict[str, UFileStudent | UFileModeus | UFileOnline]

    def __init__(self):
        self.__files = {}

    async def add(self, file: UploadFile) -> str:
        time = str(datetime.now().strftime("%Y_%m_%d_%H_%M_%S"))

        filename = file.filename
        if filename is None or (".xls" not in filename and ".xlsx" not in filename):
            _log.warning("File type not supported %s", filename)
            raise HTTPException(
                status_code=400, detail="File type not supported")

        try:
            excel = pd.ExcelFile(BytesIO(await file.read()))
            sheet_names = excel.sheet_names

            sh_0 = sheet_names[0]
            df_0 = excel.parse(sh_0)
            type = self.__get_type(df_0)

            u_file: UFileStudent | UFileModeus | UFileOnline
            if type == TypeFile.STUDENT:
                u_file = UFileStudent(sh_0, df_0)
            elif type == TypeFile.MODEUS:
                u_file = UFileModeus(sh_0, df_0)
            else:
                u_file = UFileOnline(sh_0, df_0)
                for shn in sheet_names[1:]:
                    df = excel.parse(shn)
                    u_file.add_sheet(shn, df)

            self.__files[time] = u_file

            asyncio.create_task(self.__delete_file(time))

            _log.info("Add file %s", filename)

            return time
        except Exception as e:
            _log.error(e)
            raise HTTPException(status_code=500, detail=str(e))

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

    def get(self, key: str) -> UFileStudent | UFileModeus | UFileOnline:
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
        _log.info("Delete file %s", key)


manager_files = ManagerFiles()
