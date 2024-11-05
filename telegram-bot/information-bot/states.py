from aiogram.fsm.state import State, StatesGroup

class RegistrationStates(StatesGroup):
    WAITING_FOR_EMAIL = State()
    WAITING_FOR_STUDENT_ID = State()

class LKStates(StatesGroup):
    MAIN_MENU = State()
    WAITING_CHAT_WITH_CURATOR = State()
    COURSES = State()

class Info_teaching(StatesGroup):
    INFO = State()
    CARD = State()
    NUMBER_ROOM = State()
    NAVIGATOR = State()
    LK_URFU = State()
    ONLINE_COURSE = State()
    REITING = State()

class Onboarding(StatesGroup):
    ONE = State()
    TWO = State()
