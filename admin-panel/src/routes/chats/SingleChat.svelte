<script lang="ts">
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
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";

    let props = $props();
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

    function handleKeyDown(event: KeyboardEvent) {
        if (event.key === "Escape") {
            closeImage();
        }
    }

    onMount(async () => {
        await getChat();
        await getMessages();
        await makeRead();
        window.addEventListener("keydown", handleKeyDown);
        if (paged.hasNextPage && container.scrollHeight <= container.clientHeight) {
            await loadMore();
        }
    });

    onDestroy(() => {
        signal.off("MessageReceived", handleMessageReceived);
        window.removeEventListener("keydown", handleKeyDown);
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
    
    let modalImage: string = $state(null);

    function openImage(src: string) {
        modalImage = src;
    }

    function closeImage() {
        modalImage = null;
    }
    
    let container: HTMLDivElement = $state();
    let isLoadingMore = $state(false);

    async function handleScroll() {
        if (container.scrollTop + container.scrollHeight - container.clientHeight < 3 && paged.hasNextPage && !isLoadingMore) {
            isLoadingMore = true;

            await loadMore();

            isLoadingMore = false;
        }
    }
</script>

<ToastNotifications bind:this={notifications} />

{#if errorMessage != ""}
    <ErrorAlert title="Ошибка">{errorMessage}</ErrorAlert>
{:else}
    <div class="bg-white dark:bg-gray-800 text-gray-500 dark:text-gray-400 flex flex-col p-4 sm:p-6 h-full">
        <Breadcrumbs
            class="mb-4"
            pathItems={[
                { isHome: true },
                { name: "Чаты", href: "/chats" },
                { name: `${chat.student?.info.surname} ${chat.student?.info.name} ${chat.student?.info.patronymic}` },
            ]} />
        <div
            class="bg-gray-50 dark:bg-gray-900 rounded-md p-4 overflow-auto overflow-x-hidden h-full flex flex-col-reverse gap-4"
                bind:this={container}
                on:scroll={handleScroll}>
            {#each messages as message}
                <Card
                    class={"max-w-fit min-w-40 sm:p-4 break-all" +
                        (Me.value.email == message.admin?.email
                            ? " bg-sky-100 dark:bg-sky-900"
                            : "")}>
                    <p class="font-bold">
                        {message.admin?.email ?? chat.student.email}
                    </p>

                    <p class="text-xl mb-1">{message.content}</p>

                    {#if message.documents.length > 0}
                        <div class="flex gap-1 flex-col items-baseline">
                            {#each message.documents as document}
                                {#if document.mimeType.includes("image")}
                                    <img
                                        class="my-1 max-h-[25dvh] cursor-pointer"
                                        src={`${DocumentsApi}/${document.blobId}`}
                                        alt=""
                                        on:click={() => openImage(`${DocumentsApi}/${document.blobId}`)} />
                                {/if}

                                <A href={`${DocumentsApi}/${document.blobId}`}>
                                    {document.name}
                                </A>
                            {/each}
                        </div>
                    {/if}

                    <p
                        class="relative bottom-0 right-0 text-sm text-end">
                        {new Date(message.createdAt).toLocaleTimeString("ru")}
                    </p>
                </Card>
            {/each}
            {#if isLoadingMore}
                <div class="flex justify-center p-2">
                    <Spinner />
                </div>
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
    {#if modalImage}
        <div
            class="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50"
            on:click={closeImage}>
            <img
                src={modalImage}
                class="w-auto h-auto max-w-[100vw] max-h-[100vh]"
                on:click|stopPropagation />
        </div>
    {/if}
    </div>
{/if}
