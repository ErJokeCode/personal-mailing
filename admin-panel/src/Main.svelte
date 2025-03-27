<script>
    import { Route } from "svelte-routing"
    
    import Login from "./routes/login/Login.svelte";
    import Profile from "./routes/login/Profile.svelte";

    import Students from "./routes/student/Students.svelte";
    import ActiveStudents from "./routes/student/ActiveStudents.svelte";
    import UploadFiles from "./routes/student/UploadFiles.svelte";
    import SingleActive from "./routes/student/SingleActive.svelte";
    import SingleStudent from "./routes/student/SingleStudent.svelte";

    import Notifications from "./routes/notification/Notifications.svelte";
    import SendNotification from "./routes/notification/SendNotification.svelte";
    import SingleNotification from "./routes/notification/SingleNotification.svelte";
    
    import Admins from "./routes/admin/Admins.svelte";
    import CreateAdmin from "./routes/admin/CreateAdmin.svelte";
    import SingleAdmin from "./routes/admin/SingleAdmin.svelte";

    import Chats from "./routes/chat/Chats.svelte";
    import SingleChat from "./routes/chat/SingleChat.svelte";

    import Builder from "./routes/onboarding/Builder.svelte";
    import Edit from "./routes/subjects/Edit.svelte";

    import AssignGroup from "./routes/groups/AssignGroup.svelte";
    import Faq from "./routes/faq/Faq.svelte";

    import Base from "./routes/base/Base.svelte";
    import BaseAdd from "./routes/base/BaseAdd.svelte";
    import BaseEdit from "./routes/base/BaseEdit.svelte";
</script>

<main class="h-full">
    <Route path="/login" component={Login} />
    <Route path="/profile" component={Profile} />

    <Route path="/students/all" component={Students} />
    <Route path="/students/active" component={ActiveStudents} />
    <Route path="/upload" component={UploadFiles} />
    <Route path="/students/active/:id" component={SingleActive} />
    <Route path="/students/:email" component={SingleStudent} />

    <Route path="/notifications" component={Notifications} />
    <Route path="/notifications/send" component={SendNotification} />
    <Route path="/notifications/:id" component={SingleNotification} />

    <Route path="/admin" component={Admins} />
    <Route path="/admin/create" component={CreateAdmin} />
    <Route path="/admin/:id" component={SingleAdmin} />

    <Route path="/" component={Chats} />
    <Route path="/chat/:studentId" component={SingleChat} />

    <Route path="/builder" component={Builder} />
    <Route path="/names" component={Edit} />

    <Route path="/groups" component={AssignGroup} />
    <Route path="/faq" component={Faq} />

    <Route path="/base" component={Base} />
    <Route path="/base/add" component={BaseAdd} />
    <Route path="/base/edit" component={BaseEdit} />
</main>

<style>
</style>
