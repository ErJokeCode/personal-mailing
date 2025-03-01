<script lang="ts">
    import CreateAdmin from "/src/routes/admins/CreateAdmin.svelte";
    import { goto, Router, type Route } from "@mateothegreat/svelte5-router";
    import { Me } from "/src/stores/Me.svelte";
    import { AdminsApi } from "/src//lib/server";

    import Login from "/src/routes/admins/Login.svelte";
    import AllAdmins from "/src/routes/admins/AllAdmins.svelte";
    import NotFound from "/src/routes/NotFound.svelte";
    import SingleAdmin from "/src/routes/admins/SingleAdmin.svelte";
    import Profile from "/src/routes/admins/Profile.svelte";

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
            path: ".+",
            component: NotFound,
        },
    ];
</script>

<Router hooks={{ pre: authPreHook }} basePath="/" {routes} />
