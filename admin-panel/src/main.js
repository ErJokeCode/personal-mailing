import { mount } from 'svelte'
import './app.css'
import App from './App.svelte'
import { admin } from "./utils/store";
import http from "./utils/http";
import { navigate } from 'svelte-routing';

http.get("/core/admin/me", http.status()).then((the_admin) => {
    if (the_admin) {
        admin.update((_) => the_admin);
    }
    else navigate('/login')
});

const app = mount(App, {
    target: document.getElementById('app'),
})

export default app
