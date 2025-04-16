<script lang="ts">
    import { ChatsApi, DocumentsApi, PageSize } from "/src/lib/server";
    import { Me } from "/src/stores/Me.svelte";
    import {
        A,
        Button,
        Card,
        Fileupload,
        Input,
        Spinner,
        Sidebar,
        SidebarWrapper,
        Heading,
        Tabs,
        TabItem
    } from "flowbite-svelte";
    import { createPaged } from "/src/lib/components/Paged.svelte";
    import { onDestroy, onMount } from "svelte";
    import {
        CloseOutline,
        PaperClipOutline,
        PaperPlaneOutline,
        RectangleListOutline,
    } from "flowbite-svelte-icons";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
    import { GeneralError } from "/src/lib/errors";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import { signal } from "/src/lib/utils/signal";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
    import { show } from "/src/stores/chats/Chats.svelte";

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

    let chatWidth = $state();
</script>

<ToastNotifications bind:this={notifications} />

{#if errorMessage != ""}
    <ErrorAlert title="Ошибка">{errorMessage}</ErrorAlert>
{:else}
    <div class="bg-white dark:bg-gray-800 text-gray-500 dark:text-gray-400 flex h-full w-full">
        <div class="flex flex-col px-4 py-3 sm:px-6 sm:py-5 h-full w-full">
            <div class="relative mb-3 sm:mb-4">
                <Breadcrumbs
                    class="hidden sm:block"
                    pathItems={[
                        { isHome: true },
                        { name: "Чаты", href: "/chats" },
                        { name: `${chat.student?.info.surname} ${chat.student?.info.name} ${chat.student?.info.patronymic}` },
                    ]} />
                <Breadcrumbs
                    class="block sm:hidden"
                    pathItems={[
                        { isHome: true },
                        { name: "Чаты", href: "/chats" },
                        { name: `${chat.student?.info.surname} ${chat.student?.info.name[0]}. ${chat.student?.info.patronymic[0]}.` },
                    ]} />
                <button on:click={() => show.value = !show.value}
                        class="absolute -right-2 -top-2 rounded p-1 hover:bg-gray-100 dark:hover:bg-gray-600
                            focus:outline-none cursor-pointer">
                    <RectangleListOutline size="xl" />
                </button>
            </div>
            <div
                class="bg-gray-50 dark:bg-gray-900 rounded-md p-4 overflow-auto overflow-x-hidden h-full flex flex-col-reverse gap-4"
                    bind:this={container}
                    on:scroll={handleScroll}
                    bind:clientWidth={chatWidth}>
                {#each messages as message}
                    <Card
                        class={"max-w-[80%] w-fit min-w-40 py-2 px-3 sm:py-3 sm:px-4 break-all" +
                            (Me.value.email == message.admin?.email
                                ? " bg-sky-100 dark:bg-sky-900" + (show.value || chatWidth < 1300 ? " ml-auto" : " ml-0")
                                : "")}>
                        <p class="sm:text-lg font-bold">
                            {message.admin?.email ?? chat.student.email}
                        </p>
    
                        <p class="sm:text-lg sm:mb-1">{message.content}</p>
    
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
    
                        <p class="relative bottom-0 right-0 text-xs sm:text-sm text-end">
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
    
            <div class="flex m-3 sm:m-4 gap-3 sm:gap-4">
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
                    class="bg-gray-50 dark:bg-gray-900" />
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
        <Sidebar
                class={show.value ? "fixed md:relative" : "hidden"}
                asideClass={"w-full md:w-md h-full flex-none md:border-s border-gray-200 dark:border-gray-600"
                    + (show.value ? "" : " hidden")}>
            <SidebarWrapper class="bg-white h-full px-4 py-3 sm:px-6 sm:py-5">
                <button on:click={() => show.value = false}
                        class="absolute right-2 top-1 sm:right-4 sm:top-3 rounded p-1 hover:bg-gray-100 dark:hover:bg-gray-600
                            focus:outline-none cursor-pointer md:hidden">
                    <CloseOutline size="xl" />
                </button>
                <Tabs tabStyle="underline">
                    <TabItem open title="Информация">
                        <ul>
                            <li><p><b>Фамилия:</b> {chat.student?.info.surname}</p></li>
                            <li><p><b>Имя:</b> {chat.student?.info.name}</p></li>
                            <li><p><b>Отчество:</b> {chat.student?.info.patronymic}</p></li>
                            <li><p><b>Почта:</b> {chat.student?.email}</p></li>
                            <li><p><b>Номер студенческого:</b> {chat.student?.info.personalNumber}</p></li>
                            <li><p><b>Дата рождения:</b> {chat.student?.info.dateOfBirth}</p></li>
                            <li><p><b>Номер группы:</b> {chat.student?.info.group.number}</p></li>
                            <li>
                                <p>
                                    <b>Курс:</b>
                                    {chat.student?.info.group !== undefined
                                        ? chat.student?.info.group.numberCourse
                                        : "Не известен"}
                                </p>
                            </li>
                            <li>
                                <p>
                                    <b>Направление:</b>
                                    {chat.student?.info.group !== undefined
                                        ? chat.student?.info.group.directionCode + " " + chat.student?.info.group.nameSpeciality
                                        : "Не известно"}
                                </p>
                            </li>
                            <li>
                                <p>
                                    <b>Статус:</b>
                                    {chat.student?.info.status ? "Активный студент" : "Не активный студент"}
                                </p>
                            </li>
                            <li><p><b>Тип оплаты обучения:</b> {chat.student?.info.typeOfCost}</p></li>
                            <li><p><b>Форма обучения:</b> {chat.student?.info.typeOfEducation}</p></li>
                        </ul>
                    </TabItem>
                    <TabItem title="Вложения" class="text-xl">
                        ⣿⣿⣿⣿⣿⣿⡿⠛⣛⣛⣛⣛⣛⣛⣛⣛⣛⣛⡛⠛⠿⠿⢿⣿⣿⣿⣿⣿⣿
                        ⣿⣿⣿⣿⡿⢃⣴⣿⠿⣻⢽⣲⠿⠭⠭⣽⣿⣓⣛⣛⣓⣲⣶⣢⣍⠻⢿⣿⣿
                        ⣿⣿⣿⡿⢁⣾⣿⣵⡫⣪⣷⠿⠿⢿⣷⣹⣿⣿⣿⢲⣾⣿⣾⡽⣿⣷⠈⣿⣿
                        ⣿⣿⠟⠁⣚⣿⣿⠟⡟⠡⠀⠀⠀⠶⣌⠻⣿⣿⠿⠛⠉⠉⠉⢻⣿⣿⠧⡙⢿
                        ⡿⢡⢲⠟⣡⡴⢤⣉⣛⠛⣋⣥⣿⣷⣦⣾⣿⣿⡆⢰⣾⣿⠿⠟⣛⡛⢪⣎⠈
                        ⣧⢸⣸⠐⣛⡁⢦⣍⡛⠿⢿⣛⣿⡍⢩⠽⠿⣿⣿⡦⠉⠻⣷⣶⠇⢻⣟⠟⢀
                        ⣿⣆⠣⢕⣿⣷⡈⠙⠓⠰⣶⣤⣍⠑⠘⠾⠿⠿⣉⣡⡾⠿⠗⡉⡀⠘⣶⢃⣾
                        ⣿⣿⣷⡈⢿⣿⣿⣌⠳⢠⣄⣈⠉⠘⠿⠿⠆⠶⠶⠀⠶⠶⠸⠃⠁⠀⣿⢸⣿
                        ⣿⣿⣿⣷⡌⢻⣿⣿⣧⣌⠻⢿⢃⣷⣶⣤⢀⣀⣀⢀⣀⠀⡀⠀⠀⢸⣿⢸⣿
                        ⣿⣿⣿⣿⣿⣦⡙⠪⣟⠭⣳⢦⣬⣉⣛⠛⠘⠿⠇⠸⠋⠘⣁⣁⣴⣿⣿⢸⣿
                        ⣿⣿⣿⣿⣿⣿⣿⣷⣦⣉⠒⠭⣖⣩⡟⠛⣻⣿⣿⣿⣿⣿⣟⣫⣾⢏⣿⠘⣿
                        ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣤⣍⡛⠿⠿⣶⣶⣿⣿⣿⣿⣿⣾⣿⠟⣰⣿
                        ⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣶⣶⣤⣭⣍⣉⣛⣋⣭⣥⣾⣿⣿
                    </TabItem>
                </Tabs>
            </SidebarWrapper>
        </Sidebar>
    </div>
{/if}
