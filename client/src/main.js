import App from './App.svelte'
import { admin } from "./store";
import http from "./http";

let the_admin = await http.get("/core/admin/me", http.status());
if (the_admin) {
    admin.update((_) => the_admin);
}

const app = new App({
    target: document.getElementById('app'),
})

export default app
