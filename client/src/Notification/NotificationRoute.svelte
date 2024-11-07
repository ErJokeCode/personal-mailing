<script>
    import { onMount } from "svelte";
    import http from "../http";
    import { Link } from "svelte-routing";

    let status = http.status();
    let notifications = [];

    onMount(async () => {
        status = status.start_load();
        notifications =
            (await http.get("/core/admin/notifications", status)) ?? [];
        status = status.end_load();
    });
</script>
<!---->
<!-- <Link to="/send-notification"><button>Send Notification</button></Link> -->
<!---->
<!-- <hr /> -->

<h2>Your Notifications</h2>
<table aria-busy={status.load}>
    <thead>
        <tr>
            <th>Content</th>
            <th>Date</th>
            <th>Sent To</th>
        </tr>
    </thead>
    <tbody>
        {#each notifications as notification}
            <tr>
                <th>{notification.content}</th>
                <th>{notification.date}</th>
                <th>
                    {#each notification.students as student}
                        {student.email},
                    {/each}
                </th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
