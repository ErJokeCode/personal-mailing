<script>
    import { onMount } from "svelte";
    import http from "src/utility/http.js";
    import { navigate } from "svelte-routing";

    let activeStudents = [];

    let studentStatus = http.status();

    onMount(async () => {
        studentStatus = studentStatus.start_load();
        activeStudents = (await http.get("/core/student", studentStatus)).items ?? [];
        studentStatus = studentStatus.end_load();
    });

    async function fullInfo(id) {
        navigate(`/student/active/${id}`);
    }
</script>

<h2>Активные студенты</h2>

<table aira-busy={studentStatus.load}>
    <thead>
        <tr>
            <th>Айди</th>
            <th>Почта</th>
        </tr>
    </thead>
    <tbody>
        {#each activeStudents as student}
            <tr
                role="link"
                class="contrast"
                on:click={() => fullInfo(student.id)}
            >
                <th>{student.id}</th>
                <th>{student.email}</th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
