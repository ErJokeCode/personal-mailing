<script>
    import http from "src/utility/http";
    import { onMount } from "svelte";
    import { traverseObject } from "src/utility/helper";

    export let id;

    let admin = {};
    let status = http.status();
    let article;

    onMount(async () => {
        admin = (await http.get(`/core/admin/${id}`, status)) ?? {};

        article.appendChild(traverseObject(admin));
    });
</script>

<article bind:this={article}>
    <header>{admin.email}</header>
</article>

<style>
</style>
