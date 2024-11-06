<script>
    import { admin } from "./store";

    import { Router, Link, Route } from "svelte-routing";
    import UploadRoute from "./Upload/UploadRoute.svelte";
    import LoginRoute from "./Login/LoginRoute.svelte";
    import NotificationRoute from "./Notification/NotificationRoute.svelte";
    import SendNotification from "./Notification/SendNotification.svelte";
    import AdminRoute from "./Admin/AdminRoute.svelte";
    import CreateAdmin from "./Admin/CreateAdmin.svelte";
    import ChatRoute from "./Chat/ChatRoute.svelte";
    import Chat from "./Chat/Chat.svelte";
    import Profile from "./Admin/Profile.svelte";
    import AllStudents from "./Upload/AllStudents.svelte";
    import ActiveRoute from "./Login/ActiveRoute.svelte";

    export let url = "";

    function isEmptyObject(obj) {
        return JSON.stringify(obj) === "{}";
    }
</script>

<Router {url}>
    <div class="pico-background-slate-900">
        <nav class="container">
            <ul>
                <li><Link to="/student">Student</Link></li>
                <li><Link to="/active">Active</Link></li>
                <li><Link to="/notification">Notification</Link></li>
                <li><Link to="/admin">Admin</Link></li>
                <li><Link to="/chat">Chat</Link></li>
            </ul>
            <ul>
                {#if !isEmptyObject($admin)}
                    <li>
                        <Link class="contrast" to="/me">{$admin.email}</Link>
                    </li>
                {:else}
                    <li><Link to="/login">Login</Link></li>
                {/if}
            </ul>
        </nav>
    </div>

    <br />

    <main class="container">
        <Route path="/me" component={Profile} />
        <Route path="/active" component={ActiveRoute} />
        <Route path="/login" component={LoginRoute} />
        <Route path="/upload" component={UploadRoute} />
        <Route path="/notification" component={NotificationRoute} />
        <Route path="/send-notification" component={SendNotification} />
        <Route path="/admin" component={AdminRoute} />
        <Route path="/create-admin" component={CreateAdmin} />
        <Route path="/chat" component={ChatRoute} />
        <Route path="/student" component={AllStudents} />
        <Route path="/chat/:studentId" component={Chat} />
        <main></main>
    </main>
</Router>

<style>
</style>
