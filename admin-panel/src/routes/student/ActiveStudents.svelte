<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
	import { TableHeadCell } from 'flowbite-svelte';

	import { onDestroy, onMount } from "svelte";
    import { Link, navigate } from "svelte-routing";
    import http from "../../utils/http";
    import {signal} from "../../utils/signal";

    let activeStudents = [];

    let studentStatus = http.status();

    onMount(async () => {
        studentStatus = studentStatus.start_load();
        activeStudents = (await http.get("/core/student", studentStatus)).items ?? [];
        studentStatus = studentStatus.end_load();

        signal.on("NewActiveStudent", handleNewStudent);
    });

    onDestroy(async () => {
        signal.off("NewActiveStudent", handleNewStudent);
    })

    async function handleNewStudent(message) {
        activeStudents.push(message.activeStudent);
    }

    async function fullInfo(id) {
        navigate(`/students/active/${id}`);
    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="p-4 px-6">
                <Breadcrumb class="mb-5">
                    <Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link>
                    <BreadcrumbItem>Активные студенты</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
                    Активные студенты
                </Heading>
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
                    {#each activeStudents as student}
                        {#if student.info !== null}
                            {console.log(student)}
                            <TableBodyRow on:click={() => fullInfo(student.id)}>
                                <TableBodyCell class="px-8">{student.info.email}</TableBodyCell>
                                <TableBodyCell class="px-8">{student.info.group.numberCourse}</TableBodyCell>
                                <TableBodyCell class="px-8">{student.info.group.number}</TableBodyCell>
                                <TableBodyCell class="px-8">{student.info.surname}</TableBodyCell>
                                <TableBodyCell class="px-8">{student.info.name}</TableBodyCell>
                                <TableBodyCell class="px-8">{student.info.patronymic}</TableBodyCell>
                                <TableBodyCell class="px-8">{student.info.typeOfCost}</TableBodyCell>
                                <TableBodyCell class="px-8">{student.info.personalNumber}</TableBodyCell>
                            </TableBodyRow>
                        {/if}
                    {/each}
                </TableBody>
            </Table>
        </div>
    </div>
</div>
