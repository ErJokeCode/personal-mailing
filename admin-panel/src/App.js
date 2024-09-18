import logo from './logo.svg';
import './App.css';
import {useState} from 'react';
import axios from 'axios'

const SERVER_URL = process.env.REACT_APP_SERVER_URL

function App() {
  const [file, onChangeFile] = useState();
  
  return (
    <div >
      <input type="file" onChange={(e) => onChangeFile(e.target.files[0])} />
      <button onClick={(e) => SendFile(file)}>Загрузить на сервер</button>
      <button onClick={async (e) => {
        await axios.put(`${SERVER_URL}/update/courses`)}}>Обновить курсы</button>
    </div>
  );
}

async function SendFile(file) {
  console.log(file)
  const formData = new FormData();
  formData.append("file", file)
  console.log(formData)
  await axios.post(`${SERVER_URL}/file`, formData)
}

export default App;
