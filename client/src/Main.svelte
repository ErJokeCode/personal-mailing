<script>
    import { Route } from "svelte-routing";

    import Login from "src/routes/login/Login.svelte";
    import Profile from "src/routes/login/Profile.svelte";

    import Students from "src/routes/student/Students.svelte";
    import SingleStudent from "src/routes/student/SingleStudent.svelte";
    import SingleActive from "src/routes/student/SingleActive.svelte";
    import ActiveStudents from "src/routes/student/ActiveStudents.svelte";
    import UploadFiles from "src/routes/student/UploadFiles.svelte";

    import Notifications from "src/routes/notification/Notifications.svelte";
    import SendNotification from "src/routes/notification/SendNotification.svelte";
    import SingleNotification from "src/routes/notification/SingleNotification.svelte";

    import Admins from "src/routes/admin/Admins.svelte";
    import CreateAdmin from "src/routes/admin/CreateAdmin.svelte";
    import SingleAdmin from "src/routes/admin/SingleAdmin.svelte";

    import Chats from "src/routes/chat/Chats.svelte";
    import SingleChat from "src/routes/chat/SingleChat.svelte";
</script>

<main class="container">
    <Route path="/login" component={Login} />
    <Route path="/me" component={Profile} />

    <Route path="/student" component={Students} />
    <Route path="/student/active" component={ActiveStudents} />
    <Route path="/student/upload" component={UploadFiles} />
    <Route path="/student/active/:id" component={SingleActive} />
    <Route path="/student/:email" component={SingleStudent} />

    <Route path="/notification" component={Notifications} />
    <Route path="/notification/send" component={SendNotification} />
    <Route path="/notification/:id" component={SingleNotification} />

    <Route path="/admin" component={Admins} />
    <Route path="/admin/create" component={CreateAdmin} />
    <Route path="/admin/:id" component={SingleAdmin} />

    <Route path="/chat" component={Chats} />
    <Route path="/chat/:studentId" component={SingleChat} />
</main>

<style>
    main {
        padding: 1rem;
        flex: 1;
        width: 100%;
        height: 100%;
        overflow: scroll;
    }
</style>
