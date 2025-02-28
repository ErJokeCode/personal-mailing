<script lang="ts">
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
    import { ClipboardOutline } from "flowbite-svelte-icons";
    import { GeneralError } from "/src/lib/errors";
    import { AdminsApi } from "/src/lib/server";

    async function get() {
        let res = await fetch(`${AdminsApi}`, {
            credentials: "include",
        });

        let body = await res.json();

        return body;
    }

    async function single(adminId: string) {
        goto(`/admins/${adminId}`);
    }
</script>

{#await get()}
    <Spinner size="8" class="m-4" />
{:then body}
    <Heading tag="h2" class="m-4">Админы</Heading>

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
    <Heading tag="h2" class="m-4">{GeneralError}</Heading>
{/await}
