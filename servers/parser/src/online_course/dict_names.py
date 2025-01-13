from bson import ObjectId
from config import WorkerDataBase
from src.schemas import DictNames


def upload_dict_names(modeus:str, site_inf: str, file_course: str, worker_db: WorkerDataBase):
    dict = DictNames(
        modeus=modeus, 
        site_inf=site_inf, 
        file_course=file_course
    )
    
    course_db = worker_db.info_online_course.get_one(name = dict.site_inf)
    
    worker_db.dict_names.insert_one(dict)
    
    worker_db.subject.update_one(update_data = {"$set" : {"online_course_id" : ObjectId(course_db.id)}}, full_name = dict.modeus)
    
    return {"status" : "success"}
