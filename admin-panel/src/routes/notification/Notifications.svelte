<script lang='ts'>
	import { Breadcrumb, BreadcrumbItem, Heading, Button } from 'flowbite-svelte';
	import { TableHeadCell, Table, TableBody, TableBodyCell, TableBodyRow, TableHead, Search } from 'flowbite-svelte';
    import { ArrowRightOutline } from 'flowbite-svelte-icons';
    import { onMount } from "svelte";
    import { Link, navigate } from "svelte-routing";
    import { server_url } from "../../utils/store";

    let notifications = [];
    let searchTerm = "";

    onMount(async () => {
        let response;

        try {
            response = await fetch(`${server_url}/core/admin/notifications`, {
                credentials: "include",
            });
        } catch (err) { }
        let json = await response?.json();
        notifications = json.items;
    });
    $: filtered = notifications.filter((item) => item.content.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1
                                            || item.date.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1
                                            || item.students.filter(student => student.email.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1).length !== 0);

    async function fullInfo(id) {
        navigate(`/notifications/${id}`);
    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="pt-4 px-6">
                <Breadcrumb class="mb-5">
                    <Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link>
                    <BreadcrumbItem>Рассылки</BreadcrumbItem>
                </Breadcrumb>
                <Button on:click={() => navigate('/notifications/send')} class="mb-3">
                    Создать рассылки
                    <ArrowRightOutline class="w-6 h-6 ms-2" />
                </Button>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                    Ваши рассылки
                </Heading>
                <Search size='md' class='block w-80 p-2.5 ps-10 text-sm mb-4' placeholder="Поиск" bind:value={searchTerm} />
            </div>
            <Table hoverable={true}>
                <TableHead>
                    <TableHeadCell class="px-8">Содержание</TableHeadCell>
                    <TableHeadCell class="px-8" defaultSort>Дата</TableHeadCell>
                    <TableHeadCell class="px-8">Отправлено</TableHeadCell>
                </TableHead>
                <TableBody tableBodyClass="divide-y">
                    {#each filtered as notification}
                        <TableBodyRow on:click={() => fullInfo(notification.id)}>
                            <TableBodyCell class="px-8 break-all">{notification.content.length > 50 ? notification.content.slice(0, 50) : notification.content}</TableBodyCell>
                            <TableBodyCell class="px-8">{notification.date}</TableBodyCell>
                            <TableBodyCell class="px-8 overflow-hidden text-ellipsis" style='max-width: 50dvw'>
                                {#each notification.students as student}
                                    {student.email},&nbsp;
                                {/each}
                            </TableBodyCell>
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
        </div>
    </div>
</div>

<style>
</style>