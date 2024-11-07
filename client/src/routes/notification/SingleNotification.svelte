<script>
    import http from "src/utility/http";
    import { onMount } from "svelte";
    import { traverseObject } from "src/utility/helper";

    export let id;

    let notification = {};
    let status = http.status();
    let article;

    onMount(async () => {
        notification =
            (await http.get(`/core/notification/${id}`, status)) ?? {};

        article.appendChild(traverseObject(notification));
    });
</script>

<article bind:this={article}>
    <header>{notification.date}</header>
</article>

<style>
</style>
