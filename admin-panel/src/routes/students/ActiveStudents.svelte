<script lang="ts">
    import {
        Button,
        Heading,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
    } from "flowbite-svelte";
    import PagedList from "/src/lib/components/PagedList.svelte";
    import { PageSize, StudentsApi } from "/src/lib/server";
    import { goto } from "@mateothegreat/svelte5-router";
    import { ClipboardOutline } from "flowbite-svelte-icons";
    import { signal } from "/src/lib/utils/signal";
    import { onDestroy } from "svelte";
    import { ActiveStudents } from "/src/stores/students/ActiveStudents.svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";

    signal.on("StudentAuthed", handleStudentAuthed);

    onDestroy(() => {
        signal.on("StudentAuthed", handleStudentAuthed);
    });

    function handleStudentAuthed(message) {
        ActiveStudents.trigger = !ActiveStudents.trigger;
    }

    async function get(_) {
        let url = new URL(StudentsApi);

        url.searchParams.append("search", ActiveStudents.search);
        url.searchParams.append("page", ActiveStudents.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(ActiveStudents.paged, body);

        return body;
    }

    function single(email) {
        goto(`/students/${email}`);
    }
</script>

<Breadcrumbs pathItems={[{ isHome: true }, { name: "Активные студенты" }]} />

<Heading tag="h2" class="mx-4 mb-4">Активные студенты</Heading>

<PagedList
    get={() => get(ActiveStudents.trigger)}
    bind:paged={ActiveStudents.paged}
    bind:search={ActiveStudents.search}>
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
