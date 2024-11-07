<script>
    import http from "src/utility/http";
    import { onMount } from "svelte";

    export let email;

    let student = {};
    let status = http.status();

    onMount(async () => {
        student =
            (await http.get("/parser/student/?email=" + email, status)) ?? {};
    });
</script>

<article>
    <header>{student.email}</header>
    <ul>
        {#each Object.keys(student) as key}
            <li>{key}: {student[key]}</li>
        {/each}
    </ul>
</article>

<style>
</style>
