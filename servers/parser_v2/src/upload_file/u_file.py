import logging
import pandas as pd

from models.upload_files.resp_upload import TypeFile
from models.upload_files.filters import FilterModeus, FilterOnline, FilterOnlineStudent, FilterStudent
from pandas.core.groupby.generic import DataFrameGroupBy

from models.upload_files.resp_student import ResponseStudent
from database import db
from models.student.db_student import Student
from models.student.db_group import InfoGroupInStudent

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
        self._add_sheet(name, df)

    def filter(self, filter: FilterStudent) -> ResponseStudent | None:
        df = self._get_sheet(self.__name)

        if df is None:
            return None

        filter_df = df[
            (df['Фамилия, имя, отчество'].str.contains(filter.name_student)) &
            (df['Группа'].str.contains(filter.number_group)) &
            (df['Курс'].apply(str).str.contains(filter.number_course)) &
            (df['Форма освоения'].str.contains(filter.form_education))
        ]

        return self.__to_model_resp(filter_df, filter)

    def save(self, ids: list[int | str]):
        df = self.df
        name_cl = df.columns

        for id in ids:
            student_sr = df.iloc[int(id)]
            student_model = self.__to_model_student(student_sr, name_cl)
            db.student.insert_auto(
                student_model)

    @property
    def df(self):
        return self._get_sheet(self.__name)

    def __to_model_resp(self, df: pd.DataFrame, filter: FilterStudent) -> ResponseStudent:
        sl_df = self._slice(df, filter)
        cl = sl_df.columns

        in_db = pd.Series(False, index=sl_df.index)
        for i, number in sl_df[cl[11]].items():
            new_number = "0" * (8 - len(str(number))) + str(number)
            sl_df[cl[11]][i] = new_number
            in_db[i] = db.student.find_one(personal_number=new_number) != None
        sl_df["in_db"] = in_db
        cl = sl_df.columns

        dict_sl_df = sl_df.to_dict()

        model = ResponseStudent(
            name=dict_sl_df[cl[1]],
            cafedra=dict_sl_df[cl[4]],
            number_course=dict_sl_df[cl[5]],
            group=dict_sl_df[cl[6]],
            status=dict_sl_df[cl[7]],
            type_of_cost=dict_sl_df[cl[8]],
            type_of_education=dict_sl_df[cl[9]],
            date_birth=dict_sl_df[cl[10]],
            personal_number=dict_sl_df[cl[11]],
            in_db=dict_sl_df[cl[12]],
        )
        return model

    def __to_model_student(self, student_sr: pd.Series, name_cl: list[str]):

        spl_fio = str(student_sr[name_cl[1]]).split()
        surname = spl_fio[0] if len(spl_fio) > 0 else ""
        name = spl_fio[1] if len(spl_fio) > 1 else ""
        patronymic = " ".join(spl_fio[2:]) if len(spl_fio) > 2 else ""
        number = student_sr[-1]

        student_model = Student(
            personal_number="0" * (8 - len(str(number))) + str(number),
            surname=surname,
            name=name,
            patronymic=patronymic,
            date_of_birth=student_sr[-2],
            group=InfoGroupInStudent(
                number=student_sr[6],
                number_course=student_sr[5]
            ),
            status=str(student_sr[7]).lower() == "активный",
            type_of_cost=student_sr[8],
            type_of_education=student_sr[9]
        )

        return student_model


class UFileModeus(UFile):
    __name: int | str

    def __init__(self, name: str | int, df: pd.DataFrame):
        super().__init__(TypeFile.MODEUS)
        self.__name = name
        self._add_sheet(name, df)

    def filter(self, filter: FilterModeus) -> dict:
        df = self._get_sheet(self.__name)

        if df is None:
            return {}

        filter_df = df[
            (df['Студент'].str.contains(filter.name_student)) &
            (df['РМУП название'].str.contains(filter.subject)) &
            (df['Группа название'].str.contains(filter.team))
        ]

        return self._slice(filter_df, filter).to_dict()

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
