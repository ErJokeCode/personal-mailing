<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
	import { TableHeadCell } from 'flowbite-svelte';

	import { onMount } from "svelte";
    import { goto } from '$app/navigation';
    import http from "../../../utils/http";

    let activeStudents = [];

    let studentStatus = http.status();

    onMount(async () => {
        studentStatus = studentStatus.start_load();
        activeStudents = (await http.get("/core/student", studentStatus)).items ?? [];
        studentStatus = studentStatus.end_load();
    });

    async function fullInfo(id) {
        goto(`/students/active/${id}`);
    }
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem>Активные студенты</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Активные студенты
		</Heading>
	</div>
	<Table>
		<TableHead>
			<TableHeadCell class="px-8">ID</TableHeadCell>
			<TableHeadCell class="px-8">Почта</TableHeadCell>
		</TableHead>
		<TableBody>
			{#each activeStudents as student}
				<TableBodyRow on:click={() => fullInfo(student.id)}>
					<TableBodyCell class="px-8">{student.id}</TableBodyCell>
					<TableBodyCell class="px-8">{student.email}</TableBodyCell>
				</TableBodyRow>
			{/each}
		</TableBody>
	</Table>
</main>
