<script lang="ts">
    import ErrorAlert from "/src//lib/components/ErrorAlert.svelte";
    import { Button, Spinner, TabItem, Tabs } from "flowbite-svelte";
    import { GeneralError } from "/src/lib/errors";
    import { GroupsApi, PageSize } from "/src/lib/server";
    import Paged from "/src/lib/components/Paged.svelte";
    import Search from "/src/lib/components/Search.svelte";
    import {
        goto,
        QueryString,
        type Route,
    } from "@mateothegreat/svelte5-router";

    let { body }: { body: any } = $props();

    let page = $state(1);
    let search = $state("");

    async function getGroups() {
        let url = new URL(GroupsApi);
        url.searchParams.append("adminId", body.id);
        url.searchParams.append("search", search);
        url.searchParams.append("page", page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url);

        let groups = await res.json();

        return groups;
    }

    function changeGroups() {
        let query = new QueryString();
        query.set("adminId", body.id);

        goto(`/groups?${query.toString()}`);
    }
</script>

<Tabs
    tabStyle="underline"
    class="ml-4"
    contentClass="p-4 bg-white rounded-lg dark:bg-gray-800 m-4">
    <TabItem open title="Информация">
        <p class="dark:text-white text-xl p-4">
            <b>Почта:</b>
            {body.email}
        </p>
        <p class="dark:text-white text-xl p-4">
            <b>Дата:</b>
            {body.createdAt}
        </p>
    </TabItem>

    <TabItem title="Группы">
        <Search class="m-4" bind:page bind:search />

        {#await getGroups()}
            <Spinner size="8" />
        {:then groups}
            <Paged
                class="m-4"
                bind:page
                hasNextPage={groups.hasNextPage}
                hasPreviousPage={groups.hasPreviousPage}
                totalCount={groups.totalCount}
                totalPages={groups.totalPages} />

            <Button on:click={changeGroups} class="ml-4">Привязки</Button>

            <ul>
                {#each groups.items as group (group.id)}
                    <li class="dark:text-white text-xl p-4">{group.name}</li>
                {/each}
            </ul>
        {:catch}
            <ErrorAlert title="Ошибка">{GeneralError}</ErrorAlert>
        {/await}
    </TabItem>
</Tabs>
