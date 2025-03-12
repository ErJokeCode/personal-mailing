import { createPaged } from "/src/lib/components/Paged.svelte";

export const ActiveStudents = $state({
    paged: createPaged(),
    search: "",
    trigger: false,
});