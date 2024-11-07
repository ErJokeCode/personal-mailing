<script>
    import http from "src/utility/http";
    import { onMount } from "svelte";
    import { navigate } from "svelte-routing";
    import { traverseObject } from "src/utility/helper";

    export let id;

    let student = {};
    let status = http.status();
    let article;

    onMount(async () => {
        student = await http.get(`/core/student/${id}`, status);
        article.appendChild(traverseObject(student));
    });

    async function start_chat(studentId) {
        navigate(`/chat/${studentId}`);
    }
</script>

<button on:click={() => start_chat(student.id)}>Start Chat</button>

<article bind:this={article}>
    <header>{student.email}</header>
</article>

<style>
    button {
        margin-bottom: 1rem;
    }
</style>
