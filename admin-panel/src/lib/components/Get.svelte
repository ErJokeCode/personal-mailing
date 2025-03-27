<script lang="ts">
    import { ListPlaceholder, Spinner } from "flowbite-svelte";
    import type { Snippet } from "svelte";
    import ErrorAlert from "./ErrorAlert.svelte";
    import { GeneralError } from "../errors";
    import { fade } from "svelte/transition";

    interface Props {
        get: () => void;
        children: Snippet<[any]>;
        error?: string;
        class?: string;
    }

    let {
        get,
        children,
        error = $bindable(GeneralError),
        ...restProps
    }: Props = $props();
</script>

{#await get()}
    <div in:fade>
        <ListPlaceholder class="max-w-full" />
        <ListPlaceholder class="max-w-full" />
    </div>
{:then body}
    {@render children(body)}
{:catch}
    <ErrorAlert class="m-4" title="Ошибка">{error}</ErrorAlert>
{/await}
