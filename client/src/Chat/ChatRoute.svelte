<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";

    let activeStudents = [];

    onMount(async () => {
        let response = await fetch(`${server_url}/core/student`, {
            credentials: "include",
        });

        let json = await response.json();
        activeStudents = json;
    });
</script>

<table>
    <thead>
        <tr>
            <th>Chat</th>
            <th>Email</th>
        </tr>
    </thead>
    <tbody>
        {#each activeStudents as student}
            <tr>
                <th><button>Open</button></th>
                <th>{student.email}</th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
