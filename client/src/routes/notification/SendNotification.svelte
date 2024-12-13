<script>
    import { onMount } from "svelte";
    import http from "src/utility/http.js";

    let content = "";
    let files = [];
    let ids = [];
    let activeStudents = [];

    let studentStatus = http.status();
    let sendStatus = http.status();

    onMount(async () => {
        studentStatus = studentStatus.start_load();
        activeStudents = (await http.get("/core/student", studentStatus)).items ?? [];
        studentStatus = studentStatus.end_load();
    });

    function add_id(checked, id) {
        if (checked) {
            ids.push(id);
        } else {
            ids = ids.filter((fid) => fid != id);
        }
    }

    async function send() {
        sendStatus = sendStatus.start_load();

        let data = new FormData();

        for (let file of files) {
            data.append("file", file);
        }

        let body = JSON.stringify({
            content: content,
            studentIds: ids,
        });

        data.append("body", body);

        await http.post("/core/notification", data, sendStatus);

        sendStatus = sendStatus.end_load();
    }
</script>

<label>
    Содержание:
    <input type="text" bind:value={content} />
</label>

<label>
    Файлы:
    <input type="file" multiple bind:files />
</label>

<button on:click={send} aria-busy={sendStatus.load}
    >{sendStatus.value} Отправить</button
>

<hr />

<table aira-busy={studentStatus.load}>
    <thead>
        <tr>
            <th>Выбрать</th>
            <th>Айди</th>
            <th>Почта</th>
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
