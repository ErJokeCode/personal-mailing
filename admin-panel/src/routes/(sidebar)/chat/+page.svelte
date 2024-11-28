<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Button, Heading, Table, TableBody, TableBodyCell, TableBodyRow, TableHead, TableHeadCell } from 'flowbite-svelte';
	import { ArrowRightOutline } from 'flowbite-svelte-icons';
    import { onMount } from "svelte";
    import { goto } from '$app/navigation';
    import http from "../../../utility/http";

    let chats = [];
    let status = http.status();

    onMount(async () => {
        status = status.start_load();
        chats = (await http.get("/core/admin/chats", status)) ?? [];
        status = status.end_load();
    });

    async function open_chat(id, studentId) {
        goto(`/chat/${studentId}`);
    }
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home>Главная</BreadcrumbItem>
			<BreadcrumbItem>Все чаты</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Чаты
        </Heading>
	</div>
	<Table>
		<TableHead>
			{#each ['Электронная почта', 'Чат'] as title}
				<TableHeadCell class="ps-8">{title}</TableHeadCell>
			{/each}
		</TableHead>
		<TableBody>
            {#each chats as chat}
                <TableBodyRow class="text-base">
                    <TableBodyCell class="px-8">{chat.student.email}</TableBodyCell>
                    <TableBodyCell>
                        <Button size="sm" class="gap-2 px-3"
                            on:click={() => open_chat(chat.id, chat.student.id)}>
                            Открыть
                            <ArrowRightOutline size="sm" />
                        </Button>
                    </TableBodyCell>
                </TableBodyRow>
            {/each}
		</TableBody>
	</Table>
</main>