<script>
    import http from "src/utility/http";
    import { onMount } from "svelte";
    import { navigate } from "svelte-routing";

    export let id;

    let student = {};
    let status = http.status();

    onMount(async () => {
        student = await http.get(`/core/student/${id}`, status);
    });

    async function start_chat(studentId) {
        navigate(`/chat/${studentId}`);
    }
</script>

<article>
    <header>{student.email}</header>
    <ul>
        {#each Object.keys(student) as key}
            <li>{key}: {student[key]}</li>
        {/each}
    </ul>
</article>

<button on:click={() => start_chat(student.id)}>Start Chat</button>

<style>
</style>
