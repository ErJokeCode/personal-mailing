from datetime import datetime
import logging
from fastapi import HTTPException
import pandas as pd

from models.upload_files.resp_upload import InDB, TypeFile
from models.upload_files.filters import FilterModeus, FilterOnline, FilterOnlineStudent, FilterStudent
from pandas.core.groupby.generic import DataFrameGroupBy

from database import db
from models.student.db_student import Student, StudentInDB
from models.student.db_group import InfoGroupInStudent
from models.upload_files.resp_student import RespStudent
from models.upload_files.resp_modeus import RespModeus
from models.file.stucture import ColsExcel, ListExcel, StuctureExcel, StuctureExcelInDB
from models.student.db_online_course import InfoOnlineCourseInStudent
from models.student.db_subject import SubjectInStudent, TeamInSubjectInStudent

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
    def __init__(self, dfs: dict[str, pd.DataFrame]) -> None:
        self.__dfs: dict[str, pd.DataFrame] = dfs
        self.__structure: StuctureExcelInDB
        self.__grouped_dfs: dict[str, list] | None = None
        self.__keys: list[str] = []

    def add_structure(self, structure: StuctureExcelInDB) -> None:
        self.__structure = structure

    def get_base_structure(self) -> StuctureExcel:
        struc_excel = []
        for df in self.__dfs.values():
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

    def group_df(self, df: pd.DataFrame, groups: dict[int, list[str]], cols: dict[int, list[str]], index: int, max_level: int):
        n_group = groups.get(index)
        n_cols: list[str] = []
        for i in range(index, max_level + 1):
            cols_before = cols.get(i)
            if cols_before is not None:
                n_cols = [*n_cols, *cols_before]
            group__before = groups.get(i)
            if group__before is not None:
                n_cols = [*n_cols, *group__before]

        if n_group is None:
            res_df = df[n_cols]
            res_df = res_df.rename(
                columns={key: key.split("__")[-1] for key in n_cols})
            _log.info(res_df)
            return res_df.to_dict("records")

        if index == max_level:
            res_df = df[n_group]
            res_df = res_df.rename(
                columns={key: key.split("__")[-1] for key in n_group})
            _log.info(res_df)
            dict_res_df = res_df.to_dict("list")
            for k, v in dict_res_df.items():
                if isinstance(v, list):
                    set_v = set(v)
                    dict_res_df[k] = list(set_v)[0] if len(
                        set_v) == 1 else set_v
            return dict_res_df

        if n_cols is None:
            group_df = df.groupby(n_group)
        else:
            group_df = group_df = df.groupby(n_group)[n_cols]

        new_dfs = []
        for key_group, value_group in group_df:
            new_one = self.group_df(value_group, groups,
                                    cols, index + 1, max_level)

            d = {}
            i = 0
            for c in n_group:
                value_group = key_group[i]
                d[c.split("__")[-1]] = value_group
                i += 1
            name_dict = groups[index+1][0].split("__")[0]
            d[name_dict] = new_one

            new_dfs.append(d)

            # _log.info("%s %s", key_group, new_one_df)

        return new_dfs

    def get_info_with_group(self, start_index: int, lenght: int) -> dict:
        if self.__grouped_dfs is None:
            structure = self.__structure

            grouped_dfs: dict = {}

            i = 0
            for key, val in self.__dfs.items():
                new_df = self.__rename_df(val, structure, i)
                groups, cols, max_level = self.__create_group_level(
                    structure.lists[i])
                res = self.group_df(new_df, groups, cols, 0, max_level)
                grouped_dfs[key] = res
                i += 1

            self.__grouped_dfs = grouped_dfs

        res = {}
        for key, val in self.__grouped_dfs.items():
            res[key] = val[start_index:start_index + lenght]

        return res

    def __rename_df(
        self,
        df: pd.DataFrame,
        structure: StuctureExcelInDB,
        index: int
    ) -> pd.DataFrame:
        for cl in structure.lists[index].cols:
            if cl.split is not None:
                sp = df[cl.name_col_excel].str.split(
                    cl.split.by_split, expand=True)

                _log.info(sp)

                m = sp.shape[1]
                n = len(cl.split.name_col_db) - 1
                if m > n:
                    _log.info(sp.iloc[:, n:])
                    combined_col = sp.iloc[:, n:].fillna(
                        '').agg(' '.join, axis=1).str.strip()
                    _log.info(combined_col)

                df[[*cl.split.name_col_db[:-1]]
                   ] = sp[[i for i in range(n)]]
                df[cl.split.name_col_db[-1]] = combined_col

                if cl.is_key:
                    for split_name_col_db in cl.split.name_col_db:
                        self.__keys.append(split_name_col_db.split("__")[-1])

            if cl.name_col_db is None:
                df = df.drop(cl.name_col_excel, axis=1)
            else:
                df[cl.name_col_db] = df[cl.name_col_excel].copy()
                df = df.drop(cl.name_col_excel, axis=1)

                if cl.is_key:
                    self.__keys.append(cl.name_col_db.split("__")[-1])
        return df.copy()

    def __create_group_level(self, _list: ListExcel):
        groups: dict[int, list[str]] = {}
        cols: dict[int, list[str]] = {}
        max_level = 0

        for col in _list.cols:
            if col.group_by:
                if col.level_group not in groups:
                    groups[col.level_group] = []

                if col.name_col_db is not None:
                    groups[col.level_group].append(col.name_col_db)
                if col.split is not None and col.split.name_col_db != []:
                    groups[col.level_group] = [
                        *groups[col.level_group],
                        *col.split.name_col_db
                    ]
            else:
                if col.level_group not in cols:
                    cols[col.level_group] = []

                if col.name_col_db is not None:
                    cols[col.level_group].append(col.name_col_db)
                if col.split is not None and col.split.name_col_db != []:
                    cols[col.level_group] = [
                        *cols[col.level_group],
                        *col.split.name_col_db
                    ]
            max_level = max(max_level, col.level_group)

        groups = dict(sorted(groups.items()))
        cols = dict(sorted(cols.items()))

        return (groups, cols, max_level)

    def get_concat_dfs(self):
        return pd.concat(self.__dfs)[:100].to_dict("records")

    def __from_dfs_to_df(self, dfs: list[pd.DataFrame]) -> pd.DataFrame:
        if len(dfs) == 0:
            return pd.DataFrame()

        if len(dfs) == 1:
            self.__df = dfs[0].copy()
            return dfs[0]
        else:
            raise Exception("Not implemented")

    def save(self, ids: dict[str, list[int]]):
        if self.__grouped_dfs is None:
            raise Exception("Not found grouped dfs")

        for name_table, list_ids in ids.items():
            table = self.__grouped_dfs.get(name_table)

            if table is None:
                raise Exception("Not found sheet %s", name_table)

            if len(list_ids) == 1 and list_ids[0] == -1:
                list_ids = [i for i in range(len(table))]

            self.__save(table, list_ids)

    def __save(self, info: list[dict], ids: list[int]):
        for id in ids:
            row = info[id]

            filter = {key: row[key] for key in self.__keys}

            try:
                student = Student.model_validate(row)
                try:
                    db.student.update_one(
                        student, filter, {"$inc": {"version": 1}})
                except:
                    db.student.insert_one(student)
            except Exception as e:
                _log.info(e)
                self.__structure
                student_db = db.student.find_one(
                    **filter)

                if student_db is None:
                    raise HTTPException(
                        status_code=404,
                        detail="Student not found",
                    )

                d_student_db = student_db.model_dump()
                for key, val in row.items():
                    d_student_db[key] = val

                student = Student.model_validate(d_student_db)

                db.student.update_one(
                    student, filter, {"$inc": {"version": 1}})

            _log.info(student)

            # if len(student) != 0:
            #     student_model = Student.model_validate(student)
            #     _log.info(student_model)
            # if len(subject) != 0:
            #     subject_model = SubjectInStudent.model_validate(subject)
            #     _log.info(subject_model)
            # if len(online) != 0:
            #     online_model = InfoOnlineCourseInStudent.model_validate(online)
            #     _log.info(online_model)

    def __save_student(self, ids: list[int]) -> None:
        if self.__df is None:
            raise Exception("First get info")

        if len(ids) == 1 and ids[0] == -1:
            items = self.__df
        else:
            items = self.__df.iloc[ids]

        _log.info(items)
        student = {}
        for item in items.to_dict("records"):
            for key in item.keys():
                if "student__" in str(key):
                    new_key = str(key).replace("student__", "")
                    val = item[key]

                    if new_key == "status":
                        if item[key] == "Активный":
                            val = True
                        else:
                            val = False

                    elif new_key == "personal_number":
                        val = "0" * (8 - len(str(val))) + str(val)

                    elif new_key == "date_of_birth":
                        if isinstance(val, datetime):
                            val = val.strftime("%d.%m.%Y")

                    student[new_key] = val

            student_model = Student.model_validate(student)

            try:
                db.student.update_one(
                    student_model,
                    filter={"personal_number": student_model.personal_number},
                    query={"$inc": {"version": 1}},
                    upsert=True
                )
            except Exception as e:
                if str(e) == "Item not found":
                    db.student.insert_one(student_model)
                else:
                    _log.error(e)
