<script lang="ts">
    import {
        goto,
        QueryString,
        type Route,
    } from "@mateothegreat/svelte5-router";
    import { Button, Heading } from "flowbite-svelte";
    import { ArrowLeftOutline } from "flowbite-svelte-icons";
    import { twMerge } from "tailwind-merge";
    import { RouteHistory } from "/src/stores/RouteHistory.svelte";

    interface Props {
        title?: string;
        class?: string;
    }

    let { title, ...restProps }: Props = $props();

    let baseClass = "flex items-center";
    let contentClass = $derived(twMerge([baseClass, restProps.class]));

    function returnBack() {
        if (RouteHistory.current > 0) {
            var path = RouteHistory.values.at(RouteHistory.current - 1);
            RouteHistory.isReturn = true;
            goto(path);
        }
    }
</script>

{#if RouteHistory.current > 0}
    <div class={contentClass}>
        <Button class="mr-2" on:click={returnBack}>
            <ArrowLeftOutline />
        </Button>

        {#if title}
            <Heading tag="h2">{title}</Heading>
        {/if}
    </div>
{/if}
