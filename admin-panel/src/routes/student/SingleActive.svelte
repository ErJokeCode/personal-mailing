<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading, Tabs, TabItem, Accordion, AccordionItem } from 'flowbite-svelte';
	import { Button } from 'flowbite-svelte';
	import { ArrowRightOutline } from 'flowbite-svelte-icons';
	import { onMount } from "svelte";
    import { Link, navigate } from "svelte-routing";
    import http from "../../utils/http";

    export let id;

    let main_student = {};
    let student = {};
    let status = http.status();

    onMount(async () => {
        main_student = await http.get(`/core/student/${id}`, status) ?? {};
        student = main_student.info !== undefined && main_student.info !== null ? main_student.info : {};
    });

    async function start_chat(studentId) {
        navigate(`/chat/${studentId}`);
    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="p-4 px-6">
                <Breadcrumb class="mb-5">
                    <li class="inline-flex items-center"><Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link></li>
                    <li class="inline-flex items-center"><svg class="w-6 h-6 text-gray-400 rtl:-scale-x-100" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0
                        z" clip-rule="evenodd"></path></svg>
                    <Link class="ml-0 ms-1 text-sm font-medium text-gray-700 hover:text-gray-900 md:ms-2 dark:text-gray-400 dark:hover:text-white" to="/students/active">Активные студенты</Link></li>
                    <BreadcrumbItem>Детали</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3">
                    {student.name} {student.surname} {student.patronymic}
                </Heading>
                <Button class="mb-3"
                    on:click={() => start_chat(main_student.id)}>
                    Начать чат
                    <ArrowRightOutline class="w-6 h-6 ms-2" />
                </Button>
                <Tabs tabStyle='underline' contentClass='p-4 bg-white rounded-lg dark:bg-gray-800 mt-4'>
                    <TabItem open title="Основная информация">
                        <ul>
                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Номер студенческого билета:</b>
                                    {student.personalNumber}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Почта:</b>
                                    {student.email}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Дата рождения:</b>
                                    {student.dateOfBirth}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Группа:</b>
                                    {(student.group !== undefined) ? student.group.number : 'Не известен'}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Номер курса:</b>
                                    {(student.group !== undefined) ? student.group.numberCourse : 'Не известен'}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Направление:</b>
                                    {(student.group !== undefined) ? student.group.directionCode + " " + student.group.nameSpeciality : 'Не известно'}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Статус:</b>
                                    {student.status ? 'Активный студент' : 'Не активный студент'}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Тип оплаты обучения:</b>
                                    {student.typeOfCost}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Форма обучения:</b>
                                    {student.typeOfEducation}
                                </p>
                            </li>
                        </ul>
                    </TabItem>
                    <TabItem title="Предметы">
                        <Accordion>
                            {#each student.subjects as subject}
                                <AccordionItem>
                                    <span slot="header">{subject.name}</span>
                                    <ul>
                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Полное название:</b>
                                                {subject.fullName}
                                            </p>
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Команды:</b>
                                            </p>
                                            {#if subject.teams !== undefined}
                                                <ul style="margin-left: 30px">
                                                    {#each subject.teams as team}
                                                        <li>
                                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                                <b>Номер:</b> {team.name}
                                                            </p>
                                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                                <b>Преподаватели:</b>
                                                            </p>
                                                            <ul>
                                                                {#each team.teachers as teacher}
                                                                    <li style="margin-left: 30px">
                                                                        <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                                            {teacher}
                                                                        </p>
                                                                    </li>
                                                                {/each}
                                                            </ul>
                                                        </li>
                                                    {/each}
                                                </ul>
                                            {/if}
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Форма обучения:</b>
                                                {subject.formEducation === "traditional" ? "Традиционное" : subject.formEducation === "mixed" ? "Смешанное" : subject.formEducation === "online" ? "Онлайн" : "Другое"}
                                            </p>
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Группа в телеграме:</b>
                                                {subject.groupTgLink === null ? "Нет" : subject.groupTgLink}
                                            </p>
                                        </li>
                                    </ul>
                                </AccordionItem>
                            {/each}
                        </Accordion>
                    </TabItem>
                    <TabItem title="Онлайн курсы">
                        <Accordion>
                            {#each student.onlineCourse as course}
                                <AccordionItem>
                                    <span slot="header">{course.name}</span>
                                    <ul>
                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Университет:</b>
                                                {course.university}
                                            </p>
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Дата начала:</b>
                                                {course.dateStart === null ? "Нет информации" : course.dateStart}
                                            </p>
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Дата конца:</b>
                                                {course.deadline === null ? "Нет информации" : course.deadline}
                                            </p>
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Дополнительная информация:</b>
                                                {course.info === null ? "Нет информации" : course.info}
                                            </p>
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Баллы:</b>
                                            </p>
                                        </li>

                                        <ul style="margin-left: 30px">
                                            {#each Object.keys(course.scores) as keys_score}
                                                <li>
                                                    <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                        {keys_score}: {course.scores !== undefined ? course.scores[keys_score] : "Нет информации"}
                                                    </p>
                                                </li>
                                            {/each}
                                        </ul>
                                    </ul>
                                </AccordionItem>
                            {/each}
                        </Accordion>
                    </TabItem>
                </Tabs>
            </div>
        </div>
    </div>
</div>
