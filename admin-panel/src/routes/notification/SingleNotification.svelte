<script>
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Tabs,
        TabItem,
        A,
    } from "flowbite-svelte";
    import http from "../../utils/http";
    import { onMount } from "svelte";
    import { Link } from "svelte-routing";
    import { server_url } from "../../utils/store";
    import Students from "../student/Students.svelte";

    export let id;

    let notification = {};
    let status = http.status();

    let send_status = "";
    let count = 0;

    onMount(async () => {
        notification =
            (await http.get(`/core/notifications/${id}`, status)) ?? {};
        console.log(notification);
        for (let status of notification.statuses) {
            if (status.code === 0) count++;
        }
        if (count === notification.statuses.length) {
            send_status = "успешно";
        } else if (count === 0) {
            send_status = "ошибка отправки";
        } else {
            send_status = "частично отправлено";
        }
    });
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div
            class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800"
        >
            <div class="p-4 px-6">
                <Breadcrumb class="mb-5">
                    <li class="inline-flex items-center">
                        <Link
                            class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                            to="/"
                            ><svg
                                class="w-4 h-4 me-2"
                                fill="currentColor"
                                viewBox="0 0 20 20"
                                xmlns="http://www.w3.org/2000/svg"
                            >
                                <path
                                    d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"
                                ></path></svg
                            >Главная</Link
                        >
                    </li>
                    <li class="inline-flex items-center">
                        <svg
                            class="w-6 h-6 text-gray-400 rtl:-scale-x-100"
                            fill="currentColor"
                            viewBox="0 0 20 20"
                            xmlns="http://www.w3.org/2000/svg"
                        >
                            <path
                                fill-rule="evenodd"
                                d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0
                        z"
                                clip-rule="evenodd"
                            ></path></svg
                        >
                        <Link
                            class="ml-0 ms-1 text-sm font-medium text-gray-700 hover:text-gray-900 md:ms-2 dark:text-gray-400 dark:hover:text-white"
                            to="/notifications">Рассылки</Link
                        >
                    </li>
                    <BreadcrumbItem>Детали</BreadcrumbItem>
                </Breadcrumb>
                <Heading
                    tag="h1"
                    class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3"
                >
                    {notification.date}
                </Heading>
                <div class="text-gray-900 dark:text-white">
                    <Tabs
                        tabStyle="underline"
                        contentClass="p-4 bg-white rounded-lg dark:bg-gray-800 mt-4"
                    >
                        <TabItem open title="Основная информация">
                            <ul>
                                <li>
                                    <p
                                        class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                    >
                                        <b>Статус рассылок:</b>
                                        {send_status}
                                    </p>
                                </li>

                                <li>
                                    <p
                                        class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                    >
                                        <b>Дата рассылки:</b>
                                        {notification.date}
                                    </p>
                                </li>

                                <li>
                                    <p
                                        class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                    >
                                        <b>Текст сообщения:</b>
                                        {notification.content !== ""
                                            ? notification.content
                                            : "отсутствует"}
                                    </p>
                                </li>

                                <li>
                                    <p
                                        class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                    >
                                        <b>Email админа:</b>
                                        {notification.admin === undefined
                                            ? "нет данных"
                                            : notification.admin.email}
                                    </p>
                                </li>
                            </ul>
                        </TabItem>
                        <TabItem title="Приложенные документы">
                            <ul>
                                {#each notification.documents as document}
                                    <li>
                                        <p
                                            class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                        >
                                            <b>{document.name}</b>
                                            <A
                                                class="ml-1"
                                                href={`${server_url}/core/document/${document.id}/download`}
                                                >Скачать</A
                                            >
                                        </p>
                                    </li>
                                {/each}
                                {#if notification.documents.length === 0}
                                    <p
                                        class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                    >
                                        Нет вложений
                                    </p>
                                {/if}
                            </ul>
                        </TabItem>
                        <TabItem title="Получатели">
                            <ul>
                                {#each notification.students as student}
                                    <li>
                                        <p
                                            class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                        >
                                            <b>{student.email}</b>
                                        </p>
                                    </li>
                                {/each}
                            </ul>
                        </TabItem>
                    </Tabs>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
</style>
