<script lang="ts">
	import {
		Sidebar,
		SidebarDropdownWrapper,
		SidebarGroup,
		SidebarWrapper
	} from 'flowbite-svelte';
	import {
		AngleDownOutline,
		AngleUpOutline,
		ClipboardListSolid,
		CogOutline,
		FileChartBarSolid,
		UserSettingsOutline,
		UsersGroupOutline,
		ChartPieOutline,
		RectangleListSolid,
        EditOutline
	} from 'flowbite-svelte-icons';
    import { Link } from 'svelte-routing';
    import { useLocation } from "svelte-routing";

	export let drawerHidden: boolean = false;

	let iconClass =
		'flex-shrink-0 w-6 h-6 text-gray-500 transition duration-75 group-hover:text-gray-900 dark:text-gray-400 dark:group-hover:text-white';
	let itemClass =
		'flex items-center p-2 text-base text-gray-900 transition duration-75 rounded-lg hover:bg-gray-100 group dark:text-gray-200 dark:hover:bg-gray-700';
	let activeClass =
		'bg-gray-100 dark:bg-gray-700 flex items-center p-2 text-base text-gray-900 transition duration-75 rounded-lg hover:bg-gray-100 group dark:text-gray-200 dark:hover:bg-gray-700';
	let groupClass = 'pt-2 space-y-2';
  
    $: location = useLocation();

	let posts = [
		{ name: 'Главная', icon: ChartPieOutline, href: '/' },
		{ name: 'Чаты', icon: RectangleListSolid, href: '/chats' },
		{ name: 'Загрузить файлы', icon: FileChartBarSolid, href: '/upload' },
		{ name: 'Рассылки', icon: ClipboardListSolid, href: '/notifications' },
		{
			name: 'Студенты',
			icon: UsersGroupOutline,
			children: {
				'Все студенты': '/students/all',
                'Активные студенты': '/students/active'
			}
		},
		{ name: 'Конструктор онбординга', icon: EditOutline, href: '/builder' },
		{ name: 'Администраторы', icon: UserSettingsOutline, href: '/admin' },
		{ name: 'Профиль', icon: CogOutline, href: '/profile' }
	];
	let dropdowns = Object.fromEntries(Object.keys(posts).map((x) => [x, false]));

    function isActive(href) {
        if (href === $location.pathname) return activeClass
        return itemClass
    }
</script>

{#if $location.pathname !== '/login'}
<div class="overflow-hidden lg:flex">
    <Sidebar
        class={drawerHidden ? '' : 'hidden'}
        asideClass="fixed inset-0 z-30 flex-none h-full w-64 lg:h-auto border-e border-gray-200 dark:border-gray-600 lg:overflow-y-visible lg:pt-16 lg:block"
        activeUrl={$location.pathname}
    >
        <h4 class="sr-only">Main menu</h4>
        <SidebarWrapper
            divClass="overflow-y-auto px-2 pt-20 lg:pt-5 h-full bg-white scrolling-touch max-w-2xs lg:h-[calc(100vh-4rem)] lg:block dark:bg-gray-800 lg:me-0 lg:sticky top-2"
        >
            <nav class="divide-y divide-gray-200 dark:divide-gray-700">
                <SidebarGroup ulClass={groupClass} class="mb-3">
                    {#each posts as { name, icon, children, href } (name)}
                        {#if children}
                            <SidebarDropdownWrapper bind:isOpen={dropdowns[name]} label={name} class="pr-3">
                                <AngleDownOutline slot="arrowdown" strokeWidth="3.3" size="sm" />
                                <AngleUpOutline slot="arrowup" strokeWidth="3.3" size="sm" />
                                <svelte:component this={icon} slot="icon" class={iconClass} />
    
                                {#each Object.entries(children) as [title, href]}
                                    <li><Link class={isActive(href)} to={href}><span class="ml-9">{title}</span></Link></li>
                                {/each}
                            </SidebarDropdownWrapper>
                        {:else}
                            <li><Link class={isActive(href)} to={href}><svelte:component this={icon} class={iconClass} /><span class="ml-3">{name}</span></Link></li>
                        {/if}
                    {/each}
                </SidebarGroup>
                <SidebarGroup ulClass={groupClass}>
                </SidebarGroup>
            </nav>
        </SidebarWrapper>
    </Sidebar>
</div>
{/if}