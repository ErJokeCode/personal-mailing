from database import Database
import dotenv
import os
dotenv.load_dotenv()

TOKEN_BOT = os.getenv('TOKEN_BOT')
DB = Database(host="parserdb", port=27017, 
              database="personal-mailing", 
              subject="subject", 
              student_collection="student", 
              course_info_collection="course_info", 
              student_not_found_modeus="not_found_modeus",
              dict_names="dict_names", 
              bot_onboard="bot_onboard", 
              bot_faq="bot_faq")
