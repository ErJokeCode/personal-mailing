<script>
    import http from "../http.js";

    let email = "";
    let password = "";
    let message = http.default_message();

    async function login() {
        message.load = "true";
        message.value = "";
        message.value = await http.http_json("/login", "Post", {
            email,
            password,
        });
        message.load = "false";
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

    <button type="button" on:click={login} aria-busy={message.load}
        >{message.value} Login
    </button>
</form>

<style>
</style>
