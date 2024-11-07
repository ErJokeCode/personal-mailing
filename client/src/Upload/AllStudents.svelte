<script>
    import { onMount } from "svelte";
    import http from "../http";

    let status = http.status();
    let students = [];

    let maxPage = 0;
    let curPage = 0;
    let amountPage = 100;
    let select;

    async function toPage(page) {
        if (page < 0) {
            page = 0;
        } else if (page > maxPage) {
            page = maxPage;
        }

        select.value = page;
        curPage = page;
    }

    onMount(async () => {
        status = status.start_load();
        students = (await http.get("/parser/student/all", status)) ?? [];
        status = status.end_load();
        maxPage = Math.floor(students.length / amountPage);
    });
</script>

<div class="grid">
    <h2>All Students</h2>
    <button on:click={() => toPage(curPage - 1)}>Prev</button>
    <select name="select" aria-label="Select" bind:this={select}>
        {#each [...Array(maxPage + 1).keys()] as page}
            <option on:click={() => toPage(page)}>{page}</option>
        {/each}
    </select>
    <button on:click={() => toPage(curPage + 1)}>Next</button>
</div>

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
        {#each students.slice(curPage * amountPage, curPage * amountPage + amountPage) as student}
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
    .grid {
        display: flex;
        align-items: center;
    }
    h2 {
        flex: 1;
    }
    select {
        width: fit-content;
        margin-bottom: 0;
    }
</style>
