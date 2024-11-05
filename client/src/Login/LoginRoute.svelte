<script>
    import http from "../http.js";
    import { admin } from "../store.js";
    import { navigate } from "svelte-routing";

    let email = "";
    let password = "";
    let status = http.status();

    async function login() {
        status = status.start_load();

        await http.post_json(
            "/login",
            {
                email,
                password,
            },
            status,
        );

        status = status.end_load();

        let new_admin = { email: email, date: "" };

        admin.update((_) => new_admin);
        navigate("/me");
    }
</script>

<form>
    <label>
        Email
        <input type="text" bind:value={email} />
    </label>

    <label>
        Password
        <input type="text" bind:value={password} />
    </label>

    <button type="button" on:click={login} aria-busy={status.load}
        >{status.value} Login
    </button>
</form>

<style>
</style>
