<script>
    import { server_url } from "./store.js";

    let email = "";
    let password = "";
    let message = "";

    async function login() {
        let response = await fetch(`${server_url}/login`, {
            method: "Post",
            body: JSON.stringify({
                email: email,
                password: password,
            }),
            credentials: "include",
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
            },
        });

        if (!response.ok) {
            message = "Something went wrong! " + response.statusText;
        } else {
            message = "You successfully logged in!";
        }
    }
</script>

<br />

<label>
    Email
    <input type="text" bind:value={email} />
</label>

<br />

<label>
    Password
    <input type="text" bind:value={password} />
</label>

<br />

<button type="button" on:click={login}>Login</button>

<div>{message}</div>

<style>
</style>
