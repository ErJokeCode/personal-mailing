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
        let update_topic =
            (await http.put_json(
                "/parser/bot/faq/" + topic._id,
                { name: topic.name, faqs: new_faqs },
                qStatus,
            )) ?? {};
        topics = (await http.get("/parser/bot/faq/", qStatus)) ?? [];
        qStatus.end_load();
    };

    const save_topic_name = async (topic) => {
        qStatus.start_load();
        let new_topic =
            (await http.put_json(
                "/parser/bot/faq/" + topic._id,
                { name: topic.name, faqs: topic.faqs },
                qStatus,
            )) ?? {};
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

<div class="py-4 px-6">
    <Heading
        tag="h1"
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-1">
        Редактор FAQ
    </Heading>
    <Tabs
        tabStyle="underline"
        contentClass="p-4 bg-white rounded-lg dark:bg-gray-800 mt-4">
        {#each topics as topic}
            <TabItem title={topic.name}>
                <Label for="name" class="mb-2">Название раздела</Label>
                <div class="flex mb-10">
                    <Input
                        class="mr-2"
                        type="text"
                        placeholder="Введите название"
                        bind:value={topic.name}></Input>
                    <div>
                        <Button
                            class="h-full"
                            on:click={() => save_topic_name(topic)}>
                            Сохранить
                        </Button>
                    </div>
                </div>
                <Button class="mb-4" on:click={() => add_question(topic)}>
                    Добавить вопрос
                </Button>
                {#each topic.faqs as question}
                    <Accordion class="mb-5">
                        <AccordionItem>
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
                    </Accordion>
                {/each}
                <div class="flex justify-center">
                    <Button on:click={() => delete_topic(topic)}>
                        Удалить раздел
                    </Button>
                </div>
            </TabItem>
        {/each}
        <Button class="mt-2 w-9 h-9" pill size="sm" on:click={() => add_topic()}
            ><CirclePlusOutline size="lg" /></Button>
    </Tabs>
</div>

<style>
</style>
