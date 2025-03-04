<script lang="ts">
    import { Spinner } from "flowbite-svelte";
    import type { Snippet } from "svelte";
    import ErrorAlert from "./ErrorAlert.svelte";
    import { GeneralError } from "../errors";

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
    <Spinner size="8" class="m-4 block" />
{:then body}
    {@render children(body)}
{:catch}
    <ErrorAlert class="m-4" title="Ошибка">{error}</ErrorAlert>
{/await}
