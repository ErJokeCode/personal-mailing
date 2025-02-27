<script lang="ts">
    import { serverUrl } from "./stores/server.svelte";
    import { goto, Router, type Route } from "@mateothegreat/svelte5-router";
    import Header from "./Header.svelte";
    import Sidebar from "./Sidebar.svelte";
    import Profile from "./routes/admins/Profile.svelte";
    import Login from "./routes/admins/Login.svelte";
    import { me } from "./stores/me.svelte";

    const authPreHook = async (route: Route): Promise<boolean> => {
        if (route.path == "login") {
            return true;
        }

        try {
            let res = await fetch(`${serverUrl}/core/admins/me`, {
                credentials: "include",
            });

            if (!res.ok) {
                throw new Error();
            }

            let body = await res.json();
            me.value = body;
            return true;
        } catch {
            goto("/login");
            return false;
        }
    };

    const routes: Route[] = [
        {
            path: "",
            component: "",
        },
        {
            path: "profile",
            component: Profile,
        },
        {
            path: "login",
            component: Login,
        },
    ];
</script>

<div class="h-full flex flex-col">
    <Header />

    <div class="h-full flex overflow-hidden">
        {#if me.value !== null}
            <Sidebar />
        {/if}

        <div class="h-full flex-col flex-1 overflow-scroll">
            <Router hooks={{ pre: authPreHook }} basePath="/" {routes} />
        </div>
    </div>
</div>
