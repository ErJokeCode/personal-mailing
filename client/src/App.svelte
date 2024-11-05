<script>
    import { Router, Link, Route } from "svelte-routing";
    import UploadRoute from "./Upload/UploadRoute.svelte";
    import LoginRoute from "./Login/LoginRoute.svelte";
    import NotificationRoute from "./Notification/NotificationRoute.svelte";
    import SendNotification from "./Notification/SendNotification.svelte";
    import AdminRoute from "./Admin/AdminRoute.svelte";
    import CreateAdmin from "./Admin/CreateAdmin.svelte";
    import ChatRoute from "./Chat/ChatRoute.svelte";

    export let url = "";
</script>

<Router {url}>
    <nav class="container">
        <ul>
            <li><Link to="/login">Login</Link></li>
            <li><Link to="/upload">Upload</Link></li>
            <li><Link to="/notification">Notification</Link></li>
            <li><Link to="/admin">Admin</Link></li>
            <li><Link to="/chat">Chat</Link></li>
        </ul>
    </nav>

    <br />

    <main class="container">
        <Route path="/login" component={LoginRoute} />
        <Route path="/upload" component={UploadRoute} />
        <Route path="/notification" component={NotificationRoute} />
        <Route path="/send-notification" component={SendNotification} />
        <Route path="/admin" component={AdminRoute} />
        <Route path="/create-admin" component={CreateAdmin} />
        <Route path="/chat" component={ChatRoute} />
        <main></main>
    </main>
</Router>
