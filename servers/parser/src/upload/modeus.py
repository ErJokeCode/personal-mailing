from fastapi import HTTPException
import numpy as np
import pandas as pd
from config import WorkerDataBase
from src.schemas import StudentInDB, StudentInTeam, Subject, SubjectInStudent, Team, TeamInSubjectInStudent, TypeFormSubject, HistoryUploadFileInDB


def upload_modeus(hist: HistoryUploadFileInDB, worker_db: WorkerDataBase) -> dict[str, str]:
    try:
        link = hist.link
        df = pd.read_excel(link, sheet_name=0)
    except Exception as e:
        print(e)
        hist.status_upload = "Error file read"
        worker_db.history.update_one(hist, get_item=False)
        raise HTTPException(status_code=500, detail="File read error")

    fill_subjects(df, worker_db, hist)
    fill_students(df, worker_db, hist)
    
    return {"status": "success"}

def fill_subjects(df: pd.DataFrame, worker_db: WorkerDataBase, hist: HistoryUploadFileInDB):
    worker_db.subject.delete_many()
    try:
        data = df.groupby(["РМУП название", "МУП или УК", "Частный план название"])[["Студент", "Специальность", "Сотрудники", "Группа название"]]
    except Exception as e:
        print(e)
        hist.status_upload = f"Error work with file. Use stucture file example"
        worker_db.history.update_one(hist, get_item=False)        
    
    for key, info in data:
        teams: list[Team] = []
        for group, item in info.groupby(["Группа название"]):
            teachers = item["Сотрудники"].drop_duplicates().dropna().to_list()
            students = item["Студент"].drop_duplicates().to_list()
            
            students_in_team: list[StudentInTeam] = []
            for student in students:
                sername, name, patronymic = get_split_fio(student)
                
                st_team = StudentInTeam(
                    sername=sername,
                    name=name,
                    patronymic=patronymic
                    )
                
                student_db: StudentInDB | None = worker_db.student.get_one(
                    get_none=True,
                    surname = sername,
                    name = name,
                    patronymic = patronymic)
                
                if student_db != None:
                    st_team.id = student_db.id
                    
                students_in_team.append(st_team)
            
            teams.append(Team(
                name=group[0],
                teachers=teachers,
                students=students_in_team
            ))
            
        subject = Subject(
            full_name=key[0],
            name=key[1],
            teams=teams,
            form_education=get_form_edu(key[2]).value
        )
        
        worker_db.subject.insert_one(subject)

def fill_students(df: pd.DataFrame, worker_db: WorkerDataBase, hist: HistoryUploadFileInDB):
    try:
        data = df.groupby(["Студент", "Поток", "Специальность", "Профиль"])[["РМУП название", "Частный план название", "Группа название", "МУП или УК"]]
    except Exception as e:
        print(e)
        hist.status_upload = f"Error work with file. Use stucture file example"
        worker_db.history.update_one(hist, get_item=False)    
    
    for key, value in data:
        surname, name, patronymic = get_split_fio(key[0])
        direction_code, name_speciality = get_info_speciality(key[2])
        
        student: StudentInDB | None = worker_db.student.get_one(
            get_none=True,
            surname = surname,
            name = name,
            patronymic = patronymic)
        
        if student != None:
            subjects: list[SubjectInStudent] = []
            for names, item in value.groupby(["РМУП название", "МУП или УК", "Частный план название"])[["Группа название"]]:
                subject = create_subject_in_student(names, item, worker_db)
                subjects.append(subject)
            student.group.direction_code = direction_code
            student.group.name_speciality = name_speciality
            student.subjects = subjects
            
            st = worker_db.student.update_one(student)
        
        
def create_subject_in_student(names, item: pd.DataFrame, worker_db: WorkerDataBase) -> SubjectInStudent:
    try:
        full_name = names[0]
        name = names[1]
        form_education = get_form_edu(names[2]).value
        teams = item["Группа название"].drop_duplicates().tolist()
        
        subject_db = worker_db.subject.get_one(get_none=True, full_name=full_name, name=name, form_education=form_education)
        
        teams_sabject: list[TeamInSubjectInStudent] = []
        if subject_db != None:
            for team in subject_db.teams:
                team_in_subject_in_student = TeamInSubjectInStudent(
                    name=team.name, 
                    teachers=team.teachers
                )
                if team.name in teams:
                    teams_sabject.append(team_in_subject_in_student)

        subject = SubjectInStudent(
            full_name=full_name,
            name=name,
            teams=teams_sabject,
            form_education=form_education
        ) 
        return subject
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error create subject in student")
    
def get_form_edu(rmup: str) -> TypeFormSubject:
    try:
        rmup = rmup.lower()
        if rmup.find("смешанн") != -1:
            return TypeFormSubject.mixed
        elif rmup.find("онлайн") != -1:
            return TypeFormSubject.online
        elif rmup.find("традицион") != -1 or (rmup.find(")") == -1 and rmup.find("(") == -1):
            return TypeFormSubject.traditional
        else:
            return TypeFormSubject.other
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error get type form education")
        
        
def get_split_fio(fio: str) -> tuple[str, str, str]:
    try:
        fio = fio.split()  
        
        surname=fio[0] if len(fio) > 0 else ""
        name=fio[1] if len(fio) > 1 else ""
        patronymic=fio[2] if len(fio) > 2 else ""
        
        return (surname, name, patronymic)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error get split fio")

def get_info_speciality(speciality: str) -> tuple[str, str]:
    try:
        speciality = speciality.split()
        
        direction_code = speciality[0]
        name_speciality = " ".join(speciality[1:])
        
        return (direction_code, name_speciality)
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="Error get info speciality")
    