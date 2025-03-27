import { createPaged } from "/src/lib/components/Paged.svelte";

export const AllAdmins = $state({
    search: "",
    paged: createPaged(),
});

export const AllGroups = $state({
    adminId: null,
    search: "",
    paged: createPaged(),
    trigger: false,
    selectAll: false,
    selected: new Map<string, object>(),
});