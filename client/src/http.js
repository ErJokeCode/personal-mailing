import { server_url } from "./store.js"

function default_message() {
    return { load: "false", value: "" };
}

async function http_json(url, method, body) {
    let response;

    try {
        response = await fetch(`${server_url}${url}`, {
            method: method,
            body: JSON.stringify(body),
            credentials: "include",
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
            },
        });
    } catch (err) {
        return "❌" + err;
    }

    if (response?.ok) {
        return "✅";
    }
}

async function http_files(url, method, field, files) {
    if (!files || files.length <= 0) return "❌";

    var data = new FormData();
    for (let file of files) {
        data.append(field, file);
    }

    let response;

    try {
        response = await fetch(`${server_url}${url}`, {
            method: method,
            body: data,
            credentials: "include",
        });
    } catch (err) {
        return "❌" + err;
    }

    if (response?.ok) {
        return "✅";
    }
}

export default {
    default_message, http_json, http_files,
}
