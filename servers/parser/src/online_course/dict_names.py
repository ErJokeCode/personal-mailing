from bson import ObjectId
from config import WorkerDataBase
from src.schemas import DictNames, DictNamesInDB


def add_dict_names(modeus:str, site_inf: str, file_course: str, worker_db: WorkerDataBase) -> DictNamesInDB:
    dict = DictNames(
        modeus=modeus, 
        site_inf=site_inf, 
        file_course=file_course
    )
    
    dict_db = worker_db.dict_names.insert_one(dict)
    
    update_student_course_subject(dict_db, worker_db)
    
    return dict_db

def update_dict_names(dict: DictNamesInDB, worker_db: WorkerDataBase, upsert: bool = False) -> DictNamesInDB:
    dict_db = worker_db.dict_names.update_one(dict, upsert=upsert)

    update_student_course_subject(dict_db, worker_db)
    
    return dict_db
    

def update_student_course_subject(dict: DictNamesInDB, worker_db: WorkerDataBase):
    st_col = worker_db.student.get_collect()
    
    course_db = worker_db.info_online_course.get_one(name = dict.site_inf)
    
    st_col.update_many({
        "online_course.name" : dict.file_course
        }, 
        {"$set" : 
            {
                "online_course.$.university" : course_db.university,
                "online_course.$.date_start" : course_db.date_start, 
                "online_course.$.deadline" : course_db.deadline, 
                "online_course.$.info" : course_db.info
            }
        })
    
    worker_db.subject.update_one(update_data = {"$set" : {"online_course_id" : ObjectId(course_db.id)}}, full_name = dict.modeus)