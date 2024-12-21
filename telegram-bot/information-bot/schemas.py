from typing import List, Optional
from pydantic import BaseModel, Field



class OnboardTopic(BaseModel):
    name: str
    text: str | None = None
    question: str | None = None
    answer: str | None = None

class OnboardSection(BaseModel):
    name: str
    callback_data: str
    topics: List[OnboardTopic]

class OnboardCourse(BaseModel):
    name: str
    is_main: bool = False
    is_active: bool = True
    sections: List[OnboardSection]
    


class FAQ(BaseModel):
    topic: str
    callback_data: str
    question: str
    answer: str
    
    
class FAQTopic(BaseModel):
    topic: str
    callback_data: str