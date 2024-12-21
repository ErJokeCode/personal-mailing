<script>
    import { Router } from "svelte-routing";
    import "./app.css";
    import Sidebar from './Sidebar.svelte'
    import Navbar from './Navbar.svelte'
    import Main from './Main.svelte'

    export let url = "";

	let drawerHidden = false;
</script>

<Router {url}>
	<Navbar bind:drawerHidden />
	<Sidebar bind:drawerHidden />
    <Main />
</Router>
