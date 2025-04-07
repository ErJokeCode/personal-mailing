<script lang="ts">
    import {
        Heading,
        Button,
        Textarea,
        Label,
        Select,
        Fileupload,
        Helper,
    } from "flowbite-svelte";
    import Panel from "/src/lib/components/Panel.svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
    import { onMount } from "svelte";
    import { Me } from "/src/stores/Me.svelte";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import { Base } from "/src/lib/server";

    let notifications: ToastNotifications;

    let categories = $state([]);

    let selected = $state('Не выбрано');

    let question = $state('');
    let answer = $state('');

    onMount(async () => {
        let response = await fetch(`${Base}/categories/`, {
                method: 'GET',
                credentials: "include",
            });
        let json = await response?.json();
        categories = json;
        if (categories.length === 0) selected = 'Нет категорий'
    });

    const add = async () => {
        if (selected === 'Нет категорий') {
            notifications.add({
                        type: "error",
                        text: "Добавьте категорию",
                    });
            return;
        }
        if (selected === 'Не выбрано') {
            notifications.add({
                        type: "error",
                        text: "Выберите категорию",
                    });
            return;
        }
        if (question === '') {
            notifications.add({
                        type: "error",
                        text: "Заполните данные",
                    });
            return;
        }
        let body = {
            question: question,
            answer: answer,
            tutor_id: 1,
            category_id: Number(document.getElementById('topic').value),
        };
            let response = await fetch(`${Base}/knowledge-items/`, {
                method: 'POST',
                headers: {
                    Accept: "application/json, */*",
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(body),
                credentials: "include",
            });
            if (response.ok) {
                notifications.add({
                            type: "ok",
                            text: "Вопрос успешно добавлен",
                        });
            } else {
                notifications.add({
                    type: "error",
                    text: response.statusText,
                })
            }
    }
    
    let selectedFiles: FileList | undefined = $state();
    let fileNames = $derived(selectedFiles ? Array.from(selectedFiles).map((file) => file.name).join(", "): "Файлы не выбраны");
</script>

<ToastNotifications bind:this={notifications} />

<Panel class="m-4">
    <Breadcrumbs
        pathItems={[
            { isHome: true },
            { name: "База знаний", href: "/base" },
            { name: "Добавить вопрос" },
        ]} />

    <Heading
        tag="h1"
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
        Добавить вопрос
    </Heading>

    <form>
        <Label class="space-y-2 mb-2">Категория</Label>
        <div class='flex space-x-3'>
            <div class="w-1/2">
                {#if categories.length !== 0}
                    <select id="topic"
                        bind:value={selected}
                        class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg
                            focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600
                            dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                        <option selected disabled value='Не выбрано'>Не выбрано</option>
                        {#each categories as category}
                            <option value={category.id}>{category.name}</option>
                        {/each}
                    </select>
                {:else}
                    <Select disabled placeholder='Нет категорий' />
                {/if}
            </div>
            <div class="w-1/2">
                <Fileupload clearable bind:files={selectedFiles} multiple />
                <Helper class="mt-2">{fileNames}</Helper>
            </div>
        </div>
    </form>

    <div class="mb-5">
        <Label class="mb-2">Вопрос</Label>
        <Textarea
            bind:value={question}
            placeholder="Введите текст"
            rows={4} />
    </div>

    <div class="mb-5">
        <Label class="mb-2">Ответ (опционально)</Label>
        <Textarea
            bind:value={answer}
            placeholder="Введите текст"
            rows={4} />
    </div>

    <div class="flex justify-center">
        <Button on:click={() => add()} size="lg" class="mb-5">Добавить вопрос</Button>
    </div>
</Panel>
