<script>
	import Notifications from './lib/notifications/NotificationList.svelte';
	import {
		DarkMode,
		Dropdown,
		DropdownItem,
		NavHamburger,
		NavLi,
		NavUl,
		Navbar,
        ToolbarButton
	} from 'flowbite-svelte';
	import { ChevronDownOutline, ArrowRightToBracketOutline } from 'flowbite-svelte-icons';
	import { Link, navigate, useLocation } from 'svelte-routing';

	export let fluid = true;
	export let drawerHidden = false;
	export let list = false;

    $: location = useLocation();
</script>

{#if $location.pathname !== '/login'}
<header class="fixed top-0 z-40 mx-auto w-full flex-none border-b border-gray-200 bg-white dark:border-gray-600 dark:bg-gray-800">
    <Navbar {fluid} class="text-black" color="default">
        <NavHamburger
            onClick={() => (drawerHidden = !drawerHidden)}
            class="m-0 me-3 md:block lg:hidden"
        />
        <Link to="/" class={list ? 'w-40' : 'lg:w-60'}>
            <span
                class="ml-px self-center whitespace-nowrap text-xl font-semibold dark:text-white sm:text-2xl px-5"
            >
                Админ панель
            </span>
        </Link>
        <div class="hidden lg:block lg:ps-3">
            {#if list}
                <NavUl class="ml-2" activeUrl="/" activeClass="text-primary-600 dark:text-primary-500">
                    <NavLi href="/">Home</NavLi>
                    <NavLi href="#top">Messages</NavLi>
                    <NavLi href="#top">Profile</NavLi>
                    <NavLi href="#top">Settings</NavLi>
                    <NavLi class="cursor-pointer">
                        Dropdown
                        <ChevronDownOutline  class="ms-0 inline" />
                    </NavLi>
                    <Dropdown class="z-20 w-44">
                        <DropdownItem href="#top">Item 1</DropdownItem>
                        <DropdownItem href="#top">Item 2</DropdownItem>
                        <DropdownItem href="#top">Item 3</DropdownItem>
                    </Dropdown>
                </NavUl>
            {/if}
        </div>
        <div class="ms-auto flex items-center text-gray-500 dark:text-gray-400 sm:order-2">
            <Notifications />
            <ToolbarButton size="lg" class="-mx-0.5 hover:text-gray-900 dark:hover:text-white" on:click={() => navigate('/login')}>
                <ArrowRightToBracketOutline size="lg" />
            </ToolbarButton>
            <DarkMode />
        </div>
    </Navbar>
</header>
{/if}
