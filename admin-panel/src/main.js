import { mount } from 'svelte'
import './app.css'
import App from './App.svelte'
import { admin } from "./utils/store";
import http from "./utils/http";

let the_admin = await http.get("/core/admin/me", http.status());
if (the_admin) {
    admin.update((_) => the_admin);
}

const app = mount(App, {
  target: document.getElementById('app'),
})

export default app
