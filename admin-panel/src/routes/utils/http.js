import { server_url } from "./store.js"

function status() {
    return {
        load: "false", value: "",

        start_load() {
            this.load = "true";
            this.value = "";
            return this;
        },

        end_load() {
            this.load = "false";
            return this;
        },

        set_value(value = "✓") {
            this.value = value;
        }
    }
}

async function http(url, method, body, status) {
    try {
        let resp = await fetch(`${server_url}${url}`, {
            method: method,
            body: body,
            credentials: "include",
        });

        if (!resp.ok) {
            status.set_value("⨯");
        }
        else {
            status.set_value();
        }

        return resp;
    }
    catch (err) {
        status.set_value("⨯");
        return;
    }
}

async function post(url, body, status) {
    let resp = await http(url, "Post", body, status);
    try {
        return await resp?.json();
    }
    catch {
        return;
    }
}

async function get(url, status) {
    let resp = await http(url, "Get", null, status);
    try {
        return await resp?.json();
    }
    catch {
        return;
    }
}

async function http_json(url, method, body, status) {
    try {
        let resp = await fetch(`${server_url}${url}`, {
            method: method,
            body: JSON.stringify(body),
            credentials: "include",
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
            },
        });

        if (!resp.ok) {
            status.set_value("⨯");
        }
        else {
            status.set_value();
        }

        return resp;
    }
    catch (err) {
        status.set_value("⨯");
        return;
    }
}

async function post_json(url, body, status) {
    let resp = await http_json(url, "Post", body, status);
    try {
        return await resp?.json();
    }
    catch {
        return;
    }
}

export default {
    status, post, get, post_json,
}
