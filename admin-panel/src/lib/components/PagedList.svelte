<script lang="ts">
    import type { Snippet } from "svelte";
    import type { IPaged } from "./Paged.svelte";
    import Search from "./Search.svelte";
    import Paged from "./Paged.svelte";
    import { Spinner } from "flowbite-svelte";
    import ErrorAlert from "./ErrorAlert.svelte";
    import { GeneralError } from "../errors";
    import Get from "./Get.svelte";

    interface Props {
        paged: IPaged;
        search: string;
        get: () => void;
        children: Snippet<[any]>;
        before?: Snippet;
        class?: string;
    }

    let {
        paged = $bindable(),
        search = $bindable(),
        get,
        children,
        before,
        ...restProps
    }: Props = $props();
</script>

<Search bind:page={paged.page} class="m-4" value={search} bind:search />
<Paged class="m-4 mt-0" bind:paged />

{@render before?.()}

<Get {get}>
    {#snippet children(body)}
        {@render children(body)}
    {/snippet}
</Get>
