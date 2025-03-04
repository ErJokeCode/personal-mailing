<script lang="ts">
    import Paged, {
        createPaged,
        type IPaged,
    } from "/src/lib/components/Paged.svelte";
    import { Spinner } from "flowbite-svelte";
    import { GeneralError } from "/src/lib/errors";
    import { AdminsApi } from "/src/lib/server";
    import ErrorAlert from "/src//lib/components/ErrorAlert.svelte";
    import Search from "/src/lib/components/Search.svelte";
    import type { Snippet } from "svelte";
    import PagedList from "/src/lib/components/PagedList.svelte";

    interface Props {
        search: string;
        paged: IPaged;
        pageSize: number;
        children: Snippet<[any]>;
    }

    let {
        search = $bindable(),
        paged = $bindable(),
        pageSize,
        children,
    }: Props = $props();

    async function get() {
        let url = new URL(AdminsApi);
        url.searchParams.append("search", search);
        url.searchParams.append("page", paged.page.toString());
        url.searchParams.append("pageSize", pageSize.toString());

        let res = await fetch(url, {
            credentials: "include",
        });

        let body = await res.json();

        Object.assign(paged, body);

        return body;
    }
</script>

<PagedList bind:paged bind:search {get}>
    {#snippet children(body)}
        {@render children(body)}
    {/snippet}
</PagedList>
