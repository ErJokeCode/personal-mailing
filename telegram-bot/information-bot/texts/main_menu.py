def create_text_online_course(info_course: dict, score: str) -> str:
    name = info_course['name']
    date_start = info_course['date_start']
    deadline = info_course['deadline']
    info = info_course['info']
    university = info_course['university']
    print(score)
    
    text = f"{name}\n\nКурс проводит {university}\n\n"

    if date_start != None:
        text += f"{date_start}\n\n"

    if deadline != None:
        text += f"{deadline}\n\n"
    
    if info != None:
        text += f"Информация для записи на курс:\n{info}\n\n"

    if score == "Нет на курсе":
        text += "Тебя нет на курсе! Перейди по ссылке выше или напиши куратору, он тебе обязательно поможет!"
    elif score == "Not column":
        text += "К сожалению пока информаци о итоговых баллов нет("
    else:
        if score.isdigit():
            score = int(score)
            if score < 40:
                text += f"У тебя {plural(score, "балл", "балла", "баллов")}. Молодец! Для получения зачета надо набрать минимум 40 баллов"
            elif 40 <= score < 60:
                text += f"У тебя {plural(score, "балл", "балла", "баллов")}. Молодец! У тебя отлично получается!"
            elif 60 <= score <= 80:
                text += f"У тебя {plural(score, "балл", "балла", "баллов")}. Молодец! У тебя отлично получается! Осталось совсем чуть-чуть!"
            else: 
                text += f"У тебя {plural(score, "балл", "балла", "баллов")}. Молодец! У тебя велеколепные баллы!"
        else:
            #Надо логировать
            pass

    return text

def plural(input_int, one_form, two_form, three_form):
    if input_int % 10 == 1 and input_int % 100 != 11:
        return str(input_int) + " " + one_form
    elif input_int % 10 < 5 and (input_int % 100 < 10 or input_int % 100 > 20):
        return str(input_int) + " " + two_form
    else:
        return str(input_int) + " " + three_form
    



def create_text_subjects(data: list):
    text = ""
    i = 1
    for item in data:
        full_name = item.get("fullName")
        name = item.get("name")
        form_education = item.get("form_education")
        info = item.get("info")
        print(data)

        text += f"{i}. {name}\n\n"
        # if form_education == "traditional":
        #     text += "Этот курс проводится в традиционной форме. Придется ходить на очный пары в универ))"
        # elif form_education == "mixed":
        #     text += "Этот курс проводится в смешанной форме. Придется ходить и на очный пары в универ, и решать онлайн курс"
        # elif form_education == "online":
        #     text += "Этот курс проводится онлайн. Не забудь пройти его!"
        # elif form_education == "other":
        #     text += "Этот курс от партнеров вуза"
        i += 1
    return text