import App from './App.svelte'
import { admin } from "src/utility/store";
import http from "src/utility/http";

http.get("/core/admin/me", http.status()).then((value) => {
    if (value) {
        admin.update((_) => value);
    }
});

const app = new App({
    target: document.getElementById('app'),
})

export default app
