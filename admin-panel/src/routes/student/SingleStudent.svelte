<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading, Tabs, TabItem, Accordion, AccordionItem } from 'flowbite-svelte';
	import { onMount } from "svelte";
    import http from "../../utils/http";
    import { Link } from 'svelte-routing';

    export let email;

    let student = {};
    let status = http.status();

    onMount(async () => {
        student =
            (await http.get("/parser/student/?email=" + email, status)) ?? {};
    });
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
                    <Link class="ml-0 ms-1 text-sm font-medium text-gray-700 hover:text-gray-900 md:ms-2 dark:text-gray-400 dark:hover:text-white" to="/students/all">Все студенты</Link></li>
                    <BreadcrumbItem>Детали</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3">
                    {student.name} {student.surname} {student.patronymic}
                </Heading>
                <Tabs tabStyle='underline' contentClass='p-4 bg-white rounded-lg dark:bg-gray-800 mt-4'>
                    <TabItem open title="Основная информация">
                        <ul>
                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Номер студенческого билета:</b>
                                    {student.personal_number}
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
                                    {student.date_of_birth}
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
                                    {(student.group !== undefined) ? student.group.number_course : 'Не известен'}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Направление:</b>
                                    {(student.group !== undefined) ? student.group.direction_code + " " + student.group.name_speciality : 'Не известно'}
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
                                    {student.type_of_cost}
                                </p>
                            </li>

                            <li>
                                <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Форма обучения:</b>
                                    {student.type_of_education}
                                </p>
                            </li>
                        </ul>
                    </TabItem>
                    <TabItem title="Предметы">
                        {#each student.subjects as subject}
                            <Accordion class='mb-5'>
                                <AccordionItem>
                                    <span slot="header">{subject.name}</span>
                                    <ul>
                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Полное название:</b>
                                                {subject.full_name}
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
                                                {subject.form_education === "traditional" ? "Традиционное" : subject.form_education === "mixed" ? "Смешанное" : subject.form_education === "online" ? "Онлайн" : "Другое"}
                                            </p>
                                        </li>

                                        <li>
                                            <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                <b>Группа в телеграме:</b>
                                                {subject.group_tg_link === null ? "Нет" : subject.group_tg_link}
                                            </p>
                                        </li>
                                    </ul>
                                </AccordionItem>
                            </Accordion>
                        {/each}
                    </TabItem>
                    <TabItem title="Онлайн курсы">
                        {#each student.online_course as course}
                            <Accordion class='mb-5'>
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
                                                {course.date_start === null ? "Нет информации" : course.date_start}
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
                            </Accordion>
                        {/each}
                    </TabItem>
                </Tabs>
            </div>
        </div>
    </div>
</div>