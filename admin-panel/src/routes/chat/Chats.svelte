<script>
	import { Breadcrumb, BreadcrumbItem, Button, Heading, Table, TableBody, TableBodyCell, TableBodyRow, TableHead, TableHeadCell } from 'flowbite-svelte';
	import { ArrowRightOutline } from 'flowbite-svelte-icons';
    import { onMount, onDestroy } from "svelte";
    import { Link, navigate } from "svelte-routing";
    import http from "../../utils/http";
	import { signal } from '../../utils/signal';

    let chats = [];
    let status = http.status();

    onMount(async () => {
        await getChats();

		signal.on('StudentSentMessage', handleMessage);
    });

	onDestroy(async () => {
		signal.off('StudentSentMessage', handleMessage);
	});

    async function getChats() {
        status = status.start_load();
        chats = (await http.get("/core/admin/chats", status)).items ?? [];
        status = status.end_load();
    }

    async function handleMessage(message) {
        await getChats();
    }
    async function open_chat(id, studentId) {
        navigate(`/chat/${studentId}`);
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
                    <BreadcrumbItem>Все чаты</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
                    Чаты
                </Heading>
            </div>
            <Table>
                <TableHead>
                    {#each ['Электронная почта', 'Чат', 'Непрочитанных'] as title}
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
                            <TableBodyCell class="px-8">{chat.unreadCount}</TableBodyCell>
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
        </div>
    </div>
</div>

<style></style>
