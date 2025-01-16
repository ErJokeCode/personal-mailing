<script>
    import { onMount } from "svelte";
    import http from "../../utils/http";
    import { server_url } from "../../utils/store";

	import {
		Breadcrumb,
		BreadcrumbItem,
		Button,
        Search,
        Table,
        TableHead,
        TableHeadCell,
        TableBody,
        TableBodyRow,
        TableBodyCell,
        Heading,
        Helper

	} from 'flowbite-svelte';

    import { Link } from 'svelte-routing'

    let groups = $state({});
    let searchGroup = "";
    let searchAdmin = "";
    let admins = $state([]);

    let chosenGroup = $state();

    let is_active_btn = $state(false);

    onMount(async () => {
        await getGroups();
        await getAdmins();
    });

    async function getGroups() {
        let result = await fetch(
            `${server_url}/core/data/groups?search=` + searchGroup,
            {
                credentials: "include",
            },
        );

        result = await result.json();

        groups = result;
    }

    async function getAdmins() {
        let result = await fetch(
            `${server_url}/core/admin?search=` + searchAdmin,
            {
                credentials: "include",
            },
        );

        result = await result.json();

        admins = result.items;
    }

    async function handleGroupInput(event) {
        if (event.key == "Enter") {
            await getGroups();
        }
    }

    async function handleAdminInput(event) {
        if (event.key == "Enter") {
            await getAdmins();
        }
    }

    async function handleGroupChoose(group) {
        is_active_btn = true;
        if (chosenGroup === group) {
            chosenGroup = '';
        } else {
            chosenGroup = group;
        }
        console.log(chosenGroup)
    }

    async function handleAdminChoose(adminId) {
        is_active_btn = false;
        if(chosenGroup == "") {
            return;
        }

        await http.put_json(
            "/core/admin/group",
            {
                group: chosenGroup,
                adminId: adminId,
            },
            http.status(),
        );

        chosenGroup = "";
        searchGroup = "";
        searchAdmin = "";

        await getGroups();
        await getAdmins();
    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px] dark:text-white">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800 p-4 px-6">
            <Breadcrumb class="mb-5">
                <Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                    to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                    <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                    1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link>
                <BreadcrumbItem>Группы</BreadcrumbItem>
            </Breadcrumb>
            <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl flex justify-between items-end mb-5">
                Группы
            </Heading>
            <div class="flex">
                <div class="mr-6 w-full">
                    <Search
                        bind:value={searchGroup}
                        on:keypress={handleGroupInput}
                        placeholder="Поиск по группам"
                        class='block w-80 p-2.5 ps-10 text-sm mb-1'
                        size='md'
                    />
                    <Helper class='ml-1 mb-6'>Выберите группу:</Helper>
                    <Table>
                        <TableHead>
                            <TableHeadCell>Группа</TableHeadCell>
                            <TableHeadCell>Админ</TableHeadCell>
                            <TableHeadCell class='w-1/4'></TableHeadCell>
                        </TableHead>
                        <TableBody>
                            {#each Object.entries(groups).sort() as [group, admin]}
                                <TableBodyRow>
                                    <TableBodyCell class='py-2'>{group}</TableBodyCell>
                                    <TableBodyCell class='py-2'>{admin.email}</TableBodyCell>
                                    <TableBodyCell class='py-2'>
                                        <Button
                                            on:click={() =>
                                                handleGroupChoose(group)}
                                            >Изменить
                                        </Button>
                                    </TableBodyCell>
                                </TableBodyRow>
                            {/each}
                        </TableBody>
                    </Table>
                </div>
                <div class="w-full">
                    <div class="absolute" style="top: 5.25rem;">{chosenGroup}</div>
                    <Search
                        bind:value={searchAdmin}
                        on:keypress={handleAdminInput}
                        placeholder="Поиск по админам"
                        class='block w-80 p-2.5 ps-10 text-sm mb-1'
                        size='md'
                    />
                    <Helper class='ml-1 mb-6'>Выберите админа:</Helper>
                    <Table>
                        <TableHead>
                            <TableHeadCell>Админ</TableHeadCell>
                            <TableHeadCell class='w-1/2'></TableHeadCell>
                        </TableHead>
                        <TableBody>
                            {#each admins as admin}
                                <TableBodyRow>
                                    <TableBodyCell class='py-2'>{admin.email}</TableBodyCell>
                                    <TableBodyCell class='py-2'>
                                        {#if is_active_btn}
                                            <Button
                                                on:click={() =>
                                                    handleAdminChoose(admin.id)}
                                                >Выбрать
                                            </Button>
                                        {:else}
                                            <Button
                                                on:click={() =>
                                                    handleAdminChoose(admin.id)}
                                                disabled
                                                >Выбрать
                                            </Button>
                                        {/if}
                                    </TableBodyCell>
                                </TableBodyRow>
                            {/each}
                        </TableBody>
                    </Table>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
</style>
