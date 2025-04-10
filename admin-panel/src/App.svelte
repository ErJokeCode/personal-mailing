<script lang="ts">
    import { Me } from "./stores/Me.svelte";
    import Header from "./common/Header.svelte";
    import Sidebar from "./common/Sidebar.svelte";
    import Main from "./common/Main.svelte";

	let drawerHidden = false;
</script>

<div class="h-full flex flex-col">
    <Header bind:drawerHidden />

    <div class="h-full flex overflow-hidden">
        {#if Me.value !== null}
            <Sidebar bind:drawerHidden />
        {/if}

        <div class="h-full flex-col flex-1 overflow-auto overflow-x-hidden">
            <Main />
            {#if drawerHidden}
                <div class="fixed inset-0 z-20 bg-gray-900/50 dark:bg-gray-900/60"></div>
            {/if}
        </div>
    </div>
</div>
