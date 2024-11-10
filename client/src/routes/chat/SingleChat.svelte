<script>
    import { onDestroy, onMount } from "svelte";
    import http from "src/utility/http";
    import { signal } from "src/utility/signal";

    export let studentId;

    let content = "";
    let status = http.status();
    let messages = [];
    let chatArea;
    let student = {};
    let admin = {};

    async function get_messages() {
        let json =
            (await http.get(
                `/core/chat/adminWith/${studentId}`,
                http.status(),
            )) ?? undefined;
        student = json?.student ?? {};
        admin = json?.admin ?? {};
        messages = json?.messages ?? [];
    }

    async function send_message() {
        status = status.start_load();
        await http.post_json(
            `/core/chat/adminSend`,
            {
                studentId,
                content,
            },
            status,
        );

        content = "";
        status = status.end_load();

        await get_messages();
        chatArea.scrollTo(0, chatArea.scrollHeight);
    }

    async function handle_keypress(event) {
        if (event.key == "Enter") {
            send_message();
        }
    }

    onMount(async () => {
        await get_messages();

        chatArea.scrollTo(0, chatArea.scrollHeight);

        signal.on("StudentSentMessage", handle_student_message);
    });

    onDestroy(async () => {
        signal.off("StudentSentMessage", handle_student_message);
    });

    async function handle_student_message(message) {
        if (message.student.id == studentId) {
            await get_messages();

            chatArea.scrollTo(0, chatArea.scrollHeight);
        }
    }
</script>

<div class="wrapper">
    <div class="chats" bind:this={chatArea}>
        {#each messages as message}
            {#if message.sender == "Admin"}
                <article class="right">
                    <header>
                        {admin.email}
                    </header>
                    {message.content}
                </article>
            {:else}
                <article>
                    <header>
                        {student.email}
                    </header>
                    {message.content}
                </article>
            {/if}
        {/each}
    </div>

    <footer>
        <fieldset class="container" role="group">
            <input
                bind:value={content}
                type="text"
                on:keypress={handle_keypress}
            />
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
