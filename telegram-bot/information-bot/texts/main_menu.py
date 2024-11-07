def create_text_online_course(info_course: dict, score: str) -> str:
    name = info_course['name']
    date_start = info_course['date_start']
    deadline = info_course['deadline']
    info = info_course['info']
    university = info_course['university']
    
    text = ""
    