<script lang="ts">
    import { me } from "./stores/me.svelte";
    import Header from "./common/Header.svelte";
    import Sidebar from "./common/Sidebar.svelte";
    import Main from "./common/Main.svelte";
</script>

<div class="h-full flex flex-col">
    <Header />

    <div class="h-full flex overflow-hidden">
        {#if me.value !== null}
            <Sidebar />
        {/if}

        <div class="h-full flex-col flex-1 overflow-scroll">
            <Main />
        </div>
    </div>
</div>
