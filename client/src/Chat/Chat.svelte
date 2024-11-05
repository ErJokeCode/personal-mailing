<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import http from "../http";

    export let id;
    export let studentId;

    let content = "";
    let status = http.status();
    let messages = [];

    async function get_messages() {
        let json = (await http.get(`/core/chat/${id}`, http.status())) ?? [];
        messages = json.messages;
    }

    onMount(async () => {
        await get_messages();
        window.scrollTo(0, document.body.scrollHeight);
    });

    async function send_message() {
        status = status.start_load();
        await http.post_json(
            `/core/chat/admin-to-student?studentId=${studentId}&content=${content}`,
            {},
            status,
        );

        content = "";
        status = status.end_load();

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
