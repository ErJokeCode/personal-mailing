<script lang="ts">
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Tabs,
        TabItem,
        Accordion,
        AccordionItem,
    } from "flowbite-svelte";
    import { onMount } from "svelte";
    import http from "/src/lib/utils/http";
    import type { Route } from "@mateothegreat/svelte5-router";
    import BackButton from "/src/lib/components/BackButton.svelte";

    let { route }: { route: Route } = $props();
    let email = route.params?.["email"] ?? "";

    let student: any = $state({});
    let status = http.status();

    onMount(async () => {
        student =
            (await http.get("/parser/student/?email=" + email, status)) ?? {};
    });
</script>

<BackButton
    title={`${student.surname} ${student.name} ${student.patronymic}`}
    class="m-4 mb-0" />

<div class="p-4 px-6">
    <Tabs
        tabStyle="underline"
        contentClass="p-4 bg-white rounded-lg dark:bg-gray-800 mt-4">
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
                        {student.group !== undefined
                            ? student.group.number
                            : "Не известен"}
                    </p>
                </li>

                <li>
                    <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                        <b>Номер курса:</b>
                        {student.group !== undefined
                            ? student.group.number_course
                            : "Не известен"}
                    </p>
                </li>

                <li>
                    <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                        <b>Направление:</b>
                        {student.group !== undefined
                            ? student.group.direction_code +
                              " " +
                              student.group.name_speciality
                            : "Не известно"}
                    </p>
                </li>

                <li>
                    <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                        <b>Статус:</b>
                        {student.status
                            ? "Активный студент"
                            : "Не активный студент"}
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
            <Accordion class='mb-5' multiple classActive='dark:bg-gray-600 dark:focus:ring-gray-700'>
                {#each student.subjects as subject}
                    <AccordionItem borderOpenClass='border-s border-e border-b border-gray-200 dark:border-gray-700'>
                        <span slot="header">{subject.name}</span>
                        <ul>
                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Полное название:</b>
                                    {subject.full_name}
                                </p>
                            </li>

                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Команды:</b>
                                </p>
                                {#if subject.teams !== undefined}
                                    <ul style="margin-left: 30px">
                                        {#each subject.teams as team}
                                            <li>
                                                <p
                                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                    <b>Номер:</b>
                                                    {team.name}
                                                </p>
                                                <p
                                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                                    <b>Преподаватели:</b>
                                                </p>
                                                <ul>
                                                    {#each team.teachers as teacher}
                                                        <li
                                                            style="margin-left: 30px">
                                                            <p
                                                                class="text-l text-gray-500 dark:text-gray-100 mb-2">
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
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Форма обучения:</b>
                                    {subject.form_education === "traditional"
                                        ? "Традиционное"
                                        : subject.form_education === "mixed"
                                          ? "Смешанное"
                                          : subject.form_education === "online"
                                            ? "Онлайн"
                                            : "Другое"}
                                </p>
                            </li>

                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Группа в телеграме:</b>
                                    {subject.group_tg_link === null
                                        ? "Нет"
                                        : subject.group_tg_link}
                                </p>
                            </li>
                        </ul>
                    </AccordionItem>
                {/each}
            </Accordion>
        </TabItem>
        <TabItem title="Онлайн курсы">
            <Accordion class='mb-5' multiple classActive='dark:bg-gray-600 dark:focus:ring-gray-700'>
                {#each student.online_course as course}
                    <AccordionItem borderOpenClass='border-s border-e border-b border-gray-200 dark:border-gray-700'>
                        <span slot="header">{course.name}</span>
                        <ul>
                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Университет:</b>
                                    {course.university}
                                </p>
                            </li>

                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Дата начала:</b>
                                    {course.date_start === null
                                        ? "Нет информации"
                                        : course.date_start}
                                </p>
                            </li>

                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Дата конца:</b>
                                    {course.deadline === null
                                        ? "Нет информации"
                                        : course.deadline}
                                </p>
                            </li>

                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Дополнительная информация:</b>
                                    {course.info === null
                                        ? "Нет информации"
                                        : course.info}
                                </p>
                            </li>

                            <li>
                                <p
                                    class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                    <b>Баллы:</b>
                                </p>
                            </li>

                            <ul style="margin-left: 30px">
                                {#each Object.keys(course.scores) as keys_score}
                                    <li>
                                        <p
                                            class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                            {keys_score}: {course.scores !==
                                            undefined
                                                ? course.scores[keys_score]
                                                : "Нет информации"}
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
