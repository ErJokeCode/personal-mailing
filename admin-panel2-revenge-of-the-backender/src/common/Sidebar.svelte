<script lang="ts">
    import { active, route } from "@mateothegreat/svelte5-router";
    import {
        Sidebar,
        SidebarDropdownItem,
        SidebarDropdownWrapper,
        SidebarWrapper,
    } from "flowbite-svelte";
    import {
        ClipboardListSolid,
        CogOutline,
        EditOutline,
        FileOutline,
        QuestionCircleOutline,
        TableRowOutline,
        UserSettingsOutline,
        UsersGroupOutline,
        UsersOutline,
    } from "flowbite-svelte-icons";
    import { type Component } from "svelte";
    import { RouteHistory } from "/src/stores/RouteHistory.svelte";

    function resetHistory() {
        RouteHistory.isClear = true;
    }
</script>

<!-- TODO Remake this with dropdown menus -->

<Sidebar
    class="overflow-hidden border-r-gray-200 dark:border-r-gray-600 border-r h-screen">
    <SidebarWrapper class="h-full rounded-none list-none flex flex-col gap-2">
        {@render sidebarItem("Профиль", "/profile", CogOutline)}
        {@render sidebarItem("Админы", "/admins", UserSettingsOutline)}
        {@render sidebarItem("Группы", "/groups", UsersGroupOutline)}
        {@render sidebarItem("Рассылки", "/notifications", ClipboardListSolid)}

        <SidebarDropdownWrapper label="Студенты">
            <svelte:component this={UsersOutline} slot="icon" />
            {@render sidebarItem("Все студенты", "/students", null, "ps-12")}
            {@render sidebarItem(
                "Активные студенты",
                "/active-students",
                null,
                "ps-12",
            )}
        </SidebarDropdownWrapper>

        {@render sidebarItem("Загрузить файлы", "/upload", FileOutline)}
        {@render sidebarItem("Соотношения", "/subjects", TableRowOutline)}
        {@render sidebarItem("Редактор FAQ", "/faq", QuestionCircleOutline)}
        {@render sidebarItem("Конструктор онбординга", "/builder", EditOutline)}
    </SidebarWrapper>
</Sidebar>

{#snippet sidebarItem(
    label: string,
    href: string,
    Icon?: Component,
    aClass?: string,
)}
    <li>
        <a
            class={"flex items-center p-2 text-base font-normal text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700" +
                " " +
                aClass}
            use:active={{
                active: { class: ["bg-gray-100", "dark:bg-gray-700"] },
            }}
            use:route
            on:click={resetHistory}
            {href}>
            {#if Icon}
                <Icon class="w-6 h-6" />
            {/if}
            <span class="ms-3">{label}</span>
        </a>
    </li>
{/snippet}
