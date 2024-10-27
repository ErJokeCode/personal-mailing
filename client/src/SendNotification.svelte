<script>
    import { onMount } from "svelte";
    import { server_url } from "./store";

    let login_status = "";
    let send_status = "";
    let content = "";
    let files = [];
    let ids = [];

    let activeStudents = [];

    onMount(async () => {
        let response;

        try {
            response = await fetch(`${server_url}/core/students`, {
                credentials: "include",
            });
        } catch (err) {
            login_status = "Not Logged In";
        }

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
        send_status = "Sending...";

        let response;

        try {
            let data = new FormData();

            let body = JSON.stringify({
                content: content,
                studentIds: ids,
            });

            data.append("json", body);
            if (files.length > 0) {
                data.append("file", files[0]);
            }

            response = await fetch(`${server_url}/core/send_notification`, {
                method: "Post",
                body: data,
                credentials: "include",
            });
        } catch (err) {
            send_status = "Something went wrong! " + err;
        }

        if (response?.ok) {
            send_status = "You successfully sent a notification!";
        }
    }
</script>

<h1>{login_status}</h1>

<button on:click={send}>Send</button>
<span>{send_status}</span>

<br />
<br />

<label>
    Content:
    <input type="text" bind:value={content} />
</label>
<label>
    File:
    <input type="file" bind:files />
</label>

<br />
<br />

<table>
    <tr>
        <th>Select</th>
        <th>Id</th>
        <th>Email</th>
    </tr>
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
</table>

<style>
    th {
        padding-right: 1rem;
    }
</style>
