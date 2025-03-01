<script lang="ts">
    import { Heading } from "flowbite-svelte";
    import AdminInfo from "./shared/AdminInfo.svelte";
    import { Me } from "/src/stores/Me.svelte";
</script>

<Heading tag="h2" class="m-4">Профиль</Heading>

<AdminInfo body={Me.value} />
