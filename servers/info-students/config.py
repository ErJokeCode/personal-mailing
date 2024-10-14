from database import Database
import dotenv
import os
dotenv.load_dotenv()

TOKEN_BOT = os.getenv('TOKEN_BOT')
DB = Database(host="localhost", port=27017, database="personal-mailing", user_course_collection="user_course", 
              course_info_collection="course_info", user_collection="user", user_auth_collection="user_auth")