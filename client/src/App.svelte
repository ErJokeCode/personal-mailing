<script>
    import { onMount } from "svelte";
    import * as signalR from "@microsoft/signalr";

    let student_files;
    let student_success = "";
    let courses_files;
    let courses_success = "";
    let auth_students = 0;

    onMount(async () => {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5002/signal", {
                transport: signalR.HttpTransportType.ServerSentEvents,
                withCredentials: false,
            })
            .build();

        connection.on("NewStudentAuth", function (_) {
            auth_students += 1;
        });

        connection.start();
    });

    async function send_students() {
        if (student_files.length <= 0) return;

        let file = student_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch("http://localhost:5000/upload/auth_student", {
            method: "POST",
            body: data,
        });

        if (result.ok) {
            student_success = "Success";
        } else {
            student_success = "Error";
        }
    }

    async function send_courses() {
        if (courses_files.length <= 0) return;

        let file = courses_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch("http://localhost:5000/upload/report_ok", {
            method: "POST",
            body: data,
        });

        if (result.ok) {
            student_success = "Success";
        } else {
            student_success = "Error";
        }
    }
</script>

<main>
    <h2>Authed Students: {auth_students}</h2>

    <label>
        Students File
        <input type="file" bind:files={student_files} />
    </label>
    <button on:click={send_students}>Send Students</button>
    <span>{student_success}</span>

    <br />

    <label>
        Courses File
        <input type="file" bind:files={courses_files} />
    </label>
    <button on:click={send_courses}>Send Courses</button>
    <span>{courses_success}</span>
</main>

<style>
</style>
