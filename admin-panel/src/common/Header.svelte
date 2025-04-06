<script lang="ts">
    import {
        ArrowLeftToBracketOutline,
        ArrowRightToBracketOutline,
    } from "flowbite-svelte-icons";
    import { DarkMode, Navbar, NavHamburger, ToolbarButton } from "flowbite-svelte";
    import { goto, route } from "@mateothegreat/svelte5-router";
    import { Me } from "/src/stores/Me.svelte";
    import { AdminsApi } from "/src/lib/server";
    import { RouteHistory } from "/src/stores/RouteHistory.svelte";
    import NotificationList from "./NotificationList.svelte";

    function login() {
        goto("/login");
    }

    async function signout() {
        await fetch(`${AdminsApi}/signout`, {
            credentials: "include",
            method: "POST",
        });

        window.location.replace("/");
    }

    function resetHistory() {
        RouteHistory.isClear = true;
    }
    
    export let drawerHidden = false;

    let fluid = true;
</script>

<header class="border-b border-b-gray-200 dark:border-b-gray-600">
    <Navbar {fluid} class="text-black">
        {#if Me.value !== null}
            <NavHamburger
                onClick={() => (drawerHidden = !drawerHidden)}
                class="m-0 me-3 md:block lg:hidden"
            />
        {/if}
        <a use:route on:click={resetHistory} href="/" class="lg:w-60">
            <span class="ml-px self-center whitespace-nowrap text-xl font-semibold dark:text-white sm:text-2xl px-5">
                Админ панель
            </span>
        </a>

        <div class="ms-auto flex items-center text-gray-500 dark:text-gray-400 sm:order-2">
            {#if Me.value !== null}
                <ToolbarButton class="mr-2 hover:text-gray-900 dark:hover:text-white" on:click={signout}>
                    <ArrowRightToBracketOutline size="lg" />
                </ToolbarButton>
                <NotificationList />
            {:else}
                <ToolbarButton class="mr-2 hover:text-gray-900 dark:hover:text-white" on:click={login}>
                    <ArrowLeftToBracketOutline size="lg" />
                </ToolbarButton>
            {/if}

            <DarkMode size="md" />
        </div>
    </Navbar>
</header>
