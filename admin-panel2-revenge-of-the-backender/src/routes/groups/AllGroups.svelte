<script lang="ts">
    import { AllGroups, AllAdmins } from "/src/stores/groups/AllGroups.svelte";
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
    import { GeneralError } from "/src/lib/errors";
    import {
        ClipboardOutline,
        FilterOutline,
        LinkOutline,
    } from "flowbite-svelte-icons";
    import { goto, type Route } from "@mateothegreat/svelte5-router";
    import AllAdminsInfo from "../admins/shared/AllAdminsInfo.svelte";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import BackButton from "/src/lib/components/BackButton.svelte";
    import PagedList from "/src/lib/components/PagedList.svelte";

    let { route }: { route: Route } = $props();
    AllGroups.adminId = route.query?.["adminId"] ?? AllGroups.adminId;

    let notifications: ToastNotifications;

    $effect(() => {
        let _ = AllGroups.search;
        AllGroups.selectAll = false;
    });

    async function getGroups(_) {
        let url = new URL(GroupsApi);

        if (AllGroups.adminId) {
            url.searchParams.append("adminId", AllGroups.adminId);
        }
        url.searchParams.append("search", AllGroups.search);
        url.searchParams.append("page", AllGroups.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(AllGroups.paged, body);

        return body;
    }

    function select(group, checked) {
        if (checked) {
            AllGroups.selected.set(group.id, group);
        } else {
            AllGroups.selected.delete(group.id);
        }
    }

    async function selectAll(checked: boolean) {
        try {
            let url = new URL(GroupsApi);

            if (AllGroups.adminId) {
                url.searchParams.append("adminId", AllGroups.adminId);
            }
            url.searchParams.append("search", AllGroups.search);
            url.searchParams.append("page", AllGroups.paged.page.toString());
            url.searchParams.append("pageSize", "-1");

            let res = await fetch(url, { credentials: "include" });
            let body = await res.json();

            if (checked) {
                for (let item of body.items) {
                    AllGroups.selected.set(item.id, item);
                }
            } else {
                for (let item of body.items) {
                    AllGroups.selected.delete(item.id);
                }
            }

            AllGroups.trigger = !AllGroups.trigger;
        } catch {
            notifications.add({
                type: "error",
                text: "Ошибка при выборе всех групп",
            });
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
                    groupIds: AllGroups.selected.keys().toArray(),
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
                AllGroups.trigger = !AllGroups.trigger;

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
        AllGroups.adminId = adminId;
        AllGroups.paged.page = 1;
    }

    function dismissBadge() {
        AllGroups.adminId = null;
        AllGroups.paged.page = 1;
    }

    function single(adminId) {
        goto(`/admins/${adminId}`);
    }

    function clearSelection() {
        AllGroups.selectAll = false;
        AllGroups.selected.clear();
        AllGroups.trigger = !AllGroups.trigger;
    }
</script>

<ToastNotifications bind:this={notifications} />

<div class="m-4">
    <BackButton class="inline-block align-middle" />

    <Heading tag="h2" class="inline align-middle">Группы</Heading>
</div>

<div class="flex m-4 gap-4">
    <Panel class="flex-1">
        {#if AllGroups.adminId}
            <Badge
                on:close={dismissBadge}
                dismissable
                large
                color="dark"
                class="text-lg">{AllGroups.adminId}</Badge>
        {/if}

        <PagedList
            get={() => getGroups(AllGroups.trigger)}
            bind:paged={AllGroups.paged}
            bind:search={AllGroups.search}>
            {#snippet before()}
                <div class="ml-4 mb-4 flex gap-2">
                    <Button on:click={clearSelection}>Очистить</Button>
                </div>
            {/snippet}

            {#snippet children(body)}
                <Table>
                    <TableHead>
                        <TableHeadCell>
                            <Checkbox
                                bind:checked={AllGroups.selectAll}
                                on:change={(event) =>
                                    // @ts-ignore
                                    selectAll(event.target.checked)} />
                        </TableHeadCell>
                        <TableHeadCell>Группа</TableHeadCell>
                        <TableHeadCell>Админ</TableHeadCell>
                    </TableHead>
                    <TableBody tableBodyClass="divide-y">
                        {#each body.items as group (group.id)}
                            <TableBodyRow class="text-lg">
                                <TableBodyCell>
                                    <Checkbox
                                        checked={AllGroups.selected.has(
                                            group.id,
                                        )}
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
            {/snippet}
        </PagedList>
    </Panel>

    <Panel class="flex-1">
        <AllAdminsInfo
            bind:search={AllAdmins.search}
            bind:paged={AllAdmins.paged}
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
