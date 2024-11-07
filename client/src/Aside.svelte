<script>
    import { Link } from "svelte-routing";
    import Accordion from "src/components/Accordion.svelte";
</script>

<aside class="pico-background-slate-50">
    <nav class="container">
        <Accordion summary="Students">
            <li><Link to="/student/upload">Upload Files</Link></li>
            <li><Link to="/student">All Students</Link></li>
            <li><Link to="/student/active">Active Students</Link></li>
        </Accordion>

        <Accordion summary="Notifications">
            <li>
                <Link to="/notification/send">Send Notification</Link>
                <Link to="/notification">Your Notifications</Link>
            </li>
        </Accordion>

        <Accordion summary="Chats">
            <li><Link to="/chat">Your Chats</Link></li>
        </Accordion>

        <Accordion summary="Admins">
            <li><Link to="/admin">All Admins</Link></li>
            <li>
                <Link to="/admin/create">Create Admin</Link>
            </li>
        </Accordion>
    </nav>
</aside>

<style>
    aside {
        padding: 1.2rem;
        height: 100%;
        overflow: scroll;
    }
</style>
