<script>
    import {
        Breadcrumb,
        BreadcrumbItem,
        Button,
        Heading,
    } from "flowbite-svelte";
    import {
        TableHeadCell,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
    } from "flowbite-svelte";
    import { onMount } from "svelte";
    import http from "/src/lib/utils/http";
    import {
        ChevronRightOutline,
        ChevronLeftOutline,
    } from "flowbite-svelte-icons";
    import { goto } from "@mateothegreat/svelte5-router";

    let status = http.status();
    let students = [];

    let maxPage = 0;
    let curPage = 0;
    let amountPage = 10;
    let select;

    async function toPage(page) {
        if (page < 0) {
            page = 0;
        } else if (page < 1) {
            page = 1;
        } else if (page > maxPage) {
            page = maxPage;
        }

        select.value = page;
        curPage = page;
    }

    onMount(async () => {
        status = status.start_load();
        students = (await http.get("/parser/student/all", status)) ?? [];
        status = status.end_load();
        if (students.length !== 0) {
            curPage = 1;
        }
        maxPage = Math.ceil(students.length / amountPage);
    });

    async function fullInfo(email) {
        goto(`/students/${email}`);
    }
</script>

<div class="p-4 flex justify-between">
    <Heading tag="h2">Все студенты</Heading>
    <div
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl px-6 ">
        <div class="flex space-x-2">
            <Button on:click={() => toPage(curPage - 1)}
                ><ChevronLeftOutline size="lg" /></Button>
            <select
                name="select"
                aria-label="Select"
                bind:this={select}
                on:change={() => toPage(select.value)}
                class="w-auto pr-8 bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 p-3 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                {#each [...Array(maxPage + 1).keys()] as page}
                    {#if page !== 0 || students.length === 0}
                        <option value={page}>{page}</option>
                    {/if}
                {/each}
            </select>
            <Button on:click={() => toPage(curPage + 1)}
                ><ChevronRightOutline size="lg" /></Button>
        </div>
    </div>
</div>
<Table hoverable={true}>
    <TableHead>
        <TableHeadCell class="px-8">Почта</TableHeadCell>
        <TableHeadCell class="px-8">Курс</TableHeadCell>
        <TableHeadCell class="px-8">Группа</TableHeadCell>
        <TableHeadCell class="px-8">Фамилия</TableHeadCell>
        <TableHeadCell class="px-8">Имя</TableHeadCell>
        <TableHeadCell class="px-8">Отчество</TableHeadCell>
        <TableHeadCell class="px-8">Тип</TableHeadCell>
        <TableHeadCell class="px-8">Номер студенческого</TableHeadCell>
    </TableHead>
    <TableBody>
        {#each students.slice((curPage - 1) * amountPage, curPage * amountPage + amountPage - 1) as student}
            <TableBodyRow
                on:click={() => fullInfo(student.email)}
                role="link"
                class="contrast text-lg">
                <TableBodyCell class="px-8">{student.email}</TableBodyCell>
                <TableBodyCell class="px-8"
                    >{student.group.number_course}</TableBodyCell>
                <TableBodyCell class="px-8"
                    >{student.group.number}</TableBodyCell>
                <TableBodyCell class="px-8">{student.surname}</TableBodyCell>
                <TableBodyCell class="px-8">{student.name}</TableBodyCell>
                <TableBodyCell class="px-8">{student.patronymic}</TableBodyCell>
                <TableBodyCell class="px-8"
                    >{student.type_of_cost}</TableBodyCell>
                <TableBodyCell class="px-8"
                    >{student.personal_number}</TableBodyCell>
            </TableBodyRow>
        {/each}
    </TableBody>
</Table>
