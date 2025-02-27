<script lang="ts">
    import {
        ArrowLeftToBracketOutline,
        ArrowRightToBracketOutline,
    } from "flowbite-svelte-icons";
    import { DarkMode, Navbar, ToolbarButton } from "flowbite-svelte";
    import { goto, route } from "@mateothegreat/svelte5-router";
    import { me } from "./stores/me.svelte";
    import { serverUrl } from "./stores/server.svelte";

    function login() {
        goto("/login");
    }

    async function signout() {
        await fetch(`${serverUrl}/core/admins/signout`, {
            credentials: "include",
            method: "POST",
        });

        window.location.replace("/");
    }
</script>

<header class="border-b border-b-gray-200 dark:border-b-gray-600">
    <Navbar fluid={true}>
        <a use:route href="/">
            <span class="text-xl font-semibold dark:text-white">
                Админ панель
            </span>
        </a>

        <div>
            {#if me.value !== null}
                <ToolbarButton class="mr-2" on:click={signout}>
                    <ArrowRightToBracketOutline size="lg" />
                </ToolbarButton>
            {:else}
                <ToolbarButton class="mr-2" on:click={login}>
                    <ArrowLeftToBracketOutline size="lg" />
                </ToolbarButton>
            {/if}

            <DarkMode size="lg" />
        </div>
    </Navbar>
</header>
