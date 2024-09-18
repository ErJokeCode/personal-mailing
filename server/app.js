const routes = require('./routes/routes');
const cors = require('cors');
const axios = require('axios')

const MongoClient = require("mongodb").MongoClient;
const dotenv = require('dotenv'); 
dotenv.config({ path: './.env' }); 

const port = 8000;

const express = require("express");
var fileUpload = require('express-fileupload');


      
const app = express();
app.use(express.static("public"));  // статические файлы будут в папке public
app.use(express.json());        // подключаем автоматический парсинг json
app.use(cors());
app.use(fileUpload({}));

const mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
    
   
(async () => {
     try {
        await mongoClient.connect();
        app.locals = mongoClient.db("personal-mailing");
        await app.listen(port);
    }catch(err) {
        return console.log(err);
    } 
})();
   
routes(app)
    
// прослушиваем прерывание работы программы (ctrl-c)
process.on("SIGINT", async() => {
    await mongoClient.close();
    console.log("Приложение завершило работу");
    process.exit();
});