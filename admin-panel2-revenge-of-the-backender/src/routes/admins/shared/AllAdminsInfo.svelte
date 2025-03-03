<script lang="ts">
    import Paged from "/src/lib/components/Paged.svelte";
    import { Spinner } from "flowbite-svelte";
    import { GeneralError } from "/src/lib/errors";
    import { AdminsApi } from "/src/lib/server";
    import ErrorAlert from "/src//lib/components/ErrorAlert.svelte";
    import Search from "/src/lib/components/Search.svelte";
    import type { Snippet } from "svelte";

    interface Props {
        search: string;
        page: number;
        pageSize: number;
        children: Snippet<[any]>;
    }

    let {
        search = $bindable(),
        page = $bindable(),
        pageSize,
        children,
    }: Props = $props();

    async function get() {
        let url = new URL(AdminsApi);
        url.searchParams.append("search", search);
        url.searchParams.append("page", page.toString());
        url.searchParams.append("pageSize", pageSize.toString());

        let res = await fetch(url, {
            credentials: "include",
        });

        let body = await res.json();

        return body;
    }
</script>

<Search bind:page class="m-4" value={search} bind:search />

{#await get()}
    <Spinner size="8" class="m-4" />
{:then body}
    <Paged
        class="m-4"
        bind:page
        totalCount={body.totalCount}
        totalPages={body.totalPages}
        hasNextPage={body.hasNextPage}
        hasPreviousPage={body.hasPreviousPage} />

    {@render children(body)}
{:catch}
    <ErrorAlert class="m-4" title="Ошибка">{GeneralError}</ErrorAlert>
{/await}
