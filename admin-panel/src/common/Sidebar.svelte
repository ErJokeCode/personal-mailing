<script lang="ts">
    import { active, route } from "@mateothegreat/svelte5-router";
    import {
        Sidebar,
        SidebarDropdownWrapper,
        SidebarWrapper,
    } from "flowbite-svelte";
    import {
        ClipboardListSolid,
        CogOutline,
        EditOutline,
        FileOutline,
        MessagesOutline,
        QuestionCircleOutline,
        TableRowOutline,
        UserSettingsOutline,
        UsersGroupOutline,
        UsersOutline,
        ChartPieOutline,
    } from "flowbite-svelte-icons";
    import { type Component } from "svelte";
    import { RouteHistory } from "/src/stores/RouteHistory.svelte";

    function resetHistory() {
        RouteHistory.isClear = true;
    }

	export let drawerHidden = false;

    let iconClass = "flex-shrink-0 w-6 h-6 text-gray-500 transition duration-75 group-hover:text-gray-900 dark:text-gray-400 dark:group-hover:text-white"
    let itemClass = "flex items-center p-2 text-base text-gray-900 transition duration-75 rounded-lg hover:bg-gray-100 group dark:text-gray-200 dark:hover:bg-gray-700"
</script>

<Sidebar
    class={drawerHidden ? 'fixed' : 'hidden'}
    asideClass="z-50 flex-none h-full w-64 lg:h-auto border-e border-gray-200 dark:border-gray-600 lg:overflow-y-visible lg:block">
    <SidebarWrapper class="bg-white scrolling-touch h-full rounded-none list-none flex flex-col gap-2">
        {@render sidebarItem("Профиль", "/profile", CogOutline)}
        {@render sidebarItem("Админы", "/admins", UserSettingsOutline)}
        {@render sidebarItem("Группы", "/groups", UsersGroupOutline)}
        {@render sidebarItem("Рассылки", "/notifications", ClipboardListSolid)}
        {@render sidebarItem("Чаты", "/chats", MessagesOutline)}

        <SidebarDropdownWrapper label="Студенты">
            <svelte:component this={UsersOutline} slot="icon" class={iconClass} />
            {@render sidebarItem("Все студенты", "/students", null, "ps-8")}
            {@render sidebarItem("Активные студенты", "/active-students", null, "ps-8")}
        </SidebarDropdownWrapper>

        {@render sidebarItem("Загрузить файлы", "/upload", FileOutline)}
        {@render sidebarItem("Соотношения", "/subjects", TableRowOutline)}
        {@render sidebarItem("Редактор FAQ", "/faq", QuestionCircleOutline)}
        {@render sidebarItem("Конструктор онбординга", "/builder", EditOutline)}
        {@render sidebarItem("База знаний", "/base", ChartPieOutline)}
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
            class={ `${itemClass} ${aClass}`}
            use:active={{
                active: { class: ["bg-gray-100", "dark:bg-gray-700"] },
            }}
            use:route
            on:click={resetHistory}
            {href}>
            {#if Icon}
                <Icon class={iconClass} />
            {/if}
            <span class="ms-3">{label}</span>
        </a>
    </li>
{/snippet}
