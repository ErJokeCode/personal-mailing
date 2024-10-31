<script>
    import { onMount } from "svelte";
    import { server_url } from "./store";

    let login_status = "";
    let create_status = "";
    let email = "";
    let password = "";

    async function create() {
        create_status = "Creating...";

        let response;

        try {
            response = await fetch(`${server_url}/core/admin/create`, {
                method: "Post",
                body: JSON.stringify({
                    password: password,
                    email: email,
                }),
                credentials: "include",
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json",
                },
            });
        } catch (err) {
            create_status = "Something went wrong! " + err;
        }

        if (response?.ok) {
            create_status = "You successfully created an admin!";
        }
    }
</script>

<h1>{login_status}</h1>

<button on:click={create}>Create</button>
<span>{create_status}</span>

<br />
<br />

<label>
    Email:
    <input type="text" bind:value={email} />
</label>

<label>
    Password:
    <input type="password" bind:value={password} />
</label>

<style>
</style>
