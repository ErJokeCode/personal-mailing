import { createPaged } from "/src/lib/components/Paged.svelte";

export const AllAdmins = $state({
    search: "",
    paged: createPaged(),
});
