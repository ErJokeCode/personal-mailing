<script>
    import { onMount } from "svelte";
    import http from "../http";
    import { Link } from "svelte-routing";

    let status = http.status();
    let students = [];

    onMount(async () => {
        status = status.start_load();
        students = (await http.get("/parser/student/all", status)) ?? [];
        status = status.end_load();
    });
</script>

<Link to="/upload"><button>Upload Files</button></Link>

<hr />

<h2>All Students</h2>
<table aria-busy={status.load}>
    <thead>
        <tr>
            <th>Email</th>
            <th>Personal Number</th>
            <th>Name</th>
            <th>Surname</th>
        </tr>
    </thead>
    <tbody>
        {#each students as student}
            <tr>
                <th>{student.email}</th>
                <th>{student.personal_number}</th>
                <th>{student.name}</th>
                <th>{student.surname}</th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
