import { createPaged } from "/src/lib/components/Paged.svelte";

export const Chats = $state({
    paged: createPaged(),
    search: "",
});