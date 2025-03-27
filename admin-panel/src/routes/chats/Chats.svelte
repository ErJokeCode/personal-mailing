<script lang="ts">
    import { Chats } from "/src/stores/chats/Chats.svelte";
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
    import {
        ChatsApi,
        NotificationsApi,
        PageSize,
        StudentsApi,
    } from "/src/lib/server";
    import { goto, QueryString } from "@mateothegreat/svelte5-router";
    import { CirclePlusOutline, ClipboardOutline } from "flowbite-svelte-icons";
    import Get from "/src/lib/components/Get.svelte";

    async function get() {
        let url = new URL(ChatsApi);

        url.searchParams.append("search", Chats.search);
        url.searchParams.append("page", Chats.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(Chats.paged, body);

        return body;
    }

    async function getStudents() {
        let url = new URL(StudentsApi);

        url.searchParams.append("search", Chats.search);
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

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

<PagedList {get} bind:paged={Chats.paged} bind:search={Chats.search}>
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

{#if Chats.search != ""}
    <Get get={getStudents}>
        {#snippet children(body)}
            <Heading tag="h2" class="m-4">Все активные</Heading>

            <Table>
                <TableHead>
                    <TableHeadCell>Почта</TableHeadCell>
                    <TableHeadCell>Группа</TableHeadCell>
                    <TableHeadCell>Фамилия</TableHeadCell>
                    <TableHeadCell>Имя</TableHeadCell>
                    <TableHeadCell>Отчество</TableHeadCell>
                    <TableHeadCell>Начать чат</TableHeadCell>
                </TableHead>

                <TableBody tableBodyClass="divide-y">
                    {#each body.items as student}
                        <TableBodyRow class="text-lg">
                            <TableBodyCell>
                                <Button
                                    on:click={() =>
                                        singleStudent(student.email)}>
                                    {student.email}
                                </Button>
                            </TableBodyCell>
                            <TableBodyCell
                                >{student.info.group.number}</TableBodyCell>
                            <TableBodyCell>
                                {student.info.surname}
                            </TableBodyCell>
                            <TableBodyCell>
                                {student.info.name}
                            </TableBodyCell>
                            <TableBodyCell>
                                {student.info.patronymic}
                            </TableBodyCell>
                            <TableBodyCell>
                                <Button on:click={() => single(student.id)}>
                                    <ClipboardOutline />
                                </Button>
                            </TableBodyCell>
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
        {/snippet}
    </Get>
{/if}
