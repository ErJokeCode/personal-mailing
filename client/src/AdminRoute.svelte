<script>
    import { onMount } from "svelte";
    import { server_url } from "./store";
    import { Link } from "svelte-routing";

    let login_status = "";
    let admins = [];

    onMount(async () => {
        let response;

        try {
            response = await fetch(`${server_url}/core/admin`, {
                credentials: "include",
            });
        } catch (err) {
            login_status = "Not Logged In";
        }

        let json = await response.json();
        admins = json;
    });
</script>

<h1>{login_status}</h1>

<h2>Admins:</h2>
<table>
    <tr>
        <th>Email</th>
    </tr>
    {#each admins as admin}
        <tr>
            <th>{admin.email}</th>
        </tr>
    {/each}

    <Link to="/create-admin">Create Admin</Link>
</table>

<style>
    th {
        padding-right: 1rem;
    }
</style>
