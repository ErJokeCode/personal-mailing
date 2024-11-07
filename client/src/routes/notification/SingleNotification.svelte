<script>
    import http from "src/utility/http";
    import { onMount } from "svelte";

    export let id;

    let notification = {};
    let status = http.status();

    onMount(async () => {
        notification =
            (await http.get(`/core/notification/${id}`, status)) ?? {};
    });
</script>

<article>
    <header>{notification.date}</header>
    <ul>
        {#each Object.keys(notification) as key}
            <li>{key}: {notification[key]}</li>
        {/each}
    </ul>
</article>

<style>
</style>
