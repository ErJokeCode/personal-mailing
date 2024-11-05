<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import http from "../http";

    export let id;
    export let studentId;

    let content = "";
    let status = http.default_message();
    let messages = [];

    async function get_messages() {
        let response = await fetch(`${server_url}/core/chat/${id}`, {
            credentials: "include",
        });

        let json = await response.json();
        messages = json.messages;
    }

    onMount(async () => {
        await get_messages();
        window.scrollTo(0, document.body.scrollHeight);
    });

    async function send_message() {
        status.load = "true";
        status.value = "";
        status.value = await http.http_json(
            `/core/chat/admin-to-student?studentId=${studentId}&content=${content}`,
            "Post",
            {},
        );
        content = "";
        status.load = "false";

        await get_messages();

        window.scrollTo(0, document.body.scrollHeight);
    }
</script>

<div class="offset">
    {#each messages as message}
        {#if message.sender == "Student"}
            <article class="right">
                <header>
                    {message.sender}
                </header>
                {message.content}
            </article>
        {:else}
            <article>
                <header>
                    {message.sender}
                </header>
                {message.content}
            </article>
        {/if}
    {/each}
</div>

<footer>
    <fieldset class="container" role="group">
        <input bind:value={content} type="text" />
        <button aria-busy={status.load} on:click={send_message}>Send</button>
        {status.value}
    </fieldset>
</footer>

<style>
    .right {
        text-align: right;
    }

    fieldset {
        position: fixed;
        bottom: 0;
    }

    .offset {
        padding-bottom: 5rem;
    }
</style>
