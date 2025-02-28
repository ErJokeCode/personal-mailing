<script lang="ts">
    import { Heading } from "flowbite-svelte";
    import AdminInfo from "./shared/AdminInfo.svelte";
    import { me } from "/src/stores/me.svelte";
</script>

<Heading tag="h2" class="m-4">Профиль</Heading>

<AdminInfo body={me.value} />
