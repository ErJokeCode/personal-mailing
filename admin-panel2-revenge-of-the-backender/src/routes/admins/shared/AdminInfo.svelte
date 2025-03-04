<script lang="ts">
    import { Button, TabItem, Tabs } from "flowbite-svelte";
    import { GroupsApi, PageSize } from "/src/lib/server";
    import Paged, { createPaged } from "/src/lib/components/Paged.svelte";
    import { goto, QueryString } from "@mateothegreat/svelte5-router";
    import PagedList from "/src/lib/components/PagedList.svelte";

    let { body }: { body: any } = $props();

    let paged = $state(createPaged());
    let search = $state("");

    async function getGroups() {
        let url = new URL(GroupsApi);
        url.searchParams.append("adminId", body.id);
        url.searchParams.append("search", search);
        url.searchParams.append("page", paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url);

        let groups = await res.json();

        Object.assign(paged, groups);

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
        <PagedList bind:paged bind:search get={getGroups}>
            {#snippet before()}
                <Button on:click={changeGroups} class="ml-4">Привязки</Button>
            {/snippet}

            {#snippet children(groups)}
                <ul>
                    {#each groups.items as group (group.id)}
                        <li class="dark:text-white text-xl p-4">
                            {group.name}
                        </li>
                    {/each}
                </ul>
            {/snippet}
        </PagedList>
    </TabItem>
</Tabs>
