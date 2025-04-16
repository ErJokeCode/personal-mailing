<script lang="ts">
    import {
        Button,
        Heading,
        SpeedDial,
        SpeedDialButton,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
    } from "flowbite-svelte";
    import { goto } from "@mateothegreat/svelte5-router";
    import { CirclePlusOutline, ClipboardOutline } from "flowbite-svelte-icons";
    import { PageSize } from "/src/lib/server";
    import { AllAdmins } from "/src/stores/admins/AllAdmins.svelte";
    import AllAdminsInfo from "./shared/AllAdminsInfo.svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";

    async function single(adminId: string) {
        goto(`/admins/${adminId}`);
    }

    async function create() {
        goto("/create-admin");
    }
</script>

<Breadcrumbs
    pathItems={[
        { isHome: true },
        { name: "Администраторы" },
    ]} />

<SpeedDial class="z-10">
    <SpeedDialButton name="Создать" on:click={create}>
        <CirclePlusOutline />
    </SpeedDialButton>
</SpeedDial>

<Heading tag="h2" class="m-4">Админы</Heading>

<AllAdminsInfo
    bind:paged={AllAdmins.paged}
    bind:search={AllAdmins.search}
    pageSize={PageSize}>
    {#snippet children(body)}
        <Table>
            <TableHead>
                <TableHeadCell>Почта</TableHeadCell>
                <TableHeadCell>Дата</TableHeadCell>
                <TableHeadCell>Детали</TableHeadCell>
            </TableHead>
            <TableBody tableBodyClass="divide-y">
                {#each body.items as admin (admin.id)}
                    <TableBodyRow class="text-lg">
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
    {/snippet}
</AllAdminsInfo>
