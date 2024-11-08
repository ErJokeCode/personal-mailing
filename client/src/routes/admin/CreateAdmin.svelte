<script>
    import http from "src/utility/http.js";

    let status = http.status();
    let email = "";
    let password = "";

    async function create() {
        status = status.start_load();
        await http.post_json(
            "/core/admin/create",
            {
                password,
                email,
            },
            status,
        );
        status = status.end_load();
    }
</script>

<label>
    Почта
    <input type="text" bind:value={email} />
</label>

<label>
    Пароль
    <input type="password" bind:value={password} />
</label>

<button on:click={create} aria-busy={status.load}>{status.value} Создать</button>

<style>
</style>
