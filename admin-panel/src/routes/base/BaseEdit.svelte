<script lang="ts">
    import { ChatsApi } from "/src/lib/server";
    import {
        Heading,
        Button,
        Textarea,
        Input,
        Fileupload,
        Helper,
        A,
    } from "flowbite-svelte";
    import { Label, Modal } from "flowbite-svelte";
    import { CloseOutline, ExclamationCircleOutline } from "flowbite-svelte-icons";
    import Panel from "/src/lib/components/Panel.svelte";
    import http from "/src/lib/utils/http";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
    import { onMount } from "svelte";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import { goto } from "@mateothegreat/svelte5-router";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
    import { Base } from "/src/lib/server";
    
    let errorMessage = $state('');

    let props = $props();
    let id = props.route.params['itemId'];

    let notifications: ToastNotifications;

    let status = http.status();
    let chats = $state([]);

    let categories = $state([]);

    let question = $state();
    let answer = $state();

    let newQuestion = $state("");
    let newAnswer = $state("");

    let deleteOpen = $state(false);

    onMount(async () => {
        await getQuestion();
        await getCategories();
        await getChats();
    });

    const getChats = async () => {
        status = status.start_load();
        chats = (await http.get("/core/chats", status)).items ?? [];
        status = status.end_load();
    }

    const getCategories = async () => {
        let response = await fetch(`${Base}/category/`, {
            method: 'GET',
            credentials: "include",
        });
        let json = await response?.json();
        categories = json;
    }

    const getQuestion = async () => {
        let response = await fetch(`${Base}/question/${id}`, {
            method: 'GET',
            credentials: "include",
        });
        let json = await response.json();
        if (!response.ok) {
            errorMessage = json.detail;
        }
        question = json;
        if (question.answers.length !== 0) {
            answer = question.answers[0];
            newAnswer = answer.answer;
        }
        newQuestion = question.question;
    }

    const createAnswer = async () => {
        let body = {
            id_question: question.id,
            answer: newAnswer,
        };
        let response = await fetch(`${Base}/answer/`, {
            method: 'POST',
            headers: {
                Accept: "application/json, */*",
                "Content-Type": "application/json",
            },
            credentials: "include",
            body: JSON.stringify(body)
        });
        let json = await response.json();
        answer = json;
        newAnswer = answer.answer;
    }

    const changeAnswer = async (added=true) => {
        if (question.answers.length === 0) {
            createAnswer();
        } else {
            if (added && selectedFiles !== undefined) {
                let data = new FormData();
                for (let file of selectedFiles) {
                    data.append("files", file);
                }
                await fetch(`${Base}/answer/${answer.id}/file`, {
                    method: 'POST',
                    body: data,
                    credentials: "include",
                });
            }
            if (newAnswer !== answer.answer) {
                let body = {
                    id_question: question.id,
                    answer: newAnswer,
                };
                await fetch(`${Base}/answer/${answer.id}`, {
                    method: 'PATCH',
                    headers: {
                        Accept: "application/json, */*",
                        "Content-Type": "application/json",
                    },
                    credentials: "include",
                    body: JSON.stringify(body)
                });
            }
            getQuestion();
        }
    }

    const removeFile = async (id) => {
        let response = await fetch(`${Base}/file/${id}`, {
            method: 'DELETE',
            credentials: "include",
        });
        if (response.ok) {
            notifications.add({
                type: "ok",
                text: "Успешно удалено",
            });
            getQuestion();
        } else {
            notifications.add({
                type: "error",
                text: response.statusText,
            })
        }
    }

    const updateQuestion = async () => {
        let newId = document.getElementById('topic').value;
        if (newQuestion === question.question && selectedFiles === undefined && newId === question.id_category && newAnswer === answer?.answer) return;
        changeAnswer();

        let body = {
            id_category: newId,
            question: newQuestion,
        };

        let response = await fetch(`${Base}/question/${id}`, {
            method: 'PATCH',
            headers: {
                Accept: "application/json, */*",
                "Content-Type": "application/json",
            },
            credentials: "include",
            body: JSON.stringify(body)
        });

        if (response.ok) {
            notifications.add({
                type: "ok",
                text: "Успешно изменено",
            });
            getQuestion();
        } else {
            notifications.add({
                type: "error",
                text: response.statusText,
            })
        }
    }

    const deleteQuestion = async () => {
        await fetch(`${Base}/question/${id}`, {
            method: 'DELETE',
            credentials: "include",
        });
        goto('/base')
    }

    const send = async () => {
        if (selectedChat === "") return;
        try {
            let data = new FormData();

            let body = JSON.stringify({
                content: `${newQuestion}\n${newAnswer}`,
                studentId: selectedChat.student.id,
            });

            data.append("body", body);

            if (answer?.files.length !== 0) {
                for (let file of answer?.files) {
                    data.append("documents", file);
                }
            }

            let res = await fetch(ChatsApi, {
                method: "POST",
                body: data,
                credentials: "include",
            });

            if (res.ok) {
                notifications.add({
                    type: "ok",
                    text: "Ответ отправлен",
                });
            } else {
                notifications.add({
                    type: "error",
                    text: res.statusText,
                });
            }
        } catch (err) {
            notifications.add({
                type: "error",
                text: "Ошибка при отправке сообщения",
            });
        }
    }

    let search = $state("");
    let selectedChat = $state(null);
    let isOpen = $state(false);
    let highlightedIndex = $state(-1);

    function selectChat(chat) {
        selectedChat = chat;
        isOpen = false;
        search = "";
        highlightedIndex = -1;
    }

    function filteredChats() {
        return chats.filter(chat => 
                chat.student.email.toLowerCase().includes(search.toLowerCase())
                    || chat.student.info.group.number.toLowerCase().includes(search.toLowerCase())
                    || chat.student.info.surname.toLowerCase().includes(search.toLowerCase())
                    || chat.student.info.name.toLowerCase().includes(search.toLowerCase())
                    || chat.student.info.patronymic.toLowerCase().includes(search.toLowerCase())
        );
    }
    
    function handleKeydown(event) {
        let chatsList = filteredChats();
        
        if (event.key === "ArrowDown") {
            event.preventDefault();
            highlightedIndex = (highlightedIndex + 1) % chatsList.length;
        } else if (event.key === "ArrowUp") {
            event.preventDefault();
            highlightedIndex = (highlightedIndex - 1 + chatsList.length) % chatsList.length;
        } else if (event.key === "Enter") {
            event.preventDefault();
            if (isOpen && highlightedIndex >= 0) {
                selectChat(chatsList[highlightedIndex]);
            } else {
                isOpen = !isOpen;
            }
        } else if (event.key === "Escape") {
            isOpen = false;
        } else if (event.key === "Tab") {
            isOpen = false;
        }
    }
    
    let selectedFiles: FileList | undefined = $state();
    let fileNames = $derived(selectedFiles ? Array.from(selectedFiles).map((file) => file.name).join(", "): "Файлы не выбраны");
</script>

<ToastNotifications bind:this={notifications} />

{#if errorMessage != ""}
    <ErrorAlert title="Ошибка">{errorMessage}</ErrorAlert>
{:else}
    <Breadcrumbs
        pathItems={[
            { isHome: true },
            { name: "База знаний", href: "/base" },
            { name: "Детали" },
        ]} />
    <div class="flex flex-wrap md:flex-nowrap gap-4 m-4">
        <Panel class="w-full md:w-7/12">
            <Heading
                tag="h1"
                class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                Редактировать вопрос
            </Heading>
    
            <div class="mb-5">
                <Label class="space-y-2 mb-2">Категория</Label>
                <div class="flex space-x-3">
                    <select id="topic"
                        class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg mr-auto
                            focus:ring-orange-500 focus:border-orange-500 block w-1/2 p-2.5 dark:bg-gray-700 dark:border-gray-600
                            dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                        {#each categories.content as category}
                            {#if category.id === question.id_category}
                                <option selected value={category.id}>{category.name}</option>
                            {:else}
                                <option value={category.id}>{category.name}</option>
                            {/if}
                        {/each}
                    </select>
                    <Button
                        size="md"
                        class="text-nowrap"
                        on:click={() => deleteOpen = true}>Удалить вопрос</Button>
                </div>
                <Modal bind:open={deleteOpen} size="xs" autoclose>
                    <div class="text-center">
                        <ExclamationCircleOutline
                            class="mx-auto mb-4 text-gray-400 w-12 h-12 dark:text-gray-200" />
                        <h3
                            class="mb-5 text-lg font-normal text-gray-500 dark:text-gray-400">
                            Вы уверены, что хотите удалить вопрос?
                        </h3>
                        <Button
                            color="red"
                            class="me-2"
                            on:click={() => deleteQuestion()}>Да, удалить</Button>
                        <Button color="alternative">Нет, не удалять</Button>
                    </div>
                </Modal>
            </div>
    
            <div class="mb-5">
                <Label class="mb-2">Вопрос</Label>
                <Textarea
                    bind:value={newQuestion}
                    placeholder="Введите текст"
                    rows={4} />
            </div>
    
            <div class="mb-5">
                <Label class="mb-2">Ответ</Label>
                <Textarea
                    bind:value={newAnswer}
                    placeholder="Введите текст"
                    rows={4} />
            </div>
    
            <div class="flex justify-center">
                <Button on:click={() => updateQuestion()} size="lg" class="mb-1 justify-center">Сохранить</Button>
            </div>
        </Panel>
        <div class="w-full md:w-5/12 flex flex-col">
            <Panel class="mb-4">
                <Heading
                    tag="h2"
                    class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                    Информация
                </Heading>
                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                    <b>Создано:</b>
                    {new Date(question?.created_at).toLocaleString("ru")}
                </p>
            </Panel>
            <Panel class="mb-4 flex flex-col flex-1">
                <Heading
                    tag="h2"
                    class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                    Файлы
                </Heading>
                <div class="mb-3">
                    <Fileupload clearable bind:files={selectedFiles} multiple />
                    <Helper class="mt-2">{fileNames}</Helper>
                </div>
                <div class="flex gap-1 flex-col items-baseline">
                    {#each answer?.files as file}
                        <div class="flex space-x-2 items-center">
                            <A href={`${Base}}/file/${file.id}/download/`}>
                                {file.filename}
                            </A>
                            <Button
                                on:click={() => removeFile(file.id)}
                                outline
                                color="red"
                                size="xs"
                                class="w-5 h-5">
                                <CloseOutline />
                            </Button>
                        </div>
                    {/each}
            </Panel>
            <Panel>
                <Label class="mb-2">Отправить ответ на вопрос студенту</Label>
                <div class="relative w-full mb-5 select-none">
                    <button
                        type="button"
                        class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg p-2.5 
                               focus:ring-orange-500 focus:border-orange-500 focus:outline-1 focus:outline-orange-500 dark:bg-gray-700 dark:border-gray-600 
                               dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500 
                               cursor-pointer flex justify-between items-center w-full"
                        onclick={() => isOpen = !isOpen}
                        onkeydown={handleKeydown}
                        role="combobox"
                        aria-haspopup="listbox"
                        aria-expanded={isOpen}
                        aria-controls="chat-list"
                    >
                        {#if selectedChat}
                            {selectedChat.student.email}
                        {:else}
                            Выберите чат
                        {/if}
                        <svg class="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
                        </svg>
                    </button>
                    {#if isOpen}
                        <div class="absolute mt-1 w-full bg-white border border-gray-300 dark:border-gray-600 rounded-lg shadow-lg z-10 dark:bg-gray-700">
                            <Input
                                type="text"
                                bind:value={search}
                                placeholder="Поиск"
                                onkeydown={handleKeydown}
                            />
                            <ul class="max-h-60 overflow-y-auto" role="listbox">
                                {#if chats.length === 0}
                                    <li
                                        class="py-2 px-3">
                                        Нет чатов
                                    </li>
                                {/if}
                                {#each filteredChats() as chat, index}
                                    <li
                                        class="py-2 px-3 cursor-pointer flex justify-between hover:rounded {index === highlightedIndex ? 'bg-gray-200 dark:bg-gray-600' : 'hover:bg-gray-100 dark:hover:bg-gray-600'}"
                                        onclick={() => selectChat(chat)}
                                        role="option"
                                        aria-selected={index === highlightedIndex}
                                        onkeydown={(e) => e.key === "Enter" && selectChat(chat)}
                                    >
                                        <div>
                                            {chat.student.email}
                                        </div>
                                        <div>
                                            {chat.student.info.surname} {chat.student.info.name[0]}. {chat.student.info.patronymic[0]}. 
                                        </div>
                                        <div>
                                            {chat.student.info.group.number}
                                        </div>
                                    </li>
                                {/each}
                            </ul>
                        </div>
                    {/if}
                </div>
                <Button class="ml-auto" size="md" on:click={() => send()}>Отправить</Button>
            </Panel>
        </div>
    </div>
{/if}
