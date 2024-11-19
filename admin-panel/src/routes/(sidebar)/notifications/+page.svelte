<script>
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { TableHeadCell, Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
  import { onMount } from "svelte";
  import { Button } from 'flowbite-svelte';
  import { ArrowRightOutline } from 'flowbite-svelte-icons';

  let login_status = "";
  let notifications = [];

  onMount(async () => {
    let response;

    try {
      response = await fetch("http://localhost:5000/core/admin/notifications", {
        credentials: "include",
      });
    } catch (err) {
      login_status = "Not Logged In";
    }

    let json = await response?.json();
    notifications = json;
  });
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem>Рассылки</BreadcrumbItem>
		</Breadcrumb>
    <Button href="/notifications/send-notifications" class="mb-3">
      Создать рассылки
      <ArrowRightOutline class="w-6 h-6 ms-2" />
    </Button>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Ваши рассылки
      {login_status}
		</Heading>
	</div>
  <Table hoverable={true}>
    <TableHead>
      <TableHeadCell class="px-8">Содержание</TableHeadCell>
      <TableHeadCell class="px-8" defaultSort>Дата</TableHeadCell>
      <TableHeadCell class="px-8">Отправлено</TableHeadCell>
    </TableHead>
    <TableBody tableBodyClass="divide-y">
      {#each notifications as notification}
        <TableBodyRow slot="row">
          <TableBodyCell class="px-8">{notification.content}</TableBodyCell>
          <TableBodyCell class="px-8">{notification.date}</TableBodyCell>
          <TableBodyCell class="px-8">
            {#each notification.students as student}
              {student.email},
            {/each}
          </TableBodyCell>
        </TableBodyRow>
      {/each}
    </TableBody>
  </Table>
</main>