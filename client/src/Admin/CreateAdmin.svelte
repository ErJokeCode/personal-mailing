<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import http from "../http.js";

    let message = http.default_message();
    let email = "";
    let password = "";

    async function create() {
        message.load = "true";
        message.value = "";
        message.value = await http.http_json("/core/admin/create", "Post", {
            password,
            email,
        });
        message.load = "false";
    }
</script>

<label>
    Email
    <input type="text" bind:value={email} />
</label>

<label>
    Password
    <input type="password" bind:value={password} />
</label>

<button on:click={create} aria-busy={message.load}
    >{message.value} Create</button
>

<style>
</style>
