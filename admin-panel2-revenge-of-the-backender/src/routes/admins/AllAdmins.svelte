<script lang="ts">
    import Paged from "/src/lib/components/Paged.svelte";
    import {
        Button,
        Heading,
        Spinner,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
    } from "flowbite-svelte";
    import { goto } from "@mateothegreat/svelte5-router";
    import { ClipboardOutline, SearchOutline } from "flowbite-svelte-icons";
    import { GeneralError } from "/src/lib/errors";
    import { AdminsApi, PageSize } from "/src/lib/server";
    import ErrorAlert from "/src//lib/components/ErrorAlert.svelte";
    import { AllAdmins } from "/src/stores/admins/AllAdmins.svelte";
    import Search from "/src/lib/components/Search.svelte";

    async function get() {
        let url = new URL(AdminsApi);
        url.searchParams.append("search", AllAdmins.search);
        url.searchParams.append("page", AllAdmins.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, {
            credentials: "include",
        });

        let body = await res.json();

        return body;
    }

    async function single(adminId: string) {
        goto(`/admins/${adminId}`);
    }
</script>

<Heading tag="h2" class="m-4">Админы</Heading>

<Search
    bind:page={AllAdmins.page}
    class="m-4"
    value={AllAdmins.search}
    bind:search={AllAdmins.search} />

{#await get()}
    <Spinner size="8" class="m-4" />
{:then body}
    <Paged
        class="m-4"
        bind:page={AllAdmins.page}
        totalPages={body.totalPages}
        hasNextPage={body.hasNextPage}
        hasPreviousPage={body.hasPreviousPage} />

    <Table>
        <TableHead>
            <TableHeadCell>Почта</TableHeadCell>
            <TableHeadCell>Дата</TableHeadCell>
            <TableHeadCell>Детали</TableHeadCell>
        </TableHead>
        <TableBody tableBodyClass="divide-y">
            {#each body.items as admin (admin.id)}
                <TableBodyRow>
                    <TableBodyCell>{admin.email}</TableBodyCell>
                    <TableBodyCell>{admin.createdAt}</TableBodyCell>
                    <TableBodyCell>
                        <Button on:click={() => single(admin.id)}>
                            <ClipboardOutline />
                        </Button>
                    </TableBodyCell>
                </TableBodyRow>
            {/each}
        </TableBody>
    </Table>
{:catch}
    <ErrorAlert class="m-4" title="Ошибка">{GeneralError}</ErrorAlert>
{/await}
