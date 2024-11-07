<script>
    import http from "src/utility/http";
    import { traverseObject } from "src/utility/helper";
    import { onMount } from "svelte";

    export let email;

    let article;
    let student = {};
    let status = http.status();

    onMount(async () => {
        student =
            (await http.get("/parser/student/?email=" + email, status)) ?? {};

        article.appendChild(traverseObject(student));
    });
</script>

<article bind:this={article}>
    <header>{student.email}</header>
</article>

<style>
</style>
