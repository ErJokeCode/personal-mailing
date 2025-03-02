<script lang="ts">
    import { twMerge } from "tailwind-merge";
    import { PaginationItem, Select } from "flowbite-svelte";
    import {
        ChevronLeftOutline,
        ChevronRightOutline,
    } from "flowbite-svelte-icons";

    interface Props {
        page: number;
        totalPages: number;
        totalCount: number;
        hasPreviousPage: boolean;
        hasNextPage: boolean;
        class?: string;
    }

    let {
        page = $bindable(),
        totalPages,
        totalCount,
        hasPreviousPage,
        hasNextPage,
        ...restProps
    }: Props = $props();

    let baseClass = "flex gap-1 items-center";
    let contentClass = $derived(twMerge([baseClass, restProps.class]));
    let disabledClass =
        "hover:bg-white hover:text-gray-500 dark:hover:bg-gray-800 dark:hover:text-gray-400";

    function getPages(totalPages: number) {
        let pages = [];

        for (let i = 0; i < totalPages; i++) {
            pages.push({ name: (i + 1).toString(), value: i + 1 });
        }

        return pages;
    }

    const previousPage = () => {
        if (hasPreviousPage) {
            page -= 1;
        }
    };

    const nextPage = () => {
        if (hasNextPage) {
            page += 1;
        }
    };
</script>

<div class={contentClass}>
    <Select
        placeholder=""
        class="max-w-fit pr-8"
        items={getPages(totalPages)}
        bind:value={page} />

    <PaginationItem
        class={hasPreviousPage ? "" : disabledClass}
        large
        on:click={previousPage}>
        <ChevronLeftOutline />
    </PaginationItem>

    <PaginationItem
        class={hasNextPage ? "" : disabledClass}
        large
        on:click={nextPage}>
        <ChevronRightOutline />
    </PaginationItem>

    <span class="dark:text-white text-lg">Кол-во: {totalCount}</span>
</div>
