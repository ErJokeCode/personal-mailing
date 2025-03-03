<script lang="ts">
    import {
        Badge,
        Button,
        Checkbox,
        Heading,
        Spinner,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
    } from "flowbite-svelte";
    import Panel from "/src/lib/components/Panel.svelte";
    import { GroupsApi, PageSize } from "/src/lib/server";
    import Search from "/src/lib/components/Search.svelte";
    import Paged from "/src/lib/components/Paged.svelte";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
    import { GeneralError } from "/src/lib/errors";
    import {
        ClipboardOutline,
        FilterOutline,
        LinkOutline,
    } from "flowbite-svelte-icons";
    import {
        goto,
        QueryString,
        type Route,
    } from "@mateothegreat/svelte5-router";
    import AllAdminsInfo from "../admins/shared/AllAdminsInfo.svelte";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import BackButton from "/src/lib/components/BackButton.svelte";
    import { fade } from "svelte/transition";

    let admins = $state({
        search: "",
        page: 1,
    });

    let groups = $state({
        adminId: null,
        search: "",
        page: 1,
        trigger: false,
    });

    let { route }: { route: Route } = $props();
    groups.adminId = route.query?.["adminId"] ?? null;

    let selected: Map<string, object> = new Map();

    let notifications: ToastNotifications;

    async function getGroups(_) {
        let url = new URL(GroupsApi);
        if (groups.adminId) {
            url.searchParams.append("adminId", groups.adminId);
        }
        url.searchParams.append("search", groups.search);
        url.searchParams.append("page", groups.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url);

        let body = await res.json();

        return body;
    }

    function select(group, checked) {
        if (checked) {
            selected.set(group.id, group);
        } else {
            selected.delete(group.id);
        }
    }

    async function assign(adminId: string) {
        let status = "";

        try {
            let res = await fetch(GroupsApi, {
                method: "PATCH",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    groupIds: selected.keys().toArray(),
                    adminId: adminId,
                }),
            });

            if (!res.ok) {
                let body = await res.json();

                if (body.errors) {
                    status = body.errors["GroupIds"];
                } else {
                    status = body.detail;
                }
            } else {
                groups.trigger = !groups.trigger;

                notifications.add({
                    type: "ok",
                    text: "Группы успешно привязаны",
                });
            }
        } catch {
            status = GeneralError;
        }

        if (status != "") {
            notifications.add({
                type: "error",
                text: status,
            });
        }
    }

    function filterByAdmin(adminId) {
        groups.adminId = adminId;
        groups.page = 1;
    }

    function dismissBadge() {
        groups.adminId = null;
        groups.page = 1;
    }

    function single(adminId) {
        let query = new QueryString();
        query.set("returnUrl", window.location.pathname);

        goto(`/admins/${adminId}?${query.toString()}`);
    }
</script>

<ToastNotifications bind:this={notifications} />

<div class="m-4">
    <BackButton {route} class="inline-block align-middle" />

    <Heading tag="h2" class="inline align-middle">Группы</Heading>
</div>

<div class="flex m-4 gap-4">
    <Panel class="flex-1">
        {#if groups.adminId}
            <Badge
                on:close={dismissBadge}
                dismissable
                large
                color="dark"
                class="text-lg">{groups.adminId}</Badge>
        {/if}

        <Search
            class="m-4"
            bind:page={groups.page}
            bind:search={groups.search} />

        {#await getGroups(groups.trigger)}
            <Spinner size="8" class="m-4" />
        {:then body}
            <Paged
                class="m-4"
                bind:page={groups.page}
                hasNextPage={body.hasNextPage}
                hasPreviousPage={body.hasPreviousPage}
                totalCount={body.totalCount}
                totalPages={body.totalPages} />

            <ul>
                <Table>
                    <TableHead>
                        <TableHeadCell>Выбрать</TableHeadCell>
                        <TableHeadCell>Группа</TableHeadCell>
                        <TableHeadCell>Админ</TableHeadCell>
                    </TableHead>

                    <TableBody tableBodyClass="divide-y">
                        {#each body.items as group (group.id)}
                            <TableBodyRow class="text-lg">
                                <TableBodyCell>
                                    <Checkbox
                                        checked={selected.has(group.id)}
                                        on:change={(event) =>
                                            select(
                                                group,
                                                // @ts-ignore
                                                event.target.checked,
                                            )} />
                                </TableBodyCell>
                                <TableBodyCell>{group.name}</TableBodyCell>
                                <TableBodyCell>
                                    <Button
                                        on:click={() => single(group.admin.id)}>
                                        {group.admin.email}
                                    </Button>
                                </TableBodyCell>
                            </TableBodyRow>
                        {/each}
                    </TableBody>
                </Table>
            </ul>
        {:catch}
            <ErrorAlert title="Ошибка">{GeneralError}</ErrorAlert>
        {/await}
    </Panel>

    <Panel class="flex-1">
        <AllAdminsInfo
            bind:search={admins.search}
            bind:page={admins.page}
            pageSize={PageSize}>
            {#snippet children(body)}
                <Table>
                    <TableHead>
                        <TableHeadCell>Почта</TableHeadCell>
                        <TableHeadCell>Привязать</TableHeadCell>
                        <TableHeadCell>Детали</TableHeadCell>
                        <TableHeadCell>Фильтр</TableHeadCell>
                    </TableHead>

                    <TableBody tableBodyClass="divide-y">
                        {#each body.items as admin (admin.id)}
                            <TableBodyRow class="text-lg">
                                <TableBodyCell>{admin.email}</TableBodyCell>
                                <TableBodyCell>
                                    <Button on:click={() => assign(admin.id)}>
                                        <LinkOutline />
                                    </Button>
                                </TableBodyCell>
                                <TableBodyCell>
                                    <Button on:click={() => single(admin.id)}>
                                        <ClipboardOutline />
                                    </Button>
                                </TableBodyCell>
                                <TableBodyCell>
                                    <Button
                                        on:click={() =>
                                            filterByAdmin(admin.id)}>
                                        <FilterOutline />
                                    </Button>
                                </TableBodyCell>
                            </TableBodyRow>
                        {/each}
                    </TableBody>
                </Table>
            {/snippet}
        </AllAdminsInfo>
    </Panel>
</div>
