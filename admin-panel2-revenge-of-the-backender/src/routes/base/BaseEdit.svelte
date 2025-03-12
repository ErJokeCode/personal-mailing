<script>
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Button,
        Textarea,
    } from "flowbite-svelte";
    import { Label, Modal } from "flowbite-svelte";
    import { goto, route } from "@mateothegreat/svelte5-router";
    import { ExclamationCircleOutline } from "flowbite-svelte-icons";
    import { onMount } from "svelte";
    import Panel from "/src/lib/components/Panel.svelte";
    import http from "/src/lib/utils/http";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";

    let topic;
    let email;

    let question = "ааааааааааааааа";
    let answer = "уууууууууууууу";

    let sendOpen = false;
    let deleteOpen = false;

    let status = http.status();
    let chats = [];

    onMount(async () => {
        status = status.start_load();
        chats = (await http.get("/core/chats", status)).items ?? [];
        status = status.end_load();
    });
</script>

<Panel class="m-4">
    <Breadcrumbs
        pathItems={[
            { isHome: true },
            { name: "База знаний", href: "/base" },
            { name: "Редактировать вопрос" },
        ]} />

    <Heading
        tag="h1"
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
        Редактировать вопрос
    </Heading>

    <form class="mb-5">
        <Label class="space-y-2 mb-2">Тема</Label>
        <div class="flex space-x-3">
            <select
                bind:value={topic}
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg mr-auto
                    focus:ring-orange-500 focus:border-orange-500 block w-1/2 p-2.5 dark:bg-gray-700 dark:border-gray-600
                    dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                <option selected value="Общие вопросы">Общие вопросы</option>
                <option value="Технические вопросы">Технические вопросы</option>
                <option value="Административные вопросы"
                    >Административные вопросы</option>
            </select>
            <Button size="md" on:click={() => (sendOpen = true)}
                >Отправить</Button>
            <Button
                size="md"
                class="text-nowrap"
                on:click={() => (deleteOpen = true)}>Удалить вопрос</Button>
        </div>

        <Modal
            title="Отправить ответ на вопрос студенту"
            bind:open={sendOpen}
            autoclose>
            <form>
                <Label class="mb-2">Выберите чат</Label>
                <select
                    bind:value={email}
                    class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg
                        focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600
                        dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                    {#each chats as chat}
                        <option value={chat.student.email}
                            >{chat.student.email}</option>
                    {/each}
                </select>
            </form>
            <svelte:fragment slot="footer">
                <Button>Отправить</Button>
                <Button color="alternative">Отменить</Button>
            </svelte:fragment>
        </Modal>
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
                    on:click={() => alert("ауауауауа")}>Да, удалить</Button>
                <Button color="alternative">Нет, не удалять</Button>
            </div>
        </Modal>
    </form>

    <div class="mb-5">
        <Label for="textarea-id" class="mb-2">Вопрос</Label>
        <Textarea
            bind:value={question}
            id="textarea-id"
            placeholder="Введите текст"
            rows={4}
            name="message" />
    </div>

    <div class="mb-5">
        <Label for="textarea-id" class="mb-2">Ответ</Label>
        <Textarea
            bind:value={answer}
            id="textarea-id"
            placeholder="Введите текст"
            rows={4}
            name="message" />
    </div>

    <div class="flex justify-center">
        <Button size="lg" class="mb-10">Сохранить</Button>
    </div>
</Panel>
