import dotenv
import os
dotenv.load_dotenv()

TOKEN = os.getenv('TOKEN')
URL_SERVER = os.getenv('URL_SERVER')
URL_REDIS = os.getenv('URL_REDIS')