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
    <div class="wrapper">
        <header class="pico-background-slate-50">
            <nav class="container">
                <ul></ul>
                <ul>
                    {#if !isEmptyObject($admin)}
                        <li>
                            <Link class="contrast" to="/me">{$admin.email}</Link
                            >
                        </li>
                    {:else}
                        <li><Link to="/login">Login</Link></li>
                    {/if}
                </ul>
            </nav>
        </header>

        <div class="aside-wrapper">
            <aside class="pico-background-slate-50">
                <nav class="container">
                    <details>
                        <summary>Students</summary>
                        <ul>
                            <li><Link to="/upload">Upload Files</Link></li>
                            <li><Link to="/student">All Students</Link></li>
                            <li><Link to="/active">Active Students</Link></li>
                        </ul>
                    </details>

                    <hr />

                    <details>
                        <summary>Notifications</summary>
                        <ul>
                            <li>
                                <Link to="/send-notification"
                                    >Send Notification</Link
                                >
                                <Link to="/notification"
                                    >Your Notifications</Link
                                >
                            </li>
                        </ul>
                    </details>

                    <hr />

                    <details>
                        <summary>Chats</summary>
                        <ul>
                            <li><Link to="/chat">Your Chats</Link></li>
                        </ul>
                    </details>

                    <hr />

                    <details>
                        <summary>Admins</summary>
                        <ul>
                            <li><Link to="/admin">All Admins</Link></li>
                            <li>
                                <Link to="/create-admin">Create Admin</Link>
                            </li>
                        </ul>
                    </details>
                </nav>
            </aside>

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
            </main>
        </div>
    </div>
</Router>

<style>
    .wrapper {
        display: flex;
        flex-flow: column;
        height: 100%;
    }

    .aside-wrapper {
        flex: 1;
        height: 100%;
        display: flex;
        flex-flow: row;
        overflow: scroll;
    }

    aside {
        padding: 1.2rem;
        height: 100%;
        overflow: scroll;
    }

    main {
        padding: 1rem;
        flex: 1;
        width: 100%;
        height: 100%;
        overflow: scroll;
    }
</style>
