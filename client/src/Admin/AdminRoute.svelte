<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import { Link } from "svelte-routing";
    import http from "../http";

    let admins = [];
    let status = http.status();

    onMount(async () => {
        status = status.start_load();
        admins = (await http.get("/core/admin", status)) ?? [];
        status = status.end_load();
    });
</script>

<Link to="/create-admin"><button>Create Admin</button></Link>

<hr />

<h2>Admins:</h2>
<table aria-busy={status.load}>
    <thead>
        <tr>
            <th>Email</th>
            <th>Date</th>
        </tr>
    </thead>
    {#each admins as admin}
        <tr>
            <th>{admin.email}</th>
            <th>{admin.date}</th>
        </tr>
    {/each}
</table>

<style>
</style>
