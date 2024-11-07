<script>
    import { onMount } from "svelte";
    import http from "src/utility/http";
    import { navigate } from "svelte-routing";

    let admins = [];
    let status = http.status();

    onMount(async () => {
        status = status.start_load();
        admins = (await http.get("/core/admin", status)) ?? [];
        status = status.end_load();
    });

    async function fullInfo(id) {
        navigate(`/admin/${id}`);
    }
</script>

<h2>Admins:</h2>
<table aria-busy={status.load}>
    <thead>
        <tr>
            <th>Email</th>
            <th>Date</th>
        </tr>
    </thead>
    {#each admins as admin}
        <tr role="link" class="contrast" on:click={() => fullInfo(admin.id)}>
            <th>{admin.email}</th>
            <th>{admin.date}</th>
        </tr>
    {/each}
</table>

<style>
</style>
