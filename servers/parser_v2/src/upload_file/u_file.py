from datetime import datetime
import logging
import pandas as pd

from models.upload_files.resp_upload import InDB, TypeFile
from models.upload_files.filters import FilterModeus, FilterOnline, FilterOnlineStudent, FilterStudent
from pandas.core.groupby.generic import DataFrameGroupBy

from database import db
from models.student.db_student import Student
from models.student.db_group import InfoGroupInStudent
from models.upload_files.resp_student import RespStudent
from models.upload_files.resp_modeus import RespModeus
from models.file.stucture import ColsExcel, ListExcel, StuctureExcel, StuctureExcelInDB

_log = logging.getLogger(__name__)


class UFile:
    __type: TypeFile
    __dfs: dict[int | str, pd.DataFrame]

    def __init__(self, type: TypeFile):
        self.__type = type
        self.__dfs = {}

    @property
    def type(self) -> TypeFile:
        return self.__type

    @property
    def dfs(self):
        return self.__dfs.copy()

    def _add_sheet(self, name: int | str, df: pd.DataFrame) -> None:
        self.__dfs[name] = df.fillna("")

    def _get_sheet(self, name: int | str) -> pd.DataFrame | None:
        df = self.__dfs.get(name)

        if df is None:
            return None

        return df.copy()

    def _slice(self, df: pd.DataFrame, filter: FilterStudent | FilterModeus | FilterOnline | FilterOnlineStudent) -> pd.DataFrame:
        start = (filter.page - 1) * filter.lenght
        end = filter.page * filter.lenght

        return df.iloc[start:end]


class UFileStudent(UFile):
    __name: int | str

    def __init__(self, name: str | int, df: pd.DataFrame):
        super().__init__(TypeFile.STUDENT)
        self.__name = name
        new_df = self.__create_new_df(df)
        self._add_sheet(name, new_df)

    def filter(self, filter: FilterStudent) -> list[RespStudent] | None:
        df = self._get_sheet(self.__name)

        if df is None:
            return None
        name_cl = df.columns

        filter_df = df[
            (df[name_cl[1]].str.contains(filter.name_student)) &
            (df[name_cl[6]].str.contains(filter.number_group)) &
            (df[name_cl[7]].apply(str).str.contains(filter.number_course)) &
            (df[name_cl[10]].str.contains(filter.form_education))
        ]

        students = self._slice(
            filter_df, filter).reset_index().to_dict("records")

        md_students: list[RespStudent] = [
            RespStudent.model_validate(student) for student in students]

        return md_students

    def save(self, ids: list[int | str]) -> None:
        df = self.df
        int_ids = [int(id) for id in ids]

        if len(ids) == 1 and ids[0] == -1:
            students = df
        else:
            students = df.iloc[int_ids]

        for student in students.to_dict("records"):
            student_model = Student.model_validate(student)
            if student["in_db"] == True:
                _log.info("Update")
                db.student.update_auto(
                    student_model, "$set", Student.update_fields_file_student())
            else:
                _log.info("Insert")
                db.student.update_auto(student_model)

    def __create_new_df(self, df: pd.DataFrame) -> pd.DataFrame:
        new_df = pd.DataFrame()

        name_cl_df = df.columns

        numbers = df[name_cl_df[-1]
                     ].apply(lambda x: "0" * (8 - len(str(x))) + str(x))

        new_df["personal_number"] = numbers
        new_df["full_name"] = df[name_cl_df[1]]
        new_df[["name", "surname", "patronymic"]] = df[name_cl_df[1]
                                                       ].str.extract(r'(\w+)\s*(\w+)\s*(.*)')
        new_df["date_of_birth"] = df[name_cl_df[-2]
                                     ].apply(lambda x: x.strftime('%d.%m.%Y') if isinstance(x, datetime) else x)
        new_df[["number_group", "number_course"]
               ] = df[[name_cl_df[6], name_cl_df[5]]]
        new_df["status"] = df[name_cl_df[7]].apply(
            lambda x: True if x == "Активный" else False)
        new_df[["type_of_cost", "type_of_education"]
               ] = df[[name_cl_df[8], name_cl_df[9]]]
        new_df["in_db"] = numbers.apply(
            lambda x: db.student.is_in(personal_number=x))

        return new_df

    @property
    def df(self):
        return self._get_sheet(self.__name)


class UFileModeus(UFile):
    __name: int | str

    def __init__(self, name: str | int, df: pd.DataFrame):
        super().__init__(TypeFile.MODEUS)
        self.__name = name
        new_df = self.__create_new_df(df)
        self._add_sheet(name, new_df)

    def filter(self, filter: FilterModeus) -> list[RespModeus]:
        df = self._get_sheet(self.__name)

        if df is None:
            return []
        name_cl = df.columns

        filter_df = df[
            (df[name_cl[0]].str.contains(filter.name_student)) &
            (df[name_cl[3]].str.contains(filter.subject)) &
            (df[name_cl[4]].str.contains(filter.team))
        ]

        students = self._slice(
            filter_df, filter).reset_index().to_dict("records")

        md_students: list[RespModeus] = [
            RespModeus.model_validate(student) for student in students]

        return md_students

    def save(self, ids: list[int | str]) -> dict | None:
        df = self.df

        if df is None:
            _log.warning("Not found sheet %s", self.__name)
            return None

        df = df[df[df.columns[-1]] == InDB.IN_DB.value]
        int_ids = [int(id) for id in ids]

        if len(ids) == 1 and ids[0] == -1:
            students = df
        else:
            students = df.iloc[int_ids]

        subject_group: dict[str, dict[str, dict]] = students.groupby(
            [students.columns[3]]
        ).apply(self.__group_team).to_dict()

        for name_subject, teams in subject_group.items():
            for name_team, st_and_tch in teams.items():
                students = st_and_tch["students"]
                teachers = st_and_tch["teachers"]

        return subject_group

    def __create_new_df(self, df: pd.DataFrame) -> pd.DataFrame:
        new_df = pd.DataFrame()

        name_cl_df = df.columns

        new_df["full_name"] = df[name_cl_df[1]]
        new_df[["direction_code", "name_speciality"]
               ] = df[name_cl_df[3]].str.extract(r"(\d\d.\d\d.\d\d)\s*(.*)")
        new_df[["subject_name", "team_name", "teacher"]] = df[[
            name_cl_df[5], name_cl_df[8], name_cl_df[9]]]
        new_df["in_db"] = df[name_cl_df[1]].apply(
            lambda x: db.student.is_in(full_name=x).value)

        return new_df

    def __group_team(self, df: pd.DataFrame):
        d = df.groupby(
            df.columns[-3]).apply(self.__get_students_teachers).to_dict()

        return d

    def __get_students_teachers(self, df: pd.DataFrame) -> dict:
        students = df[df.columns[:3].to_list()].to_dict("records")
        teachers = set(df[df.columns[-2]])
        return {"students": students, "teachers": list(teachers)}

    @property
    def df(self):
        return self._get_sheet(self.__name)


class UFileOnline(UFile):
    __name: int | str
    __list_table: list[int]
    __cache_group_df: pd.DataFrame | None

    def __init__(self, name: str | int, df: pd.DataFrame):
        super().__init__(TypeFile.ONLINE_COURSE)
        self.__name = name
        self.__list_table = [-1]
        self.__cache_group_df = None
        self._add_sheet(name, df)

    @property
    def df(self):
        return self._get_sheet(self.__name)

    def add_sheet(self, name, df):
        return self._add_sheet(name, df)

    def filter_course(self, filter: FilterOnline) -> dict:
        df = self._get_sheet(self.__name)

        if df is None:
            return {}

        name_course = df.columns[0]

        filter_df = df[
            (df[name_course].str.contains(filter.oc_name))
        ]

        filter_df = filter_df[[name_course]]

        return self._slice(filter_df, filter).to_dict()

    def group_by(self, ids: list[int], filter: FilterOnlineStudent) -> dict:
        if ids == self.__list_table and self.__cache_group_df is not None:
            filter_df = self.__filter_student(self.__cache_group_df, filter)
            return self._slice(filter_df, filter).to_dict()

        self.__list_table = ids
        main_df = self._get_sheet(self.__name)

        if main_df is None:
            _log.warning("Not found sheet %s", self.__name)
            return {}

        link = main_df.columns[1]
        if len(ids) == 1 and ids[0] == -1:
            shns = main_df[link]
        else:
            shns = main_df.iloc[ids][link]

        concat_dfs = self.__concat_dfs(shns)

        group = concat_dfs.groupby(
            [concat_dfs.columns[4], *concat_dfs.columns[:4].to_list()])
        group_df = group.apply(lambda df: df.to_dict("records")).reset_index()
        self.__cache_group_df = group_df

        filter_df = self.__filter_student(group_df, filter)

        return self._slice(filter_df, filter).to_dict()

    def __filter_student(self, group_df: pd.DataFrame, filter: FilterOnlineStudent) -> pd.DataFrame:
        filter_df = group_df[
            (group_df[group_df.columns[1:3]].astype(str).agg(' '.join, axis=1).str.contains(filter.name_student)) &
            (group_df[group_df.columns[-2]].str.contains(filter.number_group))
        ]
        return filter_df

    def __concat_dfs(self, shns: pd.Series) -> pd.DataFrame:
        concat_dfs: pd.DataFrame = pd.DataFrame()

        for shn in shns:
            df = self._get_sheet(shn)

            if df is None:
                _log.warning("Not found sheet %s", shn)
                continue

            concat_dfs = pd.concat(
                [concat_dfs, df[df.columns[:11]]], ignore_index=True)

        return concat_dfs

    def save(self, ids: list[int]):
        pass
        # df = self.__cache_group_df.iloc[ids]


class UpFile:
    def __init__(self, dfs: list[pd.DataFrame]) -> None:
        self.__dfs = dfs
        self.__structure: StuctureExcelInDB

    def add_structure(self, structure: StuctureExcelInDB) -> None:
        self.__structure = structure

    def get_base_structure(self) -> StuctureExcel:
        struc_excel = []
        for df in self.__dfs:
            list_cols = []
            columns = df.columns.to_list()
            for i in range(len(columns)):
                m_cl = ColsExcel(
                    number_col=i,
                    name_col_excel=columns[i],
                )
                list_cols.append(m_cl)

            list_excel = ListExcel(
                cols=list_cols
            )

            struc_excel.append(list_excel)

        return StuctureExcel(
            lists=struc_excel
        )
