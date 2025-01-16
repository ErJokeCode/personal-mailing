<script lang='ts'>
	import { Breadcrumb, BreadcrumbItem, Heading, Accordion, AccordionItem, Textarea, Label, Button, Helper, Tabs, TabItem, Input } from 'flowbite-svelte';
    import { onMount } from 'svelte';
    import { Link } from "svelte-routing";
    import { CirclePlusOutline } from 'flowbite-svelte-icons'
    import http from "../../utils/http";
    import { server_url } from "../../utils/store.js"

    let qStatus = http.status();

    let topics = $state([]);

    let success = '';

    onMount(async () => {
        load();
    });

    const load = async () => {
        qStatus.start_load();
        topics = (await http.get("/parser/bot/faq/", qStatus)) ?? [];
        qStatus.end_load();
        console.log(questions)
    }

    const add_topic = async () => {
        let name = "new_topic";
        let i = 0;
        topics.forEach(element => {
            let sp = element.name.split("__")
            if (sp[0] == name) {
                if (sp.length == 1) {
                    i = 0
                }
                else if(i < parseInt(sp[1])) {
                    i = parseInt(sp[1])
                }
           }
        });

        name = name + "__" + (i + 1).toString();

        qStatus.start_load();
        let topic = (await http.post("/parser/bot/faq/?name_topic=" + name, {}, qStatus)) ?? {};
        topics = [...topics, topic];
        qStatus.end_load();   
    }

    const add_question = async (topic) => {
        qStatus.start_load();
        let new_faqs = [...topic.faqs, {"question": "Новый вопрос", "answer": ""}]
        let update_topic = (await http.put_json("/parser/bot/faq/" + topic._id, {"name" : topic.name, "faqs": new_faqs}, qStatus)) ?? {};
        topics = (await http.get("/parser/bot/faq/", qStatus)) ?? [];
        qStatus.end_load();
    }

    const save_topic_name = async(topic) => {
        qStatus.start_load();
        let new_topic = (await http.put_json("/parser/bot/faq/" + topic._id, {"name" : topic.name, "faqs": topic.faqs}, qStatus)) ?? {};
        qStatus.end_load();
    }

    const delete_topic = async(topic) => {
        await fetch(`${server_url}/parser/bot/faq/${topic._id}`, {
            method: "DELETE",
            credentials: "include",
        });
        topics = (await http.get("/parser/bot/faq/", qStatus)) ?? [];
    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="pt-4 px-6">
                <Breadcrumb class="mb-5">
                    <Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link>
                    <BreadcrumbItem>Редактор FAQ</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-1">
                    Редактор FAQ
                </Heading>
                <Tabs tabStyle="underline">
                    {#each topics as topic}
                        <TabItem title={topic.name} open>
                            <Label for="name" class="mb-2">Название раздела</Label>
                            <div class="flex mb-10">
                                <Input class="mr-2" type="text" placeholder="Введите название" bind:value={topic.name}></Input>
                                <Button class='' on:click={() => save_topic_name(topic)}>
                                    Сохранить
                                </Button>
                            </div>
                            
                            <Button on:click={() => add_question(topic)}>
                                Добавить вопрос
                            </Button>
                            <Helper class="mb-4 mt-1"></Helper>
                            <Accordion class='mb-5'>
                                {#each topic.faqs as question}
                                    <AccordionItem>
                                        <span slot="header">{question.question}</span>
                                        <Label for="question" class="mb-2">Вопрос</Label>
                                        <Textarea bind:value={question.question} id="question" placeholder="Введите текст" rows="4" name="message" class='dark:bg-gray-700 mb-4' />
                                        <Label for="answer" class="mb-2">Ответ</Label>
                                        <Textarea bind:value={question.answer} id="answer" placeholder="Введите текст" rows="4" name="message" class='dark:bg-gray-700' />
                                    </AccordionItem>
                                {/each}
                            </Accordion>
                            <div class="flex justify-center">
                                <Button class='' on:click={() => delete_topic(topic)}>
                                    Удалить раздел
                                </Button>
                            </div>
                        </TabItem>
                    {/each}
                    <Button class='mt-2 w-9 h-9' pill size='sm' on:click={() => add_topic()}><CirclePlusOutline size='lg'/></Button>
                </Tabs>
            </div>
        </div>
    </div>
</div>

<style>
</style>