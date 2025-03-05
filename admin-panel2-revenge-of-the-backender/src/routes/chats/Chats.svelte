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
    import { createPaged } from "/src/lib/components/Paged.svelte";
    import PagedList from "/src/lib/components/PagedList.svelte";
    import { ChatsApi, NotificationsApi, PageSize } from "/src/lib/server";
    import { goto, QueryString } from "@mateothegreat/svelte5-router";
    import { CirclePlusOutline, ClipboardOutline } from "flowbite-svelte-icons";

    let paged = $state(createPaged());
    let search = $state("");

    async function get() {
        let url = new URL(ChatsApi);

        url.searchParams.append("search", search);
        url.searchParams.append("page", paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(paged, body);

        return body;
    }

    function singleStudent(email) {
        goto(`/students/${email}`);
    }

    function single(id) {
        goto(`/chats/${id}`);
    }
</script>

<Heading tag="h2" class="m-4">Чаты</Heading>

<PagedList {get} bind:paged bind:search>
    {#snippet children(body)}
        <Table>
            <TableHead>
                <TableHeadCell>Студент</TableHeadCell>
                <TableHeadCell>Непрочитанных</TableHeadCell>
                <TableHeadCell>Детали</TableHeadCell>
            </TableHead>

            <TableBody tableBodyClass="divide-y">
                {#each body.items as chat}
                    <TableBodyRow class="text-lg">
                        <TableBodyCell>
                            <Button
                                on:click={() =>
                                    singleStudent(chat.student.email)}>
                                {chat.student.email}
                            </Button>
                        </TableBodyCell>
                        <TableBodyCell>
                            {chat.unreadCount}
                        </TableBodyCell>
                        <TableBodyCell>
                            <Button on:click={() => single(chat.student.id)}>
                                <ClipboardOutline />
                            </Button>
                        </TableBodyCell>
                    </TableBodyRow>
                {/each}
            </TableBody>
        </Table>
    {/snippet}
</PagedList>
