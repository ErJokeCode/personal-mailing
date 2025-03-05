import logging
from pydantic import BaseModel, ConfigDict, computed_field
import re

from models.base.base import BaseModelInDB
from models.upload_files.resp_upload import TypeFile

colon = r"^(-?\d+)?:(-?\d+)?$"

_log = logging.getLogger(__name__)


class StuctureExcel(BaseModel):
    type_file: TypeFile | None = None
    lists: list["ListExcel"]

    @computed_field
    def hash(self) -> str:
        str_hash = ""
        for list in self.lists:
            int_hash = 0
            for col in list.cols:
                for ch in col.name_col_excel:
                    int_hash += ord(ch)
                int_hash += col.number_col
            str_hash += str(int_hash)

        return str_hash

    model_config = ConfigDict(use_enum_values=True)


class ListExcel(BaseModel):
    cols: list["ColsExcel"]


class Split(BaseModel):
    name_col_db: list[str]
    by_split: str

    @computed_field
    def count_col(self) -> int:
        return len(self.name_col_db)


class ColsExcel(BaseModel):
    number_col: int
    name_col_excel: str
    name_col_db: str | None = None
    split: Split | None = None
    is_key: bool = False
    group_by: bool = False
    level_group: int = 0


class StuctureExcelInDB(StuctureExcel, BaseModelInDB):
    pass


class ConstructorCols(BaseModel):
    number_col: str = ":"
    name_col_db: str
    split: Split | None = None
    is_key: bool = False
    group_by: bool = False
    level_group: int = 0

    def to_structure(self, base: list[ColsExcel]) -> list[ColsExcel]:
        cols = []
        _log.info(self.number_col)
        match = re.match(colon, self.number_col)
        if match:
            start_str = match.group(1)
            end_str = match.group(2)

            start = int(start_str) if start_str else 0
            end = int(end_str) if end_str else len(base)
        else:
            start, end = 0, len(base)

        for cl in base:
            if cl.number_col >= int(start) and cl.number_col <= int(end):
                cols.append(
                    ColsExcel(
                        number_col=cl.number_col,
                        name_col_excel=cl.name_col_excel,
                        name_col_db=self.name_col_db,
                        split=self.split,
                        is_key=self.is_key,
                        group_by=self.group_by,
                        level_group=self.level_group
                    )
                )
        return cols


class ConstructorList(BaseModel):
    index_list: str = ":"
    cols: list[ConstructorCols]

    def to_structure(self, base: list[ListExcel]) -> list[ListExcel]:
        _log.info(self.index_list.strip())
        match = re.match(colon, self.index_list.strip())
        if match:
            start_str = match.group(1)
            end_str = match.group(2)

            start = int(start_str) if start_str else 0
            end = int(end_str) if end_str else len(base)
        else:
            start, end = 0, len(base)

        indexes = [i for i in range(start, end)]

        lists: list[ListExcel] = []
        for i in range(len(base)):
            if i in indexes:
                cols = []
                for col in self.cols:
                    cols += col.to_structure(base[i].cols)
                lists.append(ListExcel(cols=cols))

        return lists


class ConstructorStuctureExcel(BaseModel):
    type_file: TypeFile
    lists: list[ConstructorList]

    def to_structure(self, base: StuctureExcel) -> StuctureExcel:
        lists: list[ListExcel] = []
        for _list in self.lists:
            lists += _list.to_structure(base.lists)

        _log.info(lists)

        return StuctureExcel(
            type_file=self.type_file,
            lists=lists
        )
