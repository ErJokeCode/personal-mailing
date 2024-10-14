from database import Database


DB = Database(host="localhost", port=27017, database="personal-mailing", user_course_collection="user_course", 
              course_info_collection="course_info", user_collection="user", user_auth_collection="user_auth")