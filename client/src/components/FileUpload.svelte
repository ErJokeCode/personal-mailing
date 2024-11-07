<script>
    import http from "src/utility/http.js";

    export let label;
    export let url;
    export let field = "file";

    let files;
    let status = http.status();

    async function send_students() {
        let data = new FormData();

        for (let file of files) {
            data.append(field, file);
        }

        status = status.start_load();
        await http.post(url, data, status);
        status = status.end_load();
    }
</script>

<fieldset>
    <label>
        {label}
        <input type="file" bind:files />
    </label>

    <button on:click={send_students} aria-busy={status.load}>
        {status.value} Upload
    </button>
</fieldset>

<style>
</style>
