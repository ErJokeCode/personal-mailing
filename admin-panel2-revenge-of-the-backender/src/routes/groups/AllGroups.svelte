<script lang="ts">
    import {
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
    import { AdminsApi, GroupsApi, PageSize } from "/src/lib/server";
    import Search from "/src/lib/components/Search.svelte";
    import Paged from "/src/lib/components/Paged.svelte";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
    import { GeneralError } from "/src/lib/errors";
    import { ClipboardOutline, LinkOutline } from "flowbite-svelte-icons";
    import { goto, QueryString } from "@mateothegreat/svelte5-router";

    let status = $state("");
    let trigger = $state(false);

    let searchAdmins = $state("");
    let pageAdmins = $state(1);

    let searchGroups = $state("");
    let pageGroups = $state(1);

    let selectedGroups: Map<string, object> = new Map();

    async function getGroups(_) {
        let url = new URL(GroupsApi);
        url.searchParams.append("search", searchGroups);
        url.searchParams.append("page", pageGroups.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url);

        let groups = await res.json();

        return groups;
    }

    async function getAdmins() {
        let url = new URL(AdminsApi);
        url.searchParams.append("search", searchAdmins);
        url.searchParams.append("page", pageAdmins.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, {
            credentials: "include",
        });

        let body = await res.json();

        return body;
    }

    function selectGroup(group, checked) {
        if (checked) {
            selectedGroups.set(group.id, group);
        } else {
            selectedGroups.delete(group.id);
        }
    }

    async function assign(adminId: string) {
        try {
            let res = await fetch(GroupsApi, {
                method: "PATCH",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    groupIds: selectedGroups.keys().toArray(),
                    adminId: adminId,
                }),
            });

            // TODO think of how to properly display these errors

            if (!res.ok) {
                let body = await res.json();
                if (body.errors) {
                    status = body.errors["GroupIds"];
                } else {
                    status = body.detail;
                }
            } else {
                status = "";
                trigger = !trigger;
            }
        } catch {
            status = GeneralError;
        }
    }

    function single(adminId) {
        let query = new QueryString();
        query.set("returnUrl", "/groups");

        goto(`/admins/${adminId}?${query.toString()}`);
    }
</script>

<Heading tag="h2" class="m-4">Группы {status}</Heading>

<div class="flex m-4 gap-4">
    <Panel class="flex-1">
        <Search class="m-4" bind:page={pageGroups} bind:search={searchGroups} />

        {#await getGroups(trigger)}
            <Spinner size="8" />
        {:then groups}
            <Paged
                class="m-4"
                bind:page={pageGroups}
                hasNextPage={groups.hasNextPage}
                hasPreviousPage={groups.hasPreviousPage}
                totalCount={groups.totalCount}
                totalPages={groups.totalPages} />

            <ul>
                <Table>
                    <TableHead>
                        <TableHeadCell>Выбрать</TableHeadCell>
                        <TableHeadCell>Группа</TableHeadCell>
                        <TableHeadCell>Админ</TableHeadCell>
                    </TableHead>

                    <TableBody tableBodyClass="divide-y">
                        {#each groups.items as group (group.id)}
                            <TableBodyRow class="text-lg">
                                <TableBodyCell>
                                    <Checkbox
                                        checked={selectedGroups.has(group.id)}
                                        on:change={(event) =>
                                            selectGroup(
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
        <Search
            bind:page={pageAdmins}
            class="m-4"
            value={searchAdmins}
            bind:search={searchAdmins} />

        {#await getAdmins()}
            <Spinner size="8" class="m-4" />
        {:then body}
            <Paged
                class="m-4"
                bind:page={pageAdmins}
                totalPages={body.totalPages}
                totalCount={body.totalCount}
                hasNextPage={body.hasNextPage}
                hasPreviousPage={body.hasPreviousPage} />

            <Table>
                <TableHead>
                    <TableHeadCell>Почта</TableHeadCell>
                    <TableHeadCell>Привязать</TableHeadCell>
                    <TableHeadCell>Детали</TableHeadCell>
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
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
        {:catch}
            <ErrorAlert class="m-4" title="Ошибка">{GeneralError}</ErrorAlert>
        {/await}
    </Panel>
</div>
