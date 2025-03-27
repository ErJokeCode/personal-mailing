<script lang="ts">
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Button,
        Textarea,
        Label,
    } from "flowbite-svelte";
    import Panel from "/src/lib/components/Panel.svelte";
    import { goto, route } from "@mateothegreat/svelte5-router";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";

    let topic;
</script>

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

    <form class="mb-5">
        <Label class="space-y-2 mb-2">Тема</Label>
        <select
            bind:value={topic}
            class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg
                focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600
                dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
            <option selected value="Общие вопросы">Общие вопросы</option>
            <option value="Технические вопросы">Технические вопросы</option>
            <option value="Административные вопросы"
                >Административные вопросы</option>
        </select>
    </form>

    <div class="mb-5">
        <Label for="textarea-id" class="mb-2">Вопрос</Label>
        <Textarea
            id="textarea-id"
            placeholder="Введите текст"
            rows={4}
            name="message" />
    </div>

    <div class="mb-5">
        <Label for="textarea-id" class="mb-2">Ответ (опционально)</Label>
        <Textarea
            id="textarea-id"
            placeholder="Введите текст"
            rows={4}
            name="message" />
    </div>

    <div class="flex justify-center">
        <Button size="lg" class="mb-10">Добавить вопрос</Button>
    </div>
</Panel>
