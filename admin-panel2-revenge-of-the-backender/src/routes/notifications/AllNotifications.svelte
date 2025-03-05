<script lang="ts">
    import { admin } from "./../../../../admin-panel/src/utils/store.js";
    import {
        Button,
        Heading,
        SpeedDial,
        SpeedDialButton,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
    } from "flowbite-svelte";
    import { createPaged } from "/src/lib/components/Paged.svelte";
    import PagedList from "/src/lib/components/PagedList.svelte";
    import { NotificationsApi, PageSize } from "/src/lib/server";
    import { goto, QueryString } from "@mateothegreat/svelte5-router";
    import { CirclePlusOutline, ClipboardOutline } from "flowbite-svelte-icons";

    let state = $state({
        paged: createPaged(),
        search: "",
    });

    async function get() {
        let url = new URL(NotificationsApi);

        url.searchParams.append("search", state.search);
        url.searchParams.append("page", state.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(state.paged, body);

        return body;
    }

    function singleAdmin(adminId) {
        goto(`/admins/${adminId}`);
    }

    function single(id) {
        goto(`/notifications/${id}`);
    }

    function send() {
        goto("/send-notification");
    }
</script>

<SpeedDial class="z-10">
    <SpeedDialButton name="Отправить" on:click={send}>
        <CirclePlusOutline />
    </SpeedDialButton>
</SpeedDial>

<Heading tag="h2" class="m-4">Рассылки</Heading>

<PagedList {get} bind:paged={state.paged} bind:search={state.search}>
    {#snippet children(body)}
        <Table>
            <TableHead>
                <TableHeadCell>Содержание</TableHeadCell>
                <TableHeadCell>Дата</TableHeadCell>
                <TableHeadCell>Админ</TableHeadCell>
                <TableHeadCell>Детали</TableHeadCell>
            </TableHead>

            <TableBody tableBodyClass="divide-y">
                {#each body.items as notification}
                    <TableBodyRow class="text-lg">
                        <TableBodyCell>
                            {notification.content.length > 50
                                ? notification.content.slice(0, 50)
                                : notification.content}
                        </TableBodyCell>
                        <TableBodyCell>{notification.createdAt}</TableBodyCell>
                        <TableBodyCell>
                            <Button
                                on:click={() =>
                                    singleAdmin(notification.admin.id)}>
                                {notification.admin.email}
                            </Button>
                        </TableBodyCell>
                        <TableBodyCell>
                            <Button on:click={() => single(notification.id)}>
                                <ClipboardOutline />
                            </Button>
                        </TableBodyCell>
                    </TableBodyRow>
                {/each}
            </TableBody>
        </Table>
    {/snippet}
</PagedList>
