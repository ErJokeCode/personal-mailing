<script>
    import { Router, Link, Route } from "svelte-routing";
    import UploadRoute from "./UploadRoute.svelte";
    import LoginRoute from "./LoginRoute.svelte";
    import NotificationRoute from "./NotificationRoute.svelte";
    import SendNotification from "./SendNotification.svelte";
    import AdminRoute from "./AdminRoute.svelte";
    import CreateAdmin from "./CreateAdmin.svelte";

    export let url = "";
</script>

<Router {url}>
    <nav>
        <Link to="/login">Login</Link>
        <Link to="/upload">Upload</Link>
        <Link to="/notification">Notification</Link>
        <Link to="/admin">Admin</Link>
    </nav>

    <br />

    <div>
        <Route path="/login" component={LoginRoute} />
        <Route path="/upload" component={UploadRoute} />
        <Route path="/notification" component={NotificationRoute} />
        <Route path="/send-notification" component={SendNotification} />
        <Route path="/admin" component={AdminRoute} />
        <Route path="/create-admin" component={CreateAdmin} />
    </div>
</Router>
