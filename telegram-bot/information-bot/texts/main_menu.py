def create_text_online_course(info_course: dict, scores: dict) -> str:
    print(scores)
    name = info_course['name']
    date_start = info_course['date_start']
    deadline = info_course['deadline']
    info = info_course['info']
    university = info_course['university']
    
    text = f"{name}\n\nКурс проводит {university}\n\n"

    if date_start != None:
        text += f"{date_start}\n\n"

    if deadline != None:
        text += f"{deadline}\n\n"
    
    if info != None:
        text += f"Информация для записи на курс:\n{info}\n\n"

    is_success = False
    for name, score in scores.values():
        if score == "Нет на курсе":
            text += "Тебя нет на курсе! Перейди по ссылке выше или напиши куратору, он тебе обязательно поможет!"
            break
        else:
            if score.isdigit():
                text += "Контрольные точки:\n"
                text += f"{name}: {plural(score, "балл", "балла", "баллов")}\n"

                score = int(score)

                if score >= 40:
                    is_success = True
            else:
                #Надо логировать
                pass

    if is_success:
        text += "Молодец! У тебя отлично получается!"
    else:
        text += "Для получения зачета надо набрать минимум 40 баллов по всем контрольным точкам"

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

    dict_edu = get_dict_form_edu(data)
    for form_edu in dict_edu.keys():
        i = 1
        text += get_text_form_edu(form_edu)
        for item in dict_edu[form_edu]:

            name = item.get("name")
            link = item.get("groupTgLink")
            online_course = item.get("onlineCourse")

            text += f"{i}. {name}\n"
            text += get_text_online_course(online_course)
            text += get_text_link(link)
            text += "\n"

            i += 1
        text += "\n"
    
    text += "Если вы нашли несоответсвие сообщите куратору"

    return text


def get_dict_form_edu(data):
    dict = {"traditional" : [], "mixed": [], "online": [], "other": []}
    for item in data:
        dict[item.get("formEducation")].append(item)
    return dict
    

def get_text_form_edu(name):
    if name == "traditional":
        return "Курсы проводятся в традиционной форме. Придется ходить на очный пары в универ))\n"
    elif name == "mixed":
        return "Курсы проводятся в смешанной форме. Придется ходить и на очный пары в универ, и решать онлайн курс\n"
    elif name == "online":
        return "Курсы проводятся онлайн. Не забудь пройти его!\n"
    return "Курсы от партнеров вуза\n"


def get_text_online_course(online_course):
    if online_course != None:
        return f"Курс включает в себя прохождение онлайн курса {online_course["name"]}, его проводит {online_course["university"]}. Оснонвая информация по курсу: {online_course["info"]}. Более подробную информацию можешь узнать в разделе \"Онлайн курсы\"\n"
    return ""

def get_text_link(link):
    if link != None:
        return f"Ссылка на группу по предмету {link}\n"
    return ""