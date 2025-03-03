<script lang="ts">
    import { goto, type Route } from "@mateothegreat/svelte5-router";
    import { Button, Heading } from "flowbite-svelte";
    import { ArrowLeftOutline } from "flowbite-svelte-icons";
    import { twMerge } from "tailwind-merge";

    interface Props {
        route: Route;
        fallback?: string;
        title?: string;
        class?: string;
    }

    let { route, fallback, title, ...restProps }: Props = $props();
    let returnUrl = route.query?.["returnUrl"] ?? null;

    let baseClass = "flex items-center";
    let contentClass = $derived(twMerge([baseClass, restProps.class]));

    async function returnBack() {
        goto(returnUrl ?? fallback);
    }
</script>

{#if returnUrl || fallback}
    <div class={contentClass}>
        <Button class="mr-2" on:click={returnBack}>
            <ArrowLeftOutline />
        </Button>

        {#if title}
            <Heading tag="h2">{title}</Heading>
        {/if}
    </div>
{/if}
