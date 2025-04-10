<script lang="ts">
    import { Chats } from "/src/stores/chats/Chats.svelte";
    import {
      Avatar,
        Button,
        Heading,
        Indicator,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
    } from "flowbite-svelte";
    import PagedList from "/src/lib/components/PagedList.svelte";
    import {
        ChatsApi,
        PageSize,
        StudentsApi,
    } from "/src/lib/server";
    import { goto } from "@mateothegreat/svelte5-router";
    import { ClipboardOutline } from "flowbite-svelte-icons";
    import Get from "/src/lib/components/Get.svelte";
    import { signal } from "/src/lib/utils/signal";
    import { onDestroy } from "svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
   
    signal.on("MessageReceived", get);

    let chats = $state([]);
    
    onDestroy(() => {
        signal.off("MessageReceived", get);
    });

    async function get() {
        let url = new URL(ChatsApi);

        url.searchParams.append("search", Chats.search);
        url.searchParams.append("page", Chats.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();
        chats = body.items;

        Object.assign(Chats.paged, body);

        return body;
    }

    async function getStudents() {
        let url = new URL(StudentsApi);

        url.searchParams.append("search", Chats.search);
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        return body;
    }

    function singleStudent(email) {
        goto(`/students/${email}`);
    }

    function single(id) {
        goto(`/chats/${id}`);
    }
</script>

<Breadcrumbs
    class="m-4"
    pathItems={[
        { isHome: true },
        { name: "Чаты" },
    ]} />

<Heading tag="h2" class="m-4">Чаты</Heading>

<PagedList {get} bind:paged={Chats.paged} bind:search={Chats.search}>
    {#snippet children()}
        {#if chats.length !== 0}
            <div class="bg-white dark:bg-gray-800 text-gray-500 dark:text-gray-400
                        rounded-lg border border-gray-200 dark:border-gray-700 divide-y
                        divide-gray-200 dark:divide-gray-600 relative m-4 shadow-md">
                {#each chats as chat}
                    <div class="border-b last:border-b-0 bg-white dark:bg-gray-800
                                border-gray-200 dark:border-gray-700 hover:bg-gray-50
                                dark:hover:bg-gray-600 contrast text-lg first:rounded-t-lg
                                last:rounded-b-lg flex gap-4 px-6 py-4 text-nowrap w-full">
                        <div class="cursor-pointer" on:click={() => singleStudent(chat.student.email)}>
                            <Avatar />
                        </div>
                    
                        <div class="hidden md:block text-gray-900 dark:text-white">
                            <p class="min-w-60 mt-1">{chat.student.email}</p>
                        </div>
                    
                        <div class="cursor-pointer w-full overflow-hidden" on:click={() => single(chat.student.id)}>
                            <div class="relative text-sm font-normal text-gray-500 dark:text-gray-400">
                                <div class="text-base font-semibold text-gray-900 dark:text-white flex justify-between">
                                    {#if chat.messages[0].admin}
                                        Вы
                                    {:else}
                                        <div class="hidden sm:block">{chat.student.info.surname} {chat.student.info.name} {chat.student.info.patronymic}</div>
                                        <div class="block sm:hidden min-w-30">
                                            {chat.student.info.surname} {chat.student.info.name[0]}. {chat.student.info.patronymic[0]}.
                                        </div>
                                    {/if}
                                    <div class="font-normal text-gray-500 dark:text-gray-400">
                                        {new Date(chat.messages[0].createdAt).toLocaleTimeString("ru")}
                                    </div>
                                </div>
                    
                                <div class="text-sm font-normal text-gray-500 dark:text-gray-400 flex items-center gap-2">
                                    <div class="truncate w-full">
                                        {#if chat.messages[0].documents.length !== 0}
                                            (Файл)
                                        {/if}
                                        {chat.messages[0].content}
                                    </div>
                                    
                                    {#if chat.unreadCount !== 0}
                                        <div class="relative">
                                            <Indicator color="dark" size="xl" class="text-white">
                                                {chat.unreadCount}
                                            </Indicator>
                                        </div>
                                    {/if}
                                </div>
                            </div>
                        </div>
                    </div>
                {/each}
            </div>
        {/if}
    {/snippet}
</PagedList>

{#if Chats.search != ""}
    <Get get={getStudents}>
        {#snippet children(body)}
            <Heading tag="h2" class="m-4">Все активные</Heading>

            <Table>
                <TableHead>
                    <TableHeadCell>Почта</TableHeadCell>
                    <TableHeadCell>Группа</TableHeadCell>
                    <TableHeadCell>Фамилия</TableHeadCell>
                    <TableHeadCell>Имя</TableHeadCell>
                    <TableHeadCell>Отчество</TableHeadCell>
                    <TableHeadCell>Начать чат</TableHeadCell>
                </TableHead>

                <TableBody tableBodyClass="divide-y">
                    {#each body.items as student}
                        <TableBodyRow class="text-lg">
                            <TableBodyCell class="w-1/5">
                                <Button
                                    on:click={() =>
                                        singleStudent(student.email)}>
                                    {student.email}
                                </Button>
                            </TableBodyCell>
                            <TableBodyCell
                                >{student.info.group.number}</TableBodyCell>
                            <TableBodyCell>
                                {student.info.surname}
                            </TableBodyCell>
                            <TableBodyCell>
                                {student.info.name}
                            </TableBodyCell>
                            <TableBodyCell>
                                {student.info.patronymic}
                            </TableBodyCell>
                            <TableBodyCell>
                                <Button on:click={() => single(student.id)}>
                                    <ClipboardOutline />
                                </Button>
                            </TableBodyCell>
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
        {/snippet}
    </Get>
{/if}
