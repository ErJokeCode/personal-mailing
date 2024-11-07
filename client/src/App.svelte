<script>
    import { Router } from "svelte-routing";

    import Header from "./Header.svelte";
    import Aside from "./Aside.svelte";
    import Main from "./Main.svelte";

    export let url = "";
</script>

<Router {url}>
    <div class="app-wrapper">
        <Header />

        <div class="main-wrapper">
            <Aside />
            <Main />
        </div>
    </div>
</Router>

<style>
    .app-wrapper {
        display: flex;
        flex-flow: column;
        height: 100%;
    }

    .main-wrapper {
        flex: 1;
        height: 100%;
        display: flex;
        flex-flow: row;
        overflow: scroll;
    }
</style>
