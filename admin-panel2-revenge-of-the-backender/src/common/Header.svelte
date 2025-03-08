<script lang="ts">
    import {
        ArrowLeftToBracketOutline,
        ArrowRightToBracketOutline,
    } from "flowbite-svelte-icons";
    import { DarkMode, Navbar, ToolbarButton } from "flowbite-svelte";
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
</script>

<header class="border-b border-b-gray-200 dark:border-b-gray-600">
    <Navbar fluid={true}>
        <a use:route on:click={resetHistory} href="/">
            <span class="text-xl font-semibold dark:text-white">
                Админ панель
            </span>
        </a>

        <div>
            {#if Me.value !== null}
                <ToolbarButton class="mr-2" on:click={signout}>
                    <ArrowRightToBracketOutline size="lg" />
                </ToolbarButton>
            {:else}
                <ToolbarButton class="mr-2" on:click={login}>
                    <ArrowLeftToBracketOutline size="lg" />
                </ToolbarButton>
            {/if}

            <NotificationList />

            <DarkMode size="lg" />
        </div>
    </Navbar>
</header>
