<script>
    import { onMount } from "svelte";
    import { server_url } from "../store";
    import { Link } from "svelte-routing";

    let login_status = "";
    let notifications = [];

    onMount(async () => {
        let response;

        try {
            response = await fetch(`${server_url}/core/admin/notifications`, {
                credentials: "include",
            });
        } catch (err) {
            login_status = "Not Logged In";
        }

        let json = await response.json();
        notifications = json;
    });
</script>

<Link to="/send-notification"><button>Send Notification</button></Link>

<hr />

<h2>Your Notifications</h2>
<table>
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
