<script lang="ts">
    import { route } from "@mateothegreat/svelte5-router";
    import { Breadcrumb, BreadcrumbItem } from "flowbite-svelte";

    interface PathItem {
        isHome?: boolean;
        name?: string;
        href?: string;
    }

    interface Props {
        pathItems: PathItem[];
        class?: string;
    }

    let { pathItems, ...restProps }: Props = $props();
</script>

<Breadcrumb class={restProps.class ?? "mx-5 my-4"}>
    {#each pathItems as item}
        {#if item.isHome}
            <li class="inline-flex items-center">
                <a
                    class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                    use:route
                    href="/">
                    <svg
                        class="w-4 h-4 me-2"
                        fill="currentColor"
                        viewBox="0 0 20 20"
                        xmlns="http://www.w3.org/2000/svg">
                        <path
                            d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
            1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z">
                        </path>
                    </svg>
                    Главная
                </a>
            </li>
        {:else if item.href}
            <li class="inline-flex items-center">
                <svg
                    class="w-6 h-6 text-gray-400 rtl:-scale-x-100"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                    xmlns="http://www.w3.org/2000/svg">
                    <path
                        fill-rule="evenodd"
                        d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0
            z"
                        clip-rule="evenodd">
                    </path>
                </svg>

                <a
                    class="ml-0 ms-1 text-sm font-medium text-gray-700 hover:text-gray-900 md:ms-2 dark:text-gray-400 dark:hover:text-white"
                    use:route
                    href={item.href}>
                    {item.name}
                </a>
            </li>
        {:else}
            <BreadcrumbItem>{item.name}</BreadcrumbItem>
        {/if}
    {/each}
</Breadcrumb>
