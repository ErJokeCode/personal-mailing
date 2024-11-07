<script>
    import http from "src/utility/http";
    import { onMount } from "svelte";

    export let id;

    let admin = {};
    let status = http.status();

    onMount(async () => {
        admin = (await http.get(`/core/admin/${id}`, status)) ?? {};
    });
</script>

<article>
    <header>{admin.email}</header>
    <ul>
        {#each Object.keys(admin) as key}
            <li>{key}: {admin[key]}</li>
        {/each}
    </ul>
</article>

<style>
</style>
