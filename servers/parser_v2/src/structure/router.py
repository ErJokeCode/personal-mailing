import logging
from bson import ObjectId
from fastapi import APIRouter, HTTPException

from database import db
from models.file.stucture import ColsExcel, ListExcel, Split, StuctureExcel, StuctureExcelInDB
from models.upload_files.resp_upload import TypeFile
from models.student.db_student import Student

_log = logging.getLogger(__name__)

router = APIRouter(
    prefix="/structure",
    tags=["Structure"],
)


@router.get("/")
async def get_structures(start: int = 0, limit: int = -1) -> list[StuctureExcelInDB]:
    _log.info("Get structures")

    return db.structure.find(start=start, limit=limit)


@router.get("/names_col_db")
async def get_names_col_db() -> list[str]:
    _log.info("Get names col db")

    return Student.get_names_col_db()


@router.get("/{id_structure}")
async def get_structure(id_structure: str) -> StuctureExcelInDB:
    _log.info("Get structure")

    res = db.structure.find_one(id=id_structure)

    if res is None:
        raise HTTPException(
            status_code=404,
            detail="Structure not found"
        )

    return res


@router.post("/create_base")
async def create_base_structure() -> dict:
    _log.info("Create base structure")

    structure = StuctureExcel(
        type_file=TypeFile.STUDENT,
        lists=[
            ListExcel(
                cols=[
                    ColsExcel(
                        number_col=0,
                        name_col_excel="№"
                    ),
                    ColsExcel(
                        number_col=1,
                        name_col_excel="Фамилия, имя, отчество",
                        name_col_db="student__full_name",
                        split=Split(
                            name_col_db=["student__surname",
                                         "student__name", "student__patronymic"],
                            by_split=" "
                        ),
                        is_key=True
                    ),
                    ColsExcel(
                        number_col=2,
                        name_col_excel="Форм. факультет"
                    ),
                    ColsExcel(
                        number_col=3,
                        name_col_excel="Территориальное подразделение"
                    ),
                    ColsExcel(
                        number_col=4,
                        name_col_excel="Кафедра"
                    ),
                    ColsExcel(
                        number_col=5,
                        name_col_excel="Курс",
                        name_col_db="student__number_course",
                        is_key=True
                    ),
                    ColsExcel(
                        number_col=6,
                        name_col_excel="Группа",
                        name_col_db="student__number_group"
                    ),
                    ColsExcel(
                        number_col=7,
                        name_col_excel="Состояние",
                        name_col_db="student__status"
                    ),
                    ColsExcel(
                        number_col=8,
                        name_col_excel="Вид возм. затрат",
                        name_col_db="student__type_of_cost"
                    ),
                    ColsExcel(
                        number_col=9,
                        name_col_excel="Форма освоения",
                        name_col_db="student__type_of_education"
                    ),
                    ColsExcel(
                        number_col=10,
                        name_col_excel="Дата рождения",
                        name_col_db="student__date_of_birth"
                    ),
                    ColsExcel(
                        number_col=11,
                        name_col_excel="Личный №",
                        name_col_db="student__personal_number"
                    )
                ]
            ),

        ]
    )

    structure_modeus = StuctureExcel(
        type_file=TypeFile.MODEUS,
        lists=[
            ListExcel(
                cols=[
                    ColsExcel(
                        number_col=0,
                        name_col_excel="Учебный год"
                    ),
                    ColsExcel(
                        number_col=1,
                        name_col_excel="Студент",
                        name_col_db="student__full_name",
                        split=Split(
                            name_col_db=["student__surname",
                                         "student__name",
                                         "student__patronymic"],
                            by_split=" "
                        ),
                        is_key=True,
                        group_by=True
                    ),
                    ColsExcel(
                        number_col=2,
                        name_col_excel="Поток"
                    ),
                    ColsExcel(
                        number_col=3,
                        name_col_excel="Специальность",
                        split=Split(
                            name_col_db=["student__direction_code",
                                         "student__name_speciality"],
                            by_split=" "
                        ),
                        group_by=True
                    ),
                    ColsExcel(
                        number_col=4,
                        name_col_excel="Профиль"
                    ),
                    ColsExcel(
                        number_col=5,
                        name_col_excel="РМУП название",
                        name_col_db="subject__full_name",
                        group_by=True,
                        level_group=1
                    ),
                    ColsExcel(
                        number_col=6,
                        name_col_excel="Unnamed: 6"
                    ),
                    ColsExcel(
                        number_col=7,
                        name_col_excel="Частный план название"
                    ),
                    ColsExcel(
                        number_col=8,
                        name_col_excel="Группа название",
                        name_col_db="subject__team__name",
                        group_by=True,
                        level_group=2
                    ),
                    ColsExcel(
                        number_col=9,
                        name_col_excel="Сотрудники",
                        name_col_db="subject__team__teachers",
                        group_by=True,
                        level_group=3
                    ),
                    ColsExcel(
                        number_col=10,
                        name_col_excel="МУП или УК",
                        name_col_db="subject__name",
                        group_by=True,
                        level_group=1
                    )
                ]
            ),

        ]
    )

    try:
        structure_db = db.structure.insert_one(structure, look_for=True)
        st = db.structure.insert_one(structure_modeus, look_for=True)
    except Exception as e:
        _log.warning(e)
        raise HTTPException(
            status_code=400,
            detail=e.__str__()
        )

    if structure_db is None:
        raise HTTPException(
            status_code=404,
            detail="Error add structure"
        )

    return {"status": "success"}


@router.post("/")
async def create_structure(structure: StuctureExcel) -> StuctureExcelInDB:
    _log.info("Create structure")

    try:
        structure_db = db.structure.insert_one(structure, look_for=True)
    except Exception as e:
        _log.warning(e)
        raise HTTPException(
            status_code=400,
            detail=e.__str__()
        )

    if structure_db is None:
        raise HTTPException(
            status_code=404,
            detail="Error add structure"
        )

    return structure_db


@router.put("/{id_structure}")
async def update_structure(id_structure: str, structure: StuctureExcel) -> StuctureExcelInDB:
    _log.info("Update structure")
    try:
        structure_db = db.structure.update_one(
            structure,
            filter={"_id": ObjectId(id_structure)},
            query={"$inc": {"version": 1}},
            look_for=True)
    except Exception as e:
        _log.warning(e)
        raise HTTPException(
            status_code=400,
            detail=e.__str__()
        )

    if structure_db is None:
        raise HTTPException(
            status_code=404,
            detail="Error add structure"
        )

    return structure_db


@router.delete("/{id_structure}")
async def delete_structure(id_structure: str) -> dict[str, str]:
    _log.info("Delete structure")

    return db.structure.delete_one(id=id_structure)
