<script>
    import { onMount } from "svelte";
    import http from "src/utility/http";

    export let studentId;

    let content = "";
    let status = http.status();
    let messages = [];
    let chatArea = document.querySelector(".offset");

    async function get_messages() {
        let json =
            (await http.get(
                `/core/chat/admin-with/${studentId}`,
                http.status(),
            )) ?? undefined;
        messages = json?.messages ?? [];
    }

    onMount(async () => {
        await get_messages();
        chatArea.scrollTo(0, chatArea.scrollHeight);
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

        chatArea.scrollTo(0, chatArea.scrollHeight);
    }
</script>

<div class="wrapper">
    <div class="chats" bind:this={chatArea}>
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
            <button aria-busy={status.load} on:click={send_message}>Send</button
            >
            {status.value}
        </fieldset>
    </footer>
</div>

<style>
    .wrapper {
        display: flex;
        flex-flow: column;
        height: 100%;
    }

    .right {
        text-align: right;
    }

    fieldset {
        bottom: 0;
        width: 100%;
    }

    .chats {
        padding: 1rem;
        overflow: scroll;
        flex: 1;
    }
</style>
