<script>
    import { onMount } from "svelte";
    import * as signalR from "@microsoft/signalr";
    import { server_url } from "./store.js";

    let login_status = "";
    let student_files;
    let student_success = "";
    let courses_files;
    let courses_success = "";
    let auth_students = [];

    async function getStudents() {
        let response;

        try {
            response = await fetch(`${server_url}/core/students`, {
                credentials: "include",
            });
        } catch (err) {
            auth_students = [];
        }

        if (response?.ok) {
            let json = await response.json();
            auth_students = json;
        }
    }

    onMount(async () => {
        getStudents();

        let response;

        try {
            response = await fetch(
                `${server_url}/parser/course/by_email?email=nikita.shishkov@urfu.me`,
                {
                    credentials: "include",
                },
            );
        } catch (err) {
            login_status = "Not Logged In";
        }

        if (response?.ok) {
            login_status = "Logged In";
        }

        let connection = new signalR.HubConnectionBuilder()
            .withUrl(`${server_url}/signal`, {
                transport: signalR.HttpTransportType.ServerSentEvents,
                withCredentials: false,
            })
            .build();

        connection.on("NewStudentAuthed", function (_) {
            getStudents();
        });

        connection.start();
    });

    async function send_students() {
        if (student_files.length <= 0) return;

        let file = student_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch(`${server_url}/parser/upload/auth_student`, {
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

        let result = await fetch(`${server_url}/parser/upload/report_ok`, {
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
    <h2>{login_status}</h2>

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

    <h2>Authed Students</h2>
    <table>
        <tr>
            <th>Email</th>
        </tr>
        {#each auth_students as student}
            <tr>
                <th>{student.email}</th>
            </tr>
        {/each}
    </table>
</main>

<style>
</style>
