<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import { navigate } from "svelte-routing";

    let chats = [];

    onMount(async () => {
        let response = await fetch(`${server_url}/core/admin/chats`, {
            credentials: "include",
        });

        let json = await response.json();
        chats = json;
    });

    async function open_chat(id) {
        navigate("/chat/" + id);
    }
</script>

<h2>Your Chats:</h2>

<table>
    <thead>
        <tr>
            <th>Chat</th>
            <th>Email</th>
        </tr>
    </thead>
    <tbody>
        {#each chats as chat}
            <tr>
                <th
                    ><button on:click={() => open_chat(chat.id)}>Open</button
                    ></th
                >
                <th>{chat.student.email}</th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
