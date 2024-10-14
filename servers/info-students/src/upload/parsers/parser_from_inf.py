import requests
from bs4 import BeautifulSoup
from motor.motor_asyncio import AsyncIOMotorClient
import asyncio

from config import DB

def parse_courses():
    collection = DB.get_course_info_collection()
    collection.delete_many({})
# URL страницы
    url = "https://inf-online.urfu.ru/ru/onlain-kursy/#urfu"

    # Получаем содержимое страницы
    response = requests.get(url)
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
                    course = {"name" : name, "date" : date, "university" : university, "info" : info}
                    courses.append(course)

    
    collection.insert_many(courses)