<script>
	import { Breadcrumb, BreadcrumbItem, Button, Heading, Textarea, ToolbarButton, Card } from 'flowbite-svelte';
	import { ArrowRightOutline } from 'flowbite-svelte-icons';
    import { onDestroy, onMount } from "svelte";
    import http from "../../../../utility/http";
    import { signal } from "../../../../utility/signal";
    import { page } from '$app/stores';

    let studentId = $page.params.chat;

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

        var data = new FormData();
        data.append("body", JSON.stringify({ studentId, content }));
        // Can also add documents

        await http.post(`/core/chat/adminSend`, data, status);

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

<main class="h-full w-full">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home>Главная</BreadcrumbItem>
			<BreadcrumbItem href="/chat">Все чаты</BreadcrumbItem>
			<BreadcrumbItem>Чат</BreadcrumbItem>
		</Breadcrumb>
	</div>
    <div class="px-20 mb-10 p-10">
        <Card size="xxl" class="overflow-auto mb-5 p-10" bind:this={chatArea}>
            <div class="h-s">
                {#each messages as message}
                    {#if message.sender == "Admin"}
                        <article class="space-y-3 mx-5 mb-5">
                            <footer class="flex items-center justify-end">
                                <div class="flex items-center gap-2">
                                    <p class="text-sm font-semibold text-gray-900 dark:text-white">
                                        {admin.email}
                                    </p>
                                </div>
                            </footer>
                            <div class="space-y-2 text-gray-900 dark:text-white text-right">{message.content}</div>
                        </article>
                    {:else}
                        <article class="space-y-3 mx-5 mb-5">
                            <footer class="flex items-center">
                                <div class="flex items-center gap-2">
                                    <p class="text-sm font-semibold text-gray-900 dark:text-white">
                                        {student.email}
                                    </p>
                                </div>
                            </footer>
                            <div class="space-y-2 text-gray-900 dark:text-white">{message.content}</div>
                        </article>
                    {/if}
                {/each}
            </div>
        </Card>
        <div class="flex items-center gap-5">
            <Textarea
                rows={1}
                placeholder="Введите текст" 
                bind:value={content}
                on:keypress={handle_keypress}
                class="bg-white dark:bg-gray-800"/>
            <ToolbarButton
                type="submit"
                color="default"
                class="p-2 text-primary-600 hover:bg-primary-100"
            >
                <svg
                    on:click={send_message}
                    aria-hidden="true"
                    class="h-6 w-6 rotate-90"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                    xmlns="http://www.w3.org/2000/svg"
                    ><path
                        d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z"
                    ></path></svg
                >
                <span class="sr-only">Send message</span>
            </ToolbarButton>
        </div>
    </div>
</main>

<style>
    .h-s {
        height: 60dvh;
    }
</style>