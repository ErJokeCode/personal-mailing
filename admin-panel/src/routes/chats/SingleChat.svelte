<script lang="ts">
    import { route } from "@mateothegreat/svelte5-router";
    import { ChatsApi, DocumentsApi, PageSize } from "/src/lib/server";
    import Panel from "/src/lib/components/Panel.svelte";
    import { Me } from "/src/stores/Me.svelte";
    import {
        A,
        Button,
        Card,
        Fileupload,
        Input,
        Spinner,
        Breadcrumb,
        BreadcrumbItem,
    } from "flowbite-svelte";
    import { createPaged } from "/src/lib/components/Paged.svelte";
    import { onDestroy, onMount } from "svelte";
    import {
        CloseOutline,
        PaperClipOutline,
        PaperPlaneOutline,
    } from "flowbite-svelte-icons";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
    import { GeneralError } from "/src/lib/errors";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import { signal } from "/src/lib/utils/signal";

    let props= $props();
    let studentId = props.route.params?.["studentId"] ?? "";
    let errorMessage = $state("");

    let chat: any = $state({});
    let paged = $state(createPaged());
    let messages = $state([]);

    let content = $state("");
    let files: FileList = $state(null);
    let sendText: boolean = $state(true);
    let notifications: ToastNotifications;

    signal.on("MessageReceived", handleMessageReceived);

    onMount(async () => {
        await getChat();
        await getMessages();
        await makeRead();
    });

    onDestroy(() => {
        signal.off("MessageReceived", handleMessageReceived);
    });

    async function handleMessageReceived(message) {
        messages = [message, ...messages];

        await makeRead();
    }

    async function makeRead() {
        await fetch(`${ChatsApi}/${studentId}/read`, {
            method: "PATCH",
            credentials: "include",
        });
    }

    async function getChat() {
        try {
            let res = await fetch(`${ChatsApi}/${studentId}`, {
                credentials: "include",
            });

            let body = await res.json();

            if (!res.ok) {
                errorMessage = body.detail;
                throw new Error();
            }

            chat = body;
        } catch {
            errorMessage = GeneralError;
        }
    }

    async function getMessages() {
        try {
            let url = new URL(`${ChatsApi}/${studentId}/messages`);
            url.searchParams.set("page", paged.page.toString());
            url.searchParams.set("pageSize", PageSize.toString());

            let res = await fetch(url, {
                credentials: "include",
            });

            let body = await res.json();

            if (!res.ok) {
                errorMessage = body.detail;
                throw new Error();
            }

            Object.assign(paged, body);

            messages.push(...body.items);
        } catch {
            errorMessage = GeneralError;
        }
    }

    async function loadMore() {
        if (paged.hasNextPage) {
            paged.page += 1;

            await getMessages();
        }
    }

    function removeFile(index) {
        const dt = new DataTransfer();

        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            if (index !== i) {
                dt.items.add(file);
            }
        }

        files = dt.files;
    }

    async function sendMessage() {
        sendText = null;

        try {
            let data = new FormData();

            let body = JSON.stringify({
                content: content,
                studentId: chat.student.id,
            });

            data.append("body", body);

            if (files !== null) {
                for (let file of files) {
                    data.append("documents", file);
                }
            }

            let res = await fetch(ChatsApi, {
                method: "Post",
                body: data,
                credentials: "include",
            });

            if (res.ok) {
                let message = await res.json();

                messages = [message, ...messages];

                clear();
            }
        } catch (err) {
            notifications.add({
                type: "error",
                text: "Ошибка при отправке сообщения",
            });
        }

        sendText = true;
    }

    function clear() {
        content = "";
        files = null;
    }

    async function keypress(event) {
        if (event.key == "Enter") {
            await sendMessage();
        }
    }
</script>

<ToastNotifications bind:this={notifications} />

{#if errorMessage != ""}
    <ErrorAlert title="Ошибка">{errorMessage}</ErrorAlert>
{:else}
    <Panel class="rounded-none border-l-0 border-t-0 h-full">
        <Breadcrumb class="mb-5">
            <li class="inline-flex items-center">
                <a class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                use:route href="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</a></li>
            <li class="inline-flex items-center">
                <svg class="w-6 h-6 text-gray-400 rtl:-scale-x-100" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0
                z" clip-rule="evenodd"></path></svg>
            <a class="ml-0 ms-1 text-sm font-medium text-gray-700 hover:text-gray-900 md:ms-2 dark:text-gray-400 dark:hover:text-white"
                use:route href="/chats">Чаты</a></li>
            <BreadcrumbItem>{chat.student?.email}</BreadcrumbItem>
        </Breadcrumb>
        <div
            class="bg-gray-50 dark:bg-gray-900 rounded-md p-4 overflow-auto overflow-x-hidden h-full flex flex-col-reverse gap-4">
            {#each messages as message}
                <Card
                    class={"relative max-w-fit sm:p-3 sm:pr-12" +
                        (Me.value.email == message.admin?.email
                            ? " bg-sky-100 dark:bg-sky-900"
                            : "")}>
                    <p class="font-bold">
                        {message.admin?.email ?? chat.student.email}
                    </p>

                    <p class="text-xl">{message.content}</p>

                    {#if message.documents.length > 0}
                        <div class="flex gap-1 flex-col items-baseline">
                            {#each message.documents as document}
                                {#if document.mimeType.includes("image")}
                                    <img
                                        class="mb-1"
                                        src={`${DocumentsApi}/${document.blobId}`}
                                        alt="" />
                                {/if}

                                <A href={`${DocumentsApi}/${document.blobId}`}>
                                    {document.name}
                                </A>
                            {/each}
                        </div>
                    {/if}

                    <p
                        class="ml-4 relative bottom-0 -right-10 text-sm text-end">
                        {new Date(message.createdAt).toLocaleTimeString("ru")}
                    </p>
                </Card>
            {/each}

            {#if paged.hasNextPage}
                <Button on:click={loadMore}>Больше</Button>
            {/if}
        </div>

        <div class="flex m-4 gap-4">
            <Button outline color="alternative" class="relative">
                <PaperClipOutline />
                <Fileupload
                    bind:files
                    multiple
                    class="absolute left-0 top-0 bottom-0 right-0 opacity-0" />
            </Button>
            <Input
                type="text"
                placeholder="Введите текст"
                on:keypress={keypress}
                bind:value={content}
                class="bg-dark-50 dark:bg-gray-900" />
            <Button on:click={sendMessage}>
                {#if sendText}
                    <PaperPlaneOutline class="w-6 h-6 rotate-90" />
                {:else}
                    <Spinner color="white" size="6" />
                {/if}
            </Button>
        </div>

        <div class="flex gap-2 items-center">
            {#each files as file, index}
                <span class="align-middle text-lg">{file.name}</span>
                <Button
                    on:click={() => removeFile(index)}
                    outline
                    color="red"
                    size="xs"
                    class="w-5 h-5">
                    <CloseOutline />
                </Button>
            {/each}
        </div>
    </Panel>
{/if}
