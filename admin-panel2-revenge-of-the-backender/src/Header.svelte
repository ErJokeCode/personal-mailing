<script lang="ts">
    import { ArrowRightToBracketOutline } from "flowbite-svelte-icons";
    import { DarkMode, Navbar, NavBrand, ToolbarButton } from "flowbite-svelte";
    import { route } from "@mateothegreat/svelte5-router";
</script>

<header class="border-b border-b-gray-200 dark:border-b-gray-600">
    <Navbar fluid={true}>
        <a use:route href="/">
            <span class="text-xl font-semibold dark:text-white">
                Админ панель
            </span>
        </a>

        <div>
            <ToolbarButton class="mr-2">
                <ArrowRightToBracketOutline size="lg" />
            </ToolbarButton>

            <DarkMode size="lg" />
        </div>
    </Navbar>
</header>
