<script>
	import { Breadcrumb, BreadcrumbItem, Button, Heading } from 'flowbite-svelte';
	import { TableHeadCell, Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
  import { onMount } from "svelte";
  import { ArrowRightOutline } from 'flowbite-svelte-icons';

  let login_status = "";
  let admins = [];

  onMount(async () => {
    let response;

    try {
      response = await fetch('http://localhost:5000/core/admin', {
          credentials: "include",
      });
    } catch (err) {
      login_status = "Not Logged In";
    }
    let json = await response?.json();
    admins = json.items;
  });
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem>Администраторы</BreadcrumbItem>
		</Breadcrumb>
    <Button href="/admin/create-admin" class="mb-3">
      Создать
      <ArrowRightOutline class="w-6 h-6 ms-2" />
    </Button>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Администраторы
      {login_status}
		</Heading>
	</div>
  <Table hoverable={true}>
    <TableHead>
      <TableHeadCell class="px-8">Электронная почта</TableHeadCell>
    </TableHead>
    <TableBody tableBodyClass="divide-y">
			{#each admins as admin}
				<TableBodyRow slot="row">
					<TableBodyCell class="px-8">{admin.email}</TableBodyCell>
				</TableBodyRow>
			{/each}
    </TableBody>
  </Table>
</main>
