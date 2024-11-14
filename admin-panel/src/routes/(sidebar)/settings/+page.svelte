<script lang="ts">
	import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Table,
        TableHead,
        TableHeadCell,
        TableBody,
        TableBodyRow,
        TableBodyCell
    } from 'flowbite-svelte';
    import { admin } from "../../../utility/store";
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem>Профиль</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
			Профиль
		</Heading>
        <Table>
            <TableHead>
                <TableHeadCell>{$admin.email}</TableHeadCell>
            </TableHead>
            <TableBody>
                <TableBodyRow>
                    <TableBodyCell>{$admin.date}</TableBodyCell>
                </TableBodyRow>
            </TableBody>
        </Table>
	</div>
</main>