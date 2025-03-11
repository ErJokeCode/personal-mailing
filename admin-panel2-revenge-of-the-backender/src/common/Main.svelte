<script lang="ts">
    import { RouteHistory } from "/src/stores/RouteHistory.svelte";
    import AllGroups from "/src//routes/groups/AllGroups.svelte";
    import CreateAdmin from "/src/routes/admins/CreateAdmin.svelte";
    import { goto, Router, type Route } from "@mateothegreat/svelte5-router";
    import { Me } from "/src/stores/Me.svelte";
    import { AdminsApi } from "/src//lib/server";

    import Login from "/src/routes/admins/Login.svelte";
    import AllAdmins from "/src/routes/admins/AllAdmins.svelte";
    import NotFound from "/src/routes/NotFound.svelte";
    import SingleAdmin from "/src/routes/admins/SingleAdmin.svelte";
    import Profile from "/src/routes/admins/Profile.svelte";
    import Edit from "/src/routes/subjects/Edit.svelte";
    import Faq from "/src/routes/faq/Faq.svelte";
    import Builder from "/src/routes/onboarding/Builder.svelte";
    import AllNotifications from "../routes/notifications/AllNotifications.svelte";
    import UploadFiles from "../routes/uploads/UploadFiles.svelte";
    import SingleNotification from "../routes/notifications/SingleNotification.svelte";
    import SendNotification from "../routes/notifications/SendNotification.svelte";
    import AllStudents from "../routes/students/AllStudents.svelte";
    import ActiveStudents from "../routes/students/ActiveStudents.svelte";
    import SingleStudent from "../routes/students/SingleStudent.svelte";
    import Chats from "../routes/chats/Chats.svelte";
    import SingleChat from "../routes/chats/SingleChat.svelte";
    import Base from "../routes/base/Base.svelte";
    import BaseAdd from "../routes/base/BaseAdd.svelte";
    import BaseEdit from "../routes/base/BaseEdit.svelte";

    const authPreHook = async (route: Route): Promise<boolean> => {
        if (route.path == "login") {
            return true;
        }

        try {
            let res = await fetch(`${AdminsApi}/me`, {
                credentials: "include",
            });

            if (!res.ok) {
                throw new Error();
            }

            let body = await res.json();
            Me.value = body;
            return true;
        } catch {
            goto("/login");
            return false;
        }
    };

    const globalPostHook = async (route: Route): Promise<void> => {
        if (RouteHistory.isReturn) {
            RouteHistory.isReturn = false;
            RouteHistory.values.pop();
            RouteHistory.current -= 1;
            return;
        } else if (RouteHistory.isClear) {
            RouteHistory.isClear = false;
            RouteHistory.values = [];
            RouteHistory.current = -1;
        }

        RouteHistory.values.push(window.location.pathname);
        RouteHistory.current += 1;
    };

    const routes: Route[] = [
        {
            path: "",
            component: Profile,
        },
        {
            path: "login",
            component: Login,
        },
        {
            path: "profile",
            component: Profile,
        },
        {
            path: "create-admin",
            component: CreateAdmin,
        },
        {
            path: "admins/(?<adminId>.*)",
            component: SingleAdmin,
        },
        {
            path: "admins",
            component: AllAdmins,
        },
        {
            path: "groups",
            component: AllGroups,
        },
        {
            path: "notifications/(?<notificationId>.*)",
            component: SingleNotification,
        },
        {
            path: "notifications",
            component: AllNotifications,
        },
        {
            path: "send-notification",
            component: SendNotification,
        },
        {
            path: "students/(?<email>.*)",
            component: SingleStudent,
        },
        {
            path: "students",
            component: AllStudents,
        },
        {
            path: "active-students",
            component: ActiveStudents,
        },
        {
            path: "chats/(?<studentId>.*)",
            component: SingleChat,
        },
        {
            path: "chats",
            component: Chats,
        },
        {
            path: "upload",
            component: UploadFiles,
        },
        {
            path: "subjects",
            component: Edit,
        },
        {
            path: "faq",
            component: Faq,
        },
        {
            path: "builder",
            component: Builder,
        },
        {
            path: "base",
            component: Base,
        },
        {
            path: "base-add",
            component: BaseAdd,
        },
        {
            path: "base-edit",
            component: BaseEdit,
        },
        {
            path: ".+",
            component: NotFound,
        },
    ];
</script>

<Router
    hooks={{ pre: authPreHook, post: globalPostHook }}
    basePath="/"
    {routes} />
