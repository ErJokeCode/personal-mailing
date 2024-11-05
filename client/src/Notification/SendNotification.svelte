<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import http from "../http.js";

    let message = http.default_message();
    let content = "";
    let files = [];
    let ids = [];

    let activeStudents = [];

    onMount(async () => {
        let response = await fetch(`${server_url}/core/student`, {
            credentials: "include",
        });

        let json = await response.json();
        activeStudents = json;
    });

    function add_id(checked, id) {
        if (checked) {
            ids.push(id);
        } else {
            ids = ids.filter((fid) => fid != id);
        }
    }

    async function send() {
        message.load = "true";

        let response;

        try {
            let data = new FormData();

            let body = JSON.stringify({
                content: content,
                studentIds: ids,
            });

            data.append("body", body);

            if (files.length > 0) {
                for (let file of files) {
                    data.append("file", file);
                }
            }

            response = await fetch(`${server_url}/core/notification`, {
                method: "Post",
                body: data,
                credentials: "include",
            });
        } catch (err) {
            message.load = "false";
            message.value = "❌" + err;
        }

        if (response?.ok) {
            message.load = "false";
            message.value = "✅";
        }
    }
</script>

<label>
    Content:
    <input type="text" bind:value={content} />
</label>

<label>
    Files:
    <input type="file" multiple bind:files />
</label>

<button on:click={send} aria-busy={message.load}>{message.value} Send</button>

<hr />

<table>
    <thead>
        <tr>
            <th>Select</th>
            <th>Id</th>
            <th>Email</th>
        </tr>
    </thead>
    <tbody>
        {#each activeStudents as student}
            <tr>
                <th
                    ><input
                        type="checkbox"
                        on:click={(event) =>
                            add_id(event.target.checked, student.id)}
                    /></th
                >
                <th>{student.id}</th>
                <th>{student.email}</th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
