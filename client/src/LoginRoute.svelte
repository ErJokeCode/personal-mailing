<script>
    import { server_url } from "./store.js";

    let email = "";
    let password = "";
    let message = "";

    async function login() {
        message = "Logging in...";

        let response;

        try {
            response = await fetch(`${server_url}/login`, {
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
        } catch (err) {
            message = "Something went wrong! " + response.statusText;
        }

        if (response?.ok) {
            message = "You successfully logged in!";
        }
    }
</script>

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
