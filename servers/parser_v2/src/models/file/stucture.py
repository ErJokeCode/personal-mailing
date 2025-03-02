from pydantic import BaseModel, ConfigDict, computed_field

from models.base.base import BaseModelInDB
from models.upload_files.resp_upload import TypeFile


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


class StuctureExcelInDB(StuctureExcel, BaseModelInDB):
    pass
