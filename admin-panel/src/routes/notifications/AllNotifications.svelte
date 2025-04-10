<script lang="ts">
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
    import PagedList from "/src/lib/components/PagedList.svelte";
    import { NotificationsApi, PageSize } from "/src/lib/server";
    import { goto } from "@mateothegreat/svelte5-router";
    import { CirclePlusOutline, ClipboardOutline } from "flowbite-svelte-icons";
    import { AllNotifications } from "/src/stores/notifications/AllNotifications.svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";

    async function get() {
        let url = new URL(NotificationsApi);

        url.searchParams.append("search", AllNotifications.search);
        url.searchParams.append("page", AllNotifications.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(AllNotifications.paged, body);

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

<Breadcrumbs
    class="m-4"
    pathItems={[
        { isHome: true },
        { name: "Рассылки" },
    ]} />

<SpeedDial class="z-10">
    <SpeedDialButton name="Отправить" on:click={send}>
        <CirclePlusOutline />
    </SpeedDialButton>
</SpeedDial>

<Heading tag="h2" class="m-4">Рассылки</Heading>

<PagedList
    {get}
    bind:paged={AllNotifications.paged}
    bind:search={AllNotifications.search}>
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
