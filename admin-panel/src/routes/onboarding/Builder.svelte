<script lang='ts'>
	import { Breadcrumb, BreadcrumbItem, Heading, Accordion, AccordionItem, Input, Label, Button, Helper } from 'flowbite-svelte';
    import { onMount } from 'svelte';
    import { Link } from "svelte-routing";

    let courses;

    let success = '';

    onMount(async () => {
        let response;
        try {
            response = await fetch("http://localhost:5000/parser/bot/onboard/", {
                credentials: "include",
            });
        } catch (err) { }
        let json = await response?.json();
        courses = json;
    });

    let name = '';
    let topic = '';
    let callback = '';
    let topics = '';
    let text = '';
    let question = null;
    let answer = null;
  
    async function save(id) {
        console.log(id)
        let res;
        for (let course of courses) {
            if (course._id === id) {
                res = course;
            }
            else return;
        }

        success = "Сохранение...";

        let body = JSON.stringify(res);
  
        let result = await fetch(`http://localhost:5000/parser/bot/onboard/${id}`, {
            method: "PUT",
            headers: {
                'Accept': 'application/json, */*',
                'Content-Type': 'application/json'
            },
            body: body,
            credentials: "include",
        });
  
        if (result.ok) {
            success = "Сохраниено";
        } else {
            success = "Ошибка";
        }
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
                    <BreadcrumbItem>Конструктор онбординга</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                    Конструктор онбординга
                </Heading>
                <Button on:click={save}>
                    Добавить курс
                </Button>
                <Helper class="mb-4 mt-1"></Helper>
                {#each courses as course}
                    <Accordion class='mb-5'>
                        <AccordionItem open>
                            <span slot="header">{course.name}</span>
                            <Label class="space-y-2 mb-5">
                                <span>Название</span>
                                <Input bind:value={name} class='border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700' placeholder={course.name} size="md" />
                            </Label>
                            <div class="mb-5 space-x-4">
                                <Button>Добавить раздел</Button>
                                <Button>Курс вверх</Button>
                                <Button>Курс вниз</Button>
                                <Button>Удалить курс</Button>
                            </div>
                            {#each course.sections as section}
                                <Accordion>
                                    <AccordionItem>
                                        <span slot="header">{section.name}</span>
                                        <Label class="space-y-2 mb-3 w-full mr-5">
                                            <span>Название</span>
                                            <Input bind:value={section.name} class='border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700' placeholder={section.name} size="md" />
                                        </Label>
                                        <Label class="space-y-2 mb-5 w-full">
                                            <span>Callback</span>
                                            <Input bind:value={section.callback_data} class='border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700' placeholder={section.callback_data} size="md" />
                                        </Label>
                                        <div class="mb-5 space-x-4">
                                            <Button>Добавить тему</Button>
                                            <Button>Раздел вверх</Button>
                                            <Button>Раздел вниз</Button>
                                            <Button>Удалить раздел</Button>
                                        </div>
                                        {#each section.topics as topic}
                                            <Accordion>
                                                <AccordionItem>
                                                    <span slot="header">{topic.name}</span>
                                                    <Label class="space-y-2 mb-5 w-full mr-5">
                                                        <span>Название</span>
                                                        <Input bind:value={topic.name} class='border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700' placeholder={topic.name} size="md" />
                                                    </Label>
                                                    <div class="mb-3 space-x-4">
                                                        <Button>Тема вверх</Button>
                                                        <Button>Тема вниз</Button>
                                                        <Button>Удалить тему</Button>
                                                    </div>
                                                    <Label class="space-y-2 mb-3 w-full">
                                                        <span>Текст</span>
                                                        <Input bind:value={topic.text} class='border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700' placeholder={topic.text} size="md" />
                                                    </Label>
                                                    <Label class="space-y-2 mb-3 w-full mr-5">
                                                        <span>Вопрос</span>
                                                        <Input bind:value={topic.question} class='border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700' placeholder={topic.question} size="md" />
                                                    </Label>
                                                    <Label class="space-y-2 w-full">
                                                        <span>Ответ</span>
                                                        <Input bind:value={topic.answer} class='border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700' placeholder={topic.answer} size="md" />
                                                    </Label>
                                                </AccordionItem>
                                            </Accordion>
                                        {/each}
                                    </AccordionItem>
                                </Accordion>
                            {/each}
                            <div class="w-full">
                                <Button class='mt-5 flex justify-end' on:click={() => save(course._id)}>
                                    Сохранить изменения
                                </Button>
                                <Helper class="mt-1">{success}</Helper>
                            </div>
                        </AccordionItem>
                    </Accordion>
                {/each}
            </div>
        </div>
    </div>
</div>

<style>
</style>