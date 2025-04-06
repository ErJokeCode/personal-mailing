<script lang="ts">
    import { goto } from "@mateothegreat/svelte5-router";
    import { Alert, Button } from "flowbite-svelte";
    import { HomeOutline, InfoCircleOutline } from "flowbite-svelte-icons";
    import type { Snippet } from "svelte";

    interface Props {
        title: string;
        children: Snippet;
        class?: string;
    }

    let { title, children, ...restProps }: Props = $props();

    function toHome() {
        goto("/");
    }
</script>

<Alert class={restProps.class ?? "" + "m-4"}>
    <div class="flex items-center gap-2">
        <InfoCircleOutline class="w-6 h-6" />
        <span class="text-2xl font-medium">{title}</span>
    </div>
    <p class="mt-2 mb-4 text-xl">
        {@render children()}
    </p>
    <div class="flex gap-2">
        <Button size="xs" outline on:click={toHome}><HomeOutline /></Button>
    </div>
</Alert>
