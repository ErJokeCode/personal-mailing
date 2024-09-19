const axios = require('axios');
const dotenv = require('dotenv'); 
const XLSX = require('xlsx');
var crypto = require('crypto');
var fs = require('fs');
const cheerio = require('cheerio');
const { start } = require('repl');
const { response } = require('express');
dotenv.config({ path: './.env' }); 

const MODEUS_TOKEN = process.env.MODEUS_TOKEN

const router = app => {   
    app.post("/api/file", async(req, res)=> {
        try{
            const collection = req.app.locals.collection("users");
            var path = 'public/'+req.files.file.name;

            req.files.file.mv(path).then( function(){
                excel_to_db(path, collection)
            }) 
            res.sendStatus(200)
        }
        catch(err) {
            console.log(err)
            res.sendStatus(500)
        }
    })
    
    app.get("/api/courses", async(req, res)=> {
        try{
            console.log(req.body)
            const university = req.body.university;
            const name_course = req.body.name_course;
            const collection = req.app.locals.collection("courses");
            await parse_inf_urfu_to_db(collection).then(async function() {
                let results = []
                if(name_course != undefined && university != undefined){
                    results = await collection.find({"name": name_course, "university": university}).toArray();
                }
                else if(name_course != undefined){
                    results = await collection.find({"name": name_course}).toArray();
                }
                else if(university != undefined){
                    results = await collection.find({"university": university}).toArray();
                }
                else{
                    results = await collection.find().toArray();
                }
                
                res.json(results) 
            })  
        }
        catch(err){
            console.log(err)
            res.sendStatus(500)
        }
    });

    app.get("/api/courses/universitys", async(req, res)=> {
        try{
            const collection = req.app.locals.collection("courses");
            await parse_inf_urfu_to_db(collection).then(async function() {
                const results = await collection.find().toArray();
                let universitys = []
                results.forEach(course => {
                    if (universitys.indexOf(course.university) == -1){
                        universitys.push(course.university)
                    }
                })
                res.json(universitys) 
            })  
        }
        catch(err){
            console.log(err)
            res.sendStatus(500)
        }
    });

    app.get("/api/events/search", async(req, res) => {
        try{
            const config = {
                headers: { Authorization: `Bearer ${MODEUS_TOKEN}` }
            };

            const bodyPeopleSearch = {
                "fullName": "Соловьёв Эрик",
                "sort": "+fullName",
                "size": 10,
                "page": 0
              };
            
            

            const {data} = await axios.post("https://urfu.modeus.org/schedule-calendar-v2/api/people/persons/search", bodyPeopleSearch, config )

            const idPerson = data._embedded.persons[0].id

            const bodyEventsSearch = {
                "size": 500,
                "timeMin": "2024-09-15T19:00:00Z",
                "timeMax": "2024-09-22T19:00:00Z",
                "attendeePersonId": [
                  idPerson
                ]
              }

            const events = await axios.post("https://urfu.modeus.org/schedule-calendar-v2/api/calendar/events/search?tz=Asia/Yekaterinburg", bodyEventsSearch, config)

            const allEvents = events.data._embedded.events
            const teamEvents = events.data._embedded['lesson-realization-teams'];
            const eventsLocations = events.data._embedded['event-locations']
            
            
            const eventsWithTime = []

            allEvents.forEach(event => {
                const linkTeam = event._links['lesson-realization-team'].href
                const idTeam = linkTeam.slice(1, linkTeam.length)
                const idEvent = event.id


                const team = teamEvents.filter(function(team) {
                    return team.id == idTeam
                })

                const locations = eventsLocations.filter(function(location) {
                    return location.eventId == idEvent
                })

                if(locations.customLocation == null){
                    const event_rooms = locations._links['event-rooms']
                }

                eventsWithTime.push({event, team, locations})
            });

            res.json(events.data)

        }
        catch(err) {
            console.log(err)
            res.sendStatus(500)
        }
    })
}

async function parse_inf_urfu_to_db(collection) {
    try{
        const {data} = await axios.get("https://inf-online.urfu.ru/ru/onlain-kursy/");

        const $ = cheerio.load(data);
        const table = $('.ce-bodytext table tbody');
        const courses = table_courses_to_json($, table)

        await collection.drop().then(async function() {
            await collection.insertMany(courses);
        });
    }
    catch(err){
        console.log(err);
    }
}

function table_courses_to_json($, table) {
    let courses = []

    table.each((idTable, elTable) => {
        let university = "";
        let info = "";

        const rows = $(elTable).children();

        rows.each((idRow, elRow) => {
            try{
                let name = "";
                let date = "";
                
                const cols = $(elRow).children();

                cols.each((idCol, elCol) => {
                    const col = $(elCol);

                    if(idRow == 0 && idCol == 1) {
                        const textBeforeUniversity = "Курсы ";
                        let text = col.text();
                        university = text.slice(textBeforeUniversity.length, text.length - 1);
                    }

                    if(idRow != 0 && idCol == 1){
                        const findText = "<br>";
                        let text = col.html();
                        const index = text.indexOf(findText)
                        if(index != -1){
                            if(text.slice(0, 3) == "<p>"){
                                name = text.slice(3, index)
                                date = text.slice(index + findText.length, text.length - 4)
                            }
                            else {
                                name = text.slice(0, index)
                                date = text.slice(index + findText.length, text.length)
                            }
                        }
                        else {
                            name = col.text();
                        }
                    }
                    
                    if(idRow != 0 && idCol == 2){
                        info = col.text()
                    }
                })

                if(idRow != 0) {
                    courses.push({university, name, date, info})
                }
            }
            catch (error){
                console.log(error)
            }
        })
    })

    return courses
}

async function excel_to_db(path, collection) {
    var workbook = XLSX.readFile(path);
    var sheet_name_list = workbook.SheetNames;
    var xlData

    // for(let i = 1; i < sheet_name_list.length; i++){
    //     xlData = XLSX.utils.sheet_to_json(workbook.Sheets[sheet_name_list[i]]);

    // }

    xlData = XLSX.utils.sheet_to_json(workbook.Sheets[sheet_name_list[1]]);

    console.log(xlData[0])

    var user = create_user(xlData[0]);

    var findUser = await collection.findOneAndUpdate(
        {name : user.name, sername : user.sername, patronymic : user.patronymic}, 
        {$push : {courses : user.courses[0]}}, 
        { returnDocument: "after" }
    )
    console.log(findUser)
    if(findUser == null){
        await collection.insertOne(user);  
    }

    fs.unlink(path, (err) => {
        if(err) console.log(err)
    })
}

function create_user(data) {
    var course = {
        "name" : data['Название ОК'], 
        "final score" : data['Итоговый балл']
    }
    
    var user = {
        "name" : data['Имя'], 
        "sername" : data['Фамилия'], 
        "patronymic" : data['Отчество'], 
        "group" : data['Группа'], 
        "email" : data['Адрес электронной почты'], 
        "courses" : [course]
    }
    return user
}

module.exports = router;