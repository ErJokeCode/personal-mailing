<script>
    import { onMount } from "svelte";
    import http from "../../utils/http";

	import {
		Breadcrumb,
		BreadcrumbItem,
		Button,
		Checkbox,
		Heading,
        Label,
        Input
	} from 'flowbite-svelte';

    let groups = {};
    let searchGroup = "";
    let searchAdmin = "";
    let admins = [];

    let chosenGroup = "";

    onMount(async () => {
        await getGroups();
        await getAdmins();
    });

    async function getGroups() {
        let result = await fetch(
            "http://localhost:5000/core/data/groups?search=" + searchGroup,
            {
                credentials: "include",
            },
        );

        result = await result.json();

        groups = result;
    }

    async function getAdmins() {
        let result = await fetch(
            "http://localhost:5000/core/admin?search=" + searchAdmin,
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
        chosenGroup = group;
    }

    async function handleAdminChoose(adminId) {
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
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div
            class="flex relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800"
        >
            <div class="p-4 px-6">
                <input
                    bind:value={searchGroup}
                    on:keypress={handleGroupInput}
                    type="text"
                    placeholder="Поиск"
                />
                <table>
                    <thead>
                        <tr>
                            <td>Группа</td>
                            <td>Админ</td>
                        </tr>
                    </thead>
                    <tbody>
                        {#each Object.entries(groups) as [group, admin]}
                            <tr>
                                <td>{group}</td>
                                <td
                                    ><Button
                                        on:click={() =>
                                            handleGroupChoose(group)}
                                        >{admin.email}</Button
                                    ></td
                                >
                            </tr>
                        {/each}
                    </tbody>
                </table>
            </div>
            <div class="">
                <div>{chosenGroup}</div>
                <input
                    bind:value={searchAdmin}
                    type="text"
                    on:keypress={handleAdminInput}
                    placeholder="Поиск"
                />
                <table class="">
                    <tbody>
                        {#each admins as admin}
                            <tr>
                                <td
                                    ><Button
                                        on:click={() =>
                                            handleAdminChoose(admin.id)}
                                        >{admin.email}</Button
                                    ></td
                                >
                            </tr>
                        {/each}
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<style>
</style>
