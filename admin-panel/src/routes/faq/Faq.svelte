<script lang='ts'>
	import { Breadcrumb, BreadcrumbItem, Heading, Accordion, AccordionItem, Textarea, Label, Button, Helper, Tabs, TabItem } from 'flowbite-svelte';
    import { onMount } from 'svelte';
    import { Link } from "svelte-routing";
    import { CirclePlusOutline } from 'flowbite-svelte-icons'

    let questons;
    let topics = [{'theme': 'Тема 1'}, {'theme': 'Тема 2'}, {'theme': 'Тема 3'}];

    let success = '';

    onMount(async () => {
        load();
    });

    const load = async () => {
        // let response;
        // try {
        //     response = await fetch("http://localhost:5000/parser/bot/onboard/", {
        //         credentials: "include",
        //     });
        // } catch (err) {
        //     console.log(err)
        // }
        // let json = await response?.json();
        // courses = json;
    }

    const add_tab = async () => {

    }

    const add_question = async () => {

    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="p-4 px-6">
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
                        <TabItem title={topic.theme}>
                            <Button on:click={add_question}>
                                Добавить вопрос
                            </Button>
                            <Helper class="mb-4 mt-1"></Helper>
                            <!-- {#each topic.questons as question} -->
                                <Accordion class='mb-5'>
                                    <AccordionItem>
                                        <span slot="header">Вопрос 1</span>
                                        <Label for="question" class="mb-2">Вопрос</Label>
                                        <Textarea id="question" placeholder="Введите текст" rows="4" name="message" class='dark:bg-gray-700 mb-4' />
                                        <Label for="answer" class="mb-2">Ответ</Label>
                                        <Textarea id="answer" placeholder="Введите текст" rows="4" name="message" class='dark:bg-gray-700' />
                                    </AccordionItem>
                                </Accordion>
                                <Accordion class='mb-5'>
                                    <AccordionItem>
                                        <span slot="header">Вопрос 2</span>
                                        <Label for="question" class="mb-2">Вопрос</Label>
                                        <Textarea id="question" placeholder="Введите текст" rows="4" name="message" class='dark:bg-gray-700 mb-4' />
                                        <Label for="answer" class="mb-2">Ответ</Label>
                                        <Textarea id="answer" placeholder="Введите текст" rows="4" name="message" class='dark:bg-gray-700' />
                                    </AccordionItem>
                                </Accordion>
                            <!-- {/each} -->
                        </TabItem>
                    {/each}
                    <Button class='mt-2 w-9 h-9' pill size='sm' on:click={add_tab}><CirclePlusOutline size='lg'/></Button>
                </Tabs>
            </div>
        </div>
    </div>
</div>

<style>
</style>
