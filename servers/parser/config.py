from database import Database, S3Client
import dotenv
import os
dotenv.load_dotenv()

AWS_ACCESS_KEY_ID = os.getenv("AWS_ACCESS_KEY_ID")
AWS_SECRET_ACCESS_KEY = os.getenv("AWS_SECRET_ACCESS_KEY")
URL_S3 = os.getenv("URL_S3")
BUCKET_NAME = os.getenv("BUCKET_NAME")
URL_S3_GET = os.getenv("URL_S3_GET")


DB = Database(host="parserdb", port=27017, 
              database="personal-mailing", 
              subject="subject", 
              student_collection="student", 
              course_info_collection="course_info", 
              student_not_found_modeus="not_found_modeus",
              dict_names="dict_names", 
              bot_onboard="bot_onboard", 
              bot_faq="bot_faq", 
              history_upload="history_upload")

s3_client = S3Client(
        access_key=AWS_ACCESS_KEY_ID,
        secret_key=AWS_SECRET_ACCESS_KEY,
        endpoint_url=URL_S3, 
        bucket_name=BUCKET_NAME,
        url_files=URL_S3_GET
    )
