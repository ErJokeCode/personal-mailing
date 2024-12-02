<script>
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { TableHeadCell, Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
  
    import { onMount } from "svelte";
    import http from "../../../../utility/http";
    import { goto } from '$app/navigation';

    let status = http.status();
    let students = [];

    let maxPage = 0;
    let curPage = 0;
    let amountPage = 100;
    let select;

    async function toPage(page) {
        if (page < 0) {
            page = 0;
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
        maxPage = Math.floor(students.length / amountPage);
    });

    async function fullInfo(email) {
        goto(`/students/${email}`);
    }
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem>Все студенты</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Студенты
		</Heading>
	</div>
  <Table hoverable={true}>
    <TableHead>
      <TableHeadCell class="px-8">Почта</TableHeadCell>
      <TableHeadCell class="px-8" defaultSort>Номер студенческого</TableHeadCell>
      <TableHeadCell class="px-8">Имя</TableHeadCell>
      <TableHeadCell class="px-8">Фамилия</TableHeadCell>
    </TableHead>
    <TableBody tableBodyClass="divide-y">
        {#each students as student}
        <TableBodyRow
                slot="row"
                on:click={() => fullInfo(student.email)}
                role="link"
                class="contrast">
          <TableBodyCell class="px-8">{student.email}</TableBodyCell>
          <TableBodyCell class="px-8">{student.personal_number}</TableBodyCell>
          <TableBodyCell class="px-8">{student.name}</TableBodyCell>
          <TableBodyCell class="px-8">{student.surname}</TableBodyCell>
        </TableBodyRow>
      {/each}
    </TableBody>
  </Table>
</main>
