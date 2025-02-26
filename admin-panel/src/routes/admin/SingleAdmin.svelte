<script>
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Tabs,
        TabItem,
    } from "flowbite-svelte";
    import http from "../../utils/http";
    import { onMount } from "svelte";
    import { Link } from "svelte-routing";

    export let id;

    let admin = {};
    let groups = [];
    let status = http.status();
    let student;

    onMount(async () => {
        admin = (await http.get(`/core/admins/${id}`, status)) ?? {};
        groups =
            (await http.get(`/core/groups/?adminId=${admin.id}`, status)) ?? {};
        student = [];
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
                            to="/admin">Администраторы</Link
                        >
                    </li>
                    <BreadcrumbItem>Детали</BreadcrumbItem>
                </Breadcrumb>
                <Heading
                    tag="h1"
                    class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3"
                >
                    {admin.email}
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
                                        <b>ID:</b>
                                        {admin.id}
                                    </p>
                                </li>

                                <li>
                                    <p
                                        class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                    >
                                        <b>Электронная почта:</b>
                                        {admin.email}
                                    </p>
                                </li>

                                <li>
                                    <p
                                        class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                    >
                                        <b>Дата создания:</b>
                                        {admin.createdAt}
                                    </p>
                                </li>
                            </ul>
                        </TabItem>
                        <TabItem title="Группы">
                            <ul>
                                {#each groups.sort() as group}
                                    <li>
                                        <p
                                            class="text-l text-gray-500 dark:text-gray-100 mb-2"
                                        >
                                            <b>{group.name}</b>
                                        </p>
                                    </li>
                                {/each}
                                {#if groups.length === 0}
                                    Нет прикреплённых групп
                                {/if}
                            </ul>
                        </TabItem>
                        <!-- <TabItem title="Разрешения">
                            {#each admin.permissions as permission}
                                <ul>
                                    <li>
                                        <p class="text-l text-gray-500 dark:text-gray-100 mb-2">
                                            <b>{permission.name}</b>
                                        </p>
                                    </li>
                                </ul>
                            {/each}
                        </TabItem> -->
                    </Tabs>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
</style>
