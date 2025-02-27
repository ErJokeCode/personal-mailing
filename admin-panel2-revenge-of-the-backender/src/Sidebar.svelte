<script lang="ts">
    import { active, route } from "@mateothegreat/svelte5-router";
    import { Sidebar, SidebarWrapper } from "flowbite-svelte";
    import { CogOutline } from "flowbite-svelte-icons";
    import { type Component } from "svelte";
</script>

<Sidebar
    class="overflow-hidden border-r-gray-200 dark:border-r-gray-600 border-r h-screen">
    <SidebarWrapper class="h-full rounded-none list-none flex flex-col gap-2">
        {@render sidebarItem("Профиль", "/profile", CogOutline)}
    </SidebarWrapper>
</Sidebar>

{#snippet sidebarItem(label: string, href: string, Icon: Component)}
    <li>
        <a
            class="flex items-center p-2 text-base font-normal text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700"
            use:active={{
                active: { class: ["bg-gray-100", "dark:bg-gray-700"] },
            }}
            use:route
            {href}>
            <Icon class="w-6 h-6" />
            <span class="ms-3">{label}</span>
        </a>
    </li>
{/snippet}
