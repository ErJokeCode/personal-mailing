<script lang="ts">
    import type { Snippet } from "svelte";
    import { twMerge } from "tailwind-merge";

    let props = $props();

    let { children }: { children: Snippet } = props;

    let baseClass =
        "bg-white dark:bg-gray-800 text-gray-500 dark:text-gray-400 rounded-lg border border-gray-200 dark:border-gray-700 divide-gray-200 dark:divide-gray-700 shadow-md flex flex-col p-4 sm:p-6";

    let contentClass = $derived(twMerge([baseClass, props.class]));
</script>

<div class={contentClass}>
    {@render children()}
</div>
