<script lang="ts" module>
    export interface IPaged {
        page: number;
        totalPages: number;
        totalCount: number;
        hasPreviousPage: boolean;
        hasNextPage: boolean;
    }

    export function createPaged(): IPaged {
        return {
            page: 1,
            totalPages: 1,
            totalCount: 0,
            hasPreviousPage: false,
            hasNextPage: false,
        };
    }
</script>

<script lang="ts">
    import { twMerge } from "tailwind-merge";
    import { PaginationItem, Select } from "flowbite-svelte";
    import {
        ChevronLeftOutline,
        ChevronRightOutline,
    } from "flowbite-svelte-icons";

    interface Props {
        paged: IPaged;
        class?: string;
    }

    let { paged = $bindable(), ...restProps }: Props = $props();

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
        if (paged.hasPreviousPage) {
            paged.page -= 1;
        }
    };

    const nextPage = () => {
        if (paged.hasNextPage) {
            paged.page += 1;
        }
    };
</script>

<div class={contentClass}>
    <Select
        placeholder=""
        class="max-w-fit pr-8"
        items={getPages(paged.totalPages)}
        bind:value={paged.page} />

    <PaginationItem
        class={paged.hasPreviousPage ? "" : disabledClass}
        large
        on:click={previousPage}>
        <ChevronLeftOutline />
    </PaginationItem>

    <PaginationItem
        class={paged.hasNextPage ? "" : disabledClass}
        large
        on:click={nextPage}>
        <ChevronRightOutline />
    </PaginationItem>

    <span class="dark:text-white text-lg">Кол-во: {paged.totalCount}</span>
</div>
