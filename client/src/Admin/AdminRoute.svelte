<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import { Link } from "svelte-routing";

    let admins = [];

    onMount(async () => {
        let response = await fetch(`${server_url}/core/admin`, {
            credentials: "include",
        });

        let json = await response.json();
        admins = json;
    });
</script>

<Link to="/create-admin"><button>Create Admin</button></Link>

<hr />

<h2>Admins:</h2>
<table>
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
