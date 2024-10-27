<script>
    import { onMount } from "svelte";
    import { server_url } from "./store";
    import { Link } from "svelte-routing";

    let login_status = "";
    let notifications = [];

    onMount(async () => {
        let response;

        try {
            response = await fetch(`${server_url}/core/admin_notifications`, {
                credentials: "include",
            });
        } catch (err) {
            login_status = "Not Logged In";
        }

        let json = await response.json();
        notifications = json;
    });
</script>

<h1>{login_status}</h1>

<h2>Your Notifications</h2>
<table>
    <tr>
        <th>Content</th>
        <th>Date</th>
        <th>Sent To</th>
    </tr>
    {#each notifications as notification}
        <tr>
            <th>{notification.content}</th>
            <th>{notification.date}</th>
            <th>{notification.studentIds}</th>
        </tr>
    {/each}

    <Link to="/send-notification">Send Notification</Link>
</table>

<style>
    th {
        padding-right: 1rem;
    }
</style>
