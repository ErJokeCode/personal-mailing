<script>
    import { onMount } from "svelte";
    import { navigate } from "svelte-routing";
    import http from "src/utility/http";

    let chats = [];
    let status = http.status();

    onMount(async () => {
        status = status.start_load();
        chats = (await http.get("/core/admin/chats", status)).items ?? [];
        status = status.end_load();
    });

    async function open_chat(id, studentId) {
        navigate(`/chat/${studentId}`);
    }
</script>

<h2>Мои чаты</h2>

<table aria-busy={status.load}>
    <thead>
        <tr>
            <th>Чат</th>
            <th>Почта</th>
        </tr>
    </thead>
    <tbody>
        {#each chats as chat}
            <tr>
                <th
                    ><button
                        on:click={() => open_chat(chat.id, chat.student.id)}
                        >Open</button
                    ></th
                >
                <th>{chat.student.email}</th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
