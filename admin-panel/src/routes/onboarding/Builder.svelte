<script lang="ts">
    import Panel from "/src/lib/components/Panel.svelte";
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Accordion,
        AccordionItem,
        Input,
        Label,
        Button,
        Helper,
        Checkbox,
    } from "flowbite-svelte";
    import { onMount } from "svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
    import { ServerUrl } from "/src/lib/server";

    let courses = $state([]);
    let success = $state("");

    onMount(async () => {
        load();
    });

    const load = async () => {
        let response;
        try {
            response = await fetch(`${ServerUrl}/parser/bot/onboard/`, {
                credentials: "include",
            });
        } catch (err) {
            console.log(err);
        }
        let json = await response?.json();
        courses = json;
    };

    async function save(id) {
        let res = null;
        for (let course of courses) {
            if (course._id === id) {
                res = course;
            }
        }
        if (res === null) return;

        console.log("Сохранение...");

        let body = JSON.stringify(res);

        let result = await fetch(`${ServerUrl}/parser/bot/onboard/${id}`, {
            method: "PUT",
            headers: {
                Accept: "application/json, */*",
                "Content-Type": "application/json",
            },
            body: body,
            credentials: "include",
        });

        if (result.ok) {
            console.log("Сохранено");
        } else {
            console.log("Ошибка");
        }
    }

    const add_course = async () => {
        let is_active = true;
        let is_main = false;

        let new_course = {
            name: "Новый курс",
            is_main: is_main,
            is_active: is_active,
            sections: [
                {
                    name: "Новый раздел",
                    callback_data: "data",
                    topics: [
                        {
                            name: "Новая тема",
                            text: "Текст",
                            question: null,
                            answer: null,
                        },
                    ],
                },
            ],
        };
        let body = JSON.stringify(new_course);

        let result = await fetch(`${ServerUrl}/parser/bot/onboard/one_course`, {
            method: "POST",
            headers: {
                Accept: "application/json, */*",
                "Content-Type": "application/json",
            },
            body: body,
            credentials: "include",
        });
        load();
    };

    const delete_course = async (id) => {
        let result = await fetch(`${ServerUrl}/parser/bot/onboard/${id}`, {
            method: "DELETE",
            credentials: "include",
        });
        load();
    };

    const add_section = async (index) => {
        let new_section = {
            name: "Новый раздел",
            callback_data: "data",
            topics: [
                {
                    name: "Новая тема",
                    text: "Текст",
                    question: null,
                    answer: null,
                },
            ],
        };
        courses[index] = courses[index];
        courses[index].sections.push(new_section);
    };

    const delete_section = async (index, section_index) => {
        courses[index] = courses[index];
        courses[index].sections.splice(section_index, 1);
    };

    const add_topic = async (index, section_index) => {
        let new_topic = {
            name: "Новая тема",
            text: "Текст",
            question: null,
            answer: null,
        };
        courses[index] = courses[index];
        courses[index].sections[section_index].topics.push(new_topic);
    };

    const delete_topic = async (index, section_index, topic_index) => {
        courses[index] = courses[index];
        courses[index].sections[section_index].topics.splice(topic_index, 1);
    };

    const section_up = async (course_index, section_index, k = 0, m = -1) => {
        let isec;
        let sections;
        for (let i = 0; i < courses.length; i++) {
            if (i === course_index) {
                let tsec = courses[i].sections;
                for (let j = 0; j < tsec.length; j++) {
                    if (j === section_index) {
                        k = k == 0 ? 0 : tsec.length - 1;
                        if (j == k) return;
                        isec = j;
                        sections = tsec;
                    }
                }
            }
        }
        courses[course_index] = courses[course_index];
        let tsec = sections[isec];
        sections[isec] = sections[isec + m];
        sections[isec + m] = tsec;
    };

    const section_down = async (course_index, section_name) => {
        section_up(course_index, section_name, 1, 1);
    };

    const topic_up = async (
        course_index,
        section_index,
        topic_index,
        n = 0,
        m = -1,
    ) => {
        let itop;
        let topics;
        for (let i = 0; i < courses.length; i++) {
            if (i === course_index) {
                let tsec = courses[i].sections;
                for (let j = 0; j < tsec.length; j++) {
                    if (j === section_index) {
                        let ttop = tsec[j].topics;
                        for (let k = 0; k < ttop.length; k++) {
                            if (k === topic_index) {
                                n = n == 0 ? 0 : ttop.length - 1;
                                if (k == n) return;
                                itop = k;
                                topics = ttop;
                            }
                        }
                    }
                }
            }
        }
        courses[course_index] = courses[course_index];
        let ttop = topics[itop];
        topics[itop] = topics[itop + m];
        topics[itop + m] = ttop;
    };

    const topic_down = async (course_index, section_name, topic_index) => {
        topic_up(course_index, section_name, topic_index, 1, 1);
    };
</script>

<Panel class="m-4">
    <Breadcrumbs pathItems={[{ isHome: true }, { name: "Конструктор онбординга" }]} />
    <Heading
        tag="h1"
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
        Конструктор онбординга
    </Heading>
    <Button on:click={add_course} class="max-w-fit">Добавить курс</Button>
    <Helper class="mb-4 mt-1"></Helper>
    <Accordion class='mb-5' multiple classActive='dark:bg-gray-600 dark:focus:ring-gray-700'>
        {#each courses as course, course_index}
            <AccordionItem borderOpenClass='border-s border-e border-b border-gray-200 dark:border-gray-700'>
                <span slot="header">{course.name}</span>
                <Label class="mb-5">
                    <span>Название</span>
                    <Input
                        bind:value={course.name}
                        class="border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 mt-2"
                        placeholder={course.name}
                        size="md" />
                </Label>
                <div class="mb-5 flex space-x-4">
                    <Checkbox
                        checked={course.is_main}
                        groupInputClass="ms-2"
                        on:click={() => (course.is_main = !course.is_main)}
                        >Главный курс</Checkbox>
                    <Checkbox
                        checked={course.is_active}
                        groupInputClass="ms-2"
                        on:click={() => (course.is_active = !course.is_active)}
                        >Активный курс</Checkbox>
                </div>
                <div class="space-x-4">
                    <Button class='mb-5' on:click={() => add_section(course_index)}
                        >Добавить раздел</Button>
                    <Button class='mb-5' on:click={() => delete_course(course._id)}
                        >Удалить курс</Button>
                </div>
                <Accordion class='mb-5' multiple classActive='dark:bg-gray-600 dark:focus:ring-gray-700'>
                    {#each course.sections as section, section_index}
                        <AccordionItem borderOpenClass='border-s border-e border-b border-gray-200 dark:border-gray-700'>
                            <span slot="header">{section.name}</span>
                            <Label class="mb-3 w-full mr-5">
                                <span>Название</span>
                                <Input
                                    bind:value={section.name}
                                    class="border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 mt-2"
                                    placeholder={section.name}
                                    size="md" />
                            </Label>
                            <Label class="mb-5 w-full">
                                <span>Callback data</span>
                                <Input
                                    bind:value={section.callback_data}
                                    class="border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 mt-2"
                                    placeholder={section.callback_data}
                                    size="md" />
                            </Label>
                            <div class="space-x-4">
                                <Button class='mb-5'
                                    on:click={() =>
                                        add_topic(course_index, section_index)}
                                    >Добавить тему</Button>
                                <Button class='mb-5'
                                    on:click={() =>
                                        section_up(course_index, section_index)}
                                    >Раздел вверх</Button>
                                <Button class='mb-5'
                                    on:click={() =>
                                        section_down(
                                            course_index,
                                            section_index,
                                        )}>Раздел вниз</Button>
                                <Button class='mb-5'
                                    on:click={() =>
                                        delete_section(
                                            course_index,
                                            section_index,
                                        )}>Удалить раздел</Button>
                            </div>
                            <Accordion class='mb-5' multiple classActive='dark:bg-gray-600 dark:focus:ring-gray-700'>
                                {#each section.topics as topic, topic_index}
                                    <AccordionItem borderOpenClass='border-s border-e border-b border-gray-200 dark:border-gray-700'>
                                        <span slot="header">{topic.name}</span>
                                        <Label
                                            class="mb-5 w-full mr-5">
                                            <span>Название</span>
                                            <Input
                                                bind:value={topic.name}
                                                class="border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 mt-2"
                                                placeholder={topic.name}
                                                size="md" />
                                        </Label>
                                        <div class="space-x-4">
                                            <Button class='mb-3'
                                                on:click={() =>
                                                    topic_up(
                                                        course_index,
                                                        section_index,
                                                        topic_index,
                                                    )}>Тема вверх</Button>
                                            <Button class='mb-3'
                                                on:click={() =>
                                                    topic_down(
                                                        course_index,
                                                        section_index,
                                                        topic_index,
                                                    )}>Тема вниз</Button>
                                            <Button class='mb-3'
                                                on:click={() =>
                                                    delete_topic(
                                                        course_index,
                                                        section_index,
                                                        topic_index,
                                                    )}>Удалить тему</Button>
                                        </div>
                                        <Label class="mb-3 w-full">
                                            <span>Текст</span>
                                            <Input
                                                bind:value={topic.text}
                                                class="border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 mt-2"
                                                placeholder={topic.text}
                                                size="md" />
                                        </Label>
                                        <Label
                                            class="mb-3 w-full mr-5">
                                            <span>Вопрос</span>
                                            <Input
                                                bind:value={topic.question}
                                                class="border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 mt-2"
                                                placeholder={topic.question}
                                                size="md" />
                                        </Label>
                                        <Label class="w-full">
                                            <span>Ответ</span>
                                            <Input
                                                bind:value={topic.answer}
                                                class="border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 mt-2"
                                                placeholder={topic.answer}
                                                size="md" />
                                        </Label>
                                    </AccordionItem>
                                {/each}
                            </Accordion>
                        </AccordionItem>
                    {/each}
                </Accordion>
                <div class="w-full">
                    <Button
                        class="mt-5 flex justify-end"
                        on:click={() => save(course._id)}>
                        Сохранить изменения
                    </Button>
                    <Helper class="mt-1">{success}</Helper>
                </div>
            </AccordionItem>
        {/each}
    </Accordion>
</Panel>

<style>
</style>
