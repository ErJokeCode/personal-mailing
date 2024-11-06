<script>
    import { onMount } from "svelte";
    import http from "../http.js";
    import { navigate } from "svelte-routing";

    let activeStudents = [];

    let studentStatus = http.status();

    onMount(async () => {
        studentStatus = studentStatus.start_load();
        activeStudents = (await http.get("/core/student", studentStatus)) ?? [];
        studentStatus = studentStatus.end_load();
    });

    async function start_chat(studentId) {
        navigate(`/chat/${studentId}`);
    }
</script>

<h2>Active Students</h2>

<table aira-busy={studentStatus.load}>
    <thead>
        <tr>
            <th>Id</th>
            <th>Email</th>
            <th>Chat</th>
        </tr>
    </thead>
    <tbody>
        {#each activeStudents as student}
            <tr>
                <th>{student.id}</th>
                <th>{student.email}</th>
                <th>
                    <button on:click={() => start_chat(student.id)}
                        >Start Chat</button
                    >
                </th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
