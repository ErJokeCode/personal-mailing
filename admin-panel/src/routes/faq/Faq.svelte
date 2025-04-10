<script lang="ts">
    import {
        Heading,
        Accordion,
        AccordionItem,
        Textarea,
        Label,
        Button,
        Tabs,
        TabItem,
        Input,
    } from "flowbite-svelte";
    import { onMount } from "svelte";
    import { CirclePlusOutline } from "flowbite-svelte-icons";
    import http from "/src/lib/utils/http";
    import { ServerUrl } from "/src/lib/server";
    import Panel from "/src/lib/components/Panel.svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";

    let notifications: ToastNotifications;

    let qStatus = http.status();
    let topics = $state([]);

    onMount(async () => {
        load();
    });

    const load = async () => {
        qStatus.start_load();
        topics = (await http.get("/parser/bot/faq/", qStatus)) ?? [];
        qStatus.end_load();
    };

    const add_topic = async () => {
        let name = "new_topic";
        let i = 0;
        topics.forEach((element) => {
            let sp = element.name.split("__");
            if (sp[0] == name) {
                if (sp.length == 1) {
                    i = 0;
                } else if (i < parseInt(sp[1])) {
                    i = parseInt(sp[1]);
                }
            }
        });

        name = name + "__" + (i + 1).toString();

        qStatus.start_load();
        let topic =
            (await http.post(
                "/parser/bot/faq/?name_topic=" + name,
                {},
                qStatus,
            )) ?? {};
        topics = [...topics, topic];
        qStatus.end_load();
    };

    const add_question = async (topic) => {
        qStatus.start_load();
        let new_faqs = [
            ...topic.faqs,
            { question: "Новый вопрос", answer: "" },
        ];
        await http.put_json(
                "/parser/bot/faq/" + topic._id,
                { name: topic.name, faqs: new_faqs },
                qStatus,
            );
        topics = (await http.get("/parser/bot/faq/", qStatus)) ?? [];
        qStatus.end_load();
    };

    const save_topic_name = async (topic) => {
        qStatus.start_load();
        let result = 
            await http.put_json(
                "/parser/bot/faq/" + topic._id,
                { name: topic.name, faqs: topic.faqs },
                qStatus,
            );
        if (result = "✓") {
            notifications.add({
                type: "ok",
                text: "Сохранено"
            });
        } else {
            notifications.add({
                type: "error",
                text: "Ошибка"
            });
        }
        qStatus.end_load();
    };

    const delete_topic = async (topic) => {
        await fetch(`${ServerUrl}/parser/bot/faq/${topic._id}`, {
            method: "DELETE",
            credentials: "include",
        });
        topics = (await http.get("/parser/bot/faq/", qStatus)) ?? [];
    };
</script>

<ToastNotifications bind:this={notifications} />

<Breadcrumbs pathItems={[{ isHome: true }, { name: "Редактор FAQ" }]} />

<Panel class="m-4">
    <Heading
        tag="h1"
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-2">
        Редактор FAQ
    </Heading>
    <Tabs tabStyle="underline">
        {#each topics as topic}
            <TabItem title={topic.name}>
                <Label for="name" class="mb-2">Название раздела</Label>
                <div class="flex mb-5">
                    <Input
                        class="mr-2"
                        type="text"
                        placeholder="Введите название"
                        bind:value={topic.name}></Input>
                    <Button
                        class="text-nowrap"
                        on:click={() => delete_topic(topic)}>
                        Удалить раздел
                    </Button>
                </div>
                <Button class="mb-4" on:click={() => add_question(topic)}>
                    Добавить вопрос
                </Button>
                <Accordion
                    class="mb-5"
                    multiple
                    classActive="dark:bg-gray-600 dark:focus:ring-gray-700">
                    {#each topic.faqs as question}
                        <AccordionItem
                            borderOpenClass="border-s border-e border-b border-gray-200 dark:border-gray-700">
                            <span slot="header">{question.question}</span>
                            <Label for="question" class="mb-2">Вопрос</Label>
                            <Textarea
                                bind:value={question.question}
                                id="question"
                                placeholder="Введите текст"
                                rows={4}
                                name="message"
                                class="dark:bg-gray-700 mb-4" />
                            <Label for="answer" class="mb-2">Ответ</Label>
                            <Textarea
                                bind:value={question.answer}
                                id="answer"
                                placeholder="Введите текст"
                                rows={4}
                                name="message"
                                class="dark:bg-gray-700" />
                        </AccordionItem>
                    {/each}
                </Accordion>
                <div class="flex justify-center">
                    <Button size="lg" on:click={() => save_topic_name(topic)}>
                        Сохранить
                    </Button>
                </div>
            </TabItem>
        {/each}
        <Button class="mt-2 w-9 h-9" pill size="sm" on:click={() => add_topic()}
            ><CirclePlusOutline size="lg" /></Button>
    </Tabs>
</Panel>

<style>
</style>
