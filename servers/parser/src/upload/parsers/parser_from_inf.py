import requests
from bs4 import BeautifulSoup
from motor.motor_asyncio import AsyncIOMotorClient
import asyncio

from config import DB
from src.schemas import OnlineCourseInDB

def parse_courses():

    collection = DB.get_course_info_collection()
    collection.delete_many({})

    url = "https://inf-online.urfu.ru/ru/onlain-kursy/#urfu"

    # Получаем содержимое страницы
    try:
        response = requests.get(url)
    except:
        return {"status" : "Not connect to urfu"}
    
    if response.status_code != 200:
        raise {"status" : "Error connect to urfu"}
    else:
        soup = BeautifulSoup(response.content, 'html.parser')

        # Находим таблицы с курсами
        tables = soup.find_all('table')

        # Список для хранения курсов
        courses = []

        # Проходим по каждой таблице
        for table in tables[1:]:
            rows = table.find_all('tr')
            university = ""
            info = ""
            for row in rows: 
                cols = row.find_all('td')
                if(len(cols) > 0):
                    text_col_0 = cols[0].text
                    if(text_col_0 == '№'):
                        university = cols[1].text.replace("\xa0", " ").rstrip()
                    elif(text_col_0.rstrip().isdigit() == False):
                        university = cols[0].text.replace("\xa0", " ").rstrip()
                    elif(text_col_0.rstrip().isdigit() == True):
                        name_and_date = str(cols[1]).split('<br/>')               
                        date = ""
                        if(len(name_and_date) == 1):
                            end_index = name_and_date[0].find("</")
                            start_index = name_and_date[0].find("p>")
                            name = name_and_date[0][start_index + 2:end_index]
                        elif(len(name_and_date) == 2):
                            end_index = name_and_date[0].rfind(">")
                            name = name_and_date[0][end_index + 1:]
                            start_index = name_and_date[1].find("<")
                            date = name_and_date[1][:start_index]
                        if(len(cols) == 3):
                            info = cols[2].text.replace("\xa0", " ")

                        split_university = university.split()
                        if split_university[0] == "Курсы" or split_university[0] == "курсы":
                            university = " ".join(split_university[1:])
                        course = OnlineCourseInDB(name=name, date_start=date, deadline=None, university=university, info=info).model_dump(by_alias=True, exclude=["id"])

                        res = collection.update_one(
                            {"name" : course["name"], "university" : course["university"]}, 
                            {"$set": {"date_start": course["date_start"], "deadline" : course["deadline"], "info" : course["info"]}}, 
                            upsert=True
                        )
            
        return {"status" : "success"}
        