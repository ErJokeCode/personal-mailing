<script lang="ts">
    import { Toast, type ColorVariant } from "flowbite-svelte";
    import { CheckOutline, CloseOutline } from "flowbite-svelte-icons";
    import type { Component, Snippet } from "svelte";
    import { fly } from "svelte/transition";

    interface Item {
        type: "ok" | "error";
        text: string;
        autohide?: boolean;
        content?: Snippet<[any]>;
        params?: any;
        globalId?: number;
    }

    let items: Item[] = $state([]);
    let statuses = $state({});
    let count = 0;
    let container;

    export function add(item: Item) {
        item.globalId = count;
        statuses[item.globalId] = true;

        items = [item, ...items];
        count += 1;

        let activeCount = Object.entries(statuses).filter(
            (value) => value[1],
        ).length;

        if (activeCount == 4) {
            container.setAttribute(
                "style",
                `max-height: ${container.clientHeight}px`,
            );
        }

        if (item.autohide ?? true) {
            setTimeout(() => {
                statuses[item.globalId] = false;
            }, 5000);
        }
    }

    function close(id) {
        statuses[id] = false;
    }
</script>

<div
    bind:this={container}
    class="flex-col-reverse fixed bottom-5 end-5 flex gap-2 z-10 overflow-hidden">
    {#each items as item, index (index)}
        {#if item.type == "error"}
            {@render toast(item, "red", CloseOutline)}
        {:else}
            {@render toast(item, "green", CheckOutline)}
        {/if}
    {/each}
</div>

{#snippet toast(item: Item, color: ColorVariant, Icon: Component)}
    <Toast
        bind:toastStatus={statuses[item.globalId]}
        class="border-gray-200 dark:border-gray-700 border-1 rounded-2xl"
        align={item.content ? false : true}
        {color}
        transition={fly}
        on:close={() => close(item.globalId)}
        params={{ duration: 200, x: 200 }}>
        <Icon slot="icon" />

        <div class={"font-semibold text-md" + (item.content ? " mb-3" : "")}>
            {item.text}
        </div>

        {@render item.content?.(item.params)}
    </Toast>
{/snippet}
