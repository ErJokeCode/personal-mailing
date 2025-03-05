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
    import { NotificationsApi, PageSize, StudentsApi } from "/src/lib/server";
    import { goto, QueryString } from "@mateothegreat/svelte5-router";
    import { CirclePlusOutline, ClipboardOutline } from "flowbite-svelte-icons";

    let state = $state({
        paged: createPaged(),
        search: "",
    });

    async function get() {
        let url = new URL(StudentsApi);

        url.searchParams.append("search", state.search);
        url.searchParams.append("page", state.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(state.paged, body);

        return body;
    }

    function single(email) {
        goto(`/students/${email}`);
    }
</script>

<Heading tag="h2" class="m-4">Активные студенты</Heading>

<PagedList {get} bind:paged={state.paged} bind:search={state.search}>
    {#snippet children(body)}
        <Table>
            <TableHead>
                <TableHeadCell>Почта</TableHeadCell>
                <TableHeadCell>Группа</TableHeadCell>
                <TableHeadCell>Курс</TableHeadCell>
                <TableHeadCell>Фамилия</TableHeadCell>
                <TableHeadCell>Имя</TableHeadCell>
                <TableHeadCell>Отчество</TableHeadCell>
                <TableHeadCell>Детали</TableHeadCell>
            </TableHead>

            <TableBody tableBodyClass="divide-y">
                {#each body.items as student}
                    <TableBodyRow class="text-lg">
                        <TableBodyCell>
                            {student.info.email}
                        </TableBodyCell>
                        <TableBodyCell
                            >{student.info.group.number}</TableBodyCell>
                        <TableBodyCell>
                            {student.info.group.numberCourse}
                        </TableBodyCell>
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
                            <Button on:click={() => single(student.email)}>
                                <ClipboardOutline />
                            </Button>
                        </TableBodyCell>
                    </TableBodyRow>
                {/each}
            </TableBody>
        </Table>
    {/snippet}
</PagedList>
