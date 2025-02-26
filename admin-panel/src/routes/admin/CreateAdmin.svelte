<script>
    import { Label, Input, Button } from "flowbite-svelte";
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Checkbox,
    } from "flowbite-svelte";
    import { Link } from "svelte-routing";
    import http from "../../utils/http.js";
    import { onMount } from "svelte";

    let status = http.status();
    let email = "";
    let password = "";

    // let permissions = [];
    // let permission_list;

    onMount(async () => {
        // permission_list = (await http.get("/core/data/permissions", status)) ?? [];
    });

    async function create() {
        if (email.length === 0 || password.length === 0) return;
        status = status.start_load();
        await http.post_json(
            "/core/admins",
            {
                password,
                email,
                // permissions
            },
            status,
        );
        status = status.end_load();
    }

    // function add_permission(checked, permission) {
    //     if (checked) {
    //         permissions.push(permission);
    //     } else {
    //         permissions = permissions.filter(p => p != permission);
    //     }
    // }
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
                    <BreadcrumbItem>Создать</BreadcrumbItem>
                </Breadcrumb>
                <Heading
                    tag="h1"
                    class="mb-5 text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl"
                >
                    Создать администратора
                </Heading>
                <div class="mb-4">
                    <Label class="space-y-2 dark:text-white">
                        <span>Электронная почта</span>
                        <Input
                            type="text"
                            name="email"
                            placeholder="name@company.com"
                            required
                            class="border outline-none dark:border-gray-600 dark:bg-gray-700"
                            bind:value={email}
                        />
                    </Label>
                </div>
                <div class="mb-4">
                    <Label class="space-y-2 dark:text-white">
                        <span>Пароль</span>
                        <Input
                            type="password"
                            name="password"
                            placeholder="••••••••"
                            required
                            class="border outline-none dark:border-gray-600 dark:bg-gray-700"
                            bind:value={password}
                        />
                    </Label>
                </div>
                <!-- <div class="mb-5 dark:text-white">
                <Label class='space-y-2 dark:text-white'>
                    <span>Права:</span>
                </Label>
                <div>
                    {#each permission_list as permission}
                        {#if permission.claim !== 'View'}
                            <Checkbox class="ml-2 mt-2" on:click={(event) => add_permission(event.target?.checked, permission.claim)}>{permission.name}</Checkbox>
                        {:else}
                            <Checkbox class="ml-2 mt-2" checked disabled>{permission.name}</Checkbox>
                        {/if}                        
                    {/each}
                </div>
            </div> -->
                <Button class="mb-2" on:click={create}
                    >{status.value} Создать</Button
                >
            </div>
        </div>
    </div>
</div>
