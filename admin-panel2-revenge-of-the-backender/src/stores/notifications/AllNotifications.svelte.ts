import { createPaged } from "/src/lib/components/Paged.svelte";

export const AllNotifications = $state({
    paged: createPaged(),
    search: "",
});