from fastapi import UploadFile
import pandas as pd

from config import DB


def update_auth(file: UploadFile):
    excel = pd.ExcelFile(file.file.read())
    collection = DB.get_collection("courses")

    sheet_name = excel.sheet_names[0]

    df = pd.read_excel(excel, sheet_name=sheet_name, engine="openpyxl")
    data = df.to_dict('records')

    return data