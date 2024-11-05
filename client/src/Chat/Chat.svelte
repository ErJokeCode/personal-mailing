<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import http from "../http";

    export let id;

    let studentId = "cabfa570-d6f3-4156-9723-3222a7458d66";
    let content = "";
    let status = http.default_message();
    let messages = [];

    onMount(async () => {
        let response = await fetch(`${server_url}/core/chat/${id}`, {
            credentials: "include",
        });

        let json = await response.json();
        messages = json.messages;
    });

    async function send_message() {
        status.load = "true";
        status.value = "";
        status.value = await http.http_json(
            `/core/chat/admin-to-student?studentId=${studentId}&content=${content}`,
            "Post",
            {},
        );
        status.load = "false";
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
        <button aria-busy={status.load} on:click={send_message}
            >{status.value} Send</button
        >
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
