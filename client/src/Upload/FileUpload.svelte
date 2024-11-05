<script>
    import http from "../http.js";

    export let label;
    export let url;
    export let field = "file";

    let files;
    let message = http.default_message();

    async function send_students() {
        message.load = "true";
        message.value = "";
        message.value = await http.http_files(url, "Post", field, files);
        message.load = "false";
    }
</script>

<fieldset>
    <label>
        {label}
        <input type="file" multiple bind:files />
    </label>

    <button on:click={send_students} aria-busy={message.load}>
        {message.value} Upload
    </button>
</fieldset>

<style>
</style>
