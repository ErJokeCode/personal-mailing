<script>
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { TableHeadCell, Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
  import { onMount } from "svelte";
  import { A } from 'flowbite-svelte';
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
			<BreadcrumbItem home href="/">Home</BreadcrumbItem>
			<BreadcrumbItem>Notifications</BreadcrumbItem>
		</Breadcrumb>
    <A href="/notifications/send-notifications" aClass="inline-flex items-center font-medium  hover:underline mb-3">
      Send Notifications
      <ArrowRightOutline class="w-6 h-6 ms-2" />
    </A>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Your Notifications
      {login_status}
		</Heading>
	</div>
  <Table hoverable={true}>
    <TableHead>
      <TableHeadCell class="px-8">Content</TableHeadCell>
      <TableHeadCell class="px-8" defaultSort>Date</TableHeadCell>
      <TableHeadCell class="px-8">Sent To</TableHeadCell>
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