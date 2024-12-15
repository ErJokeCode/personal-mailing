<script>
    import { navigate, Router } from "svelte-routing";
    import "./app.css";
    import Sidebar from './Sidebar.svelte'
    import Navbar from './Navbar.svelte'
    import Main from './Main.svelte'
    import { admin } from "./utils/store";

    export let url = "";

	let drawerHidden = false;
    if ($admin.email === undefined) navigate('/login')
</script>

<Router {url}>
	<Navbar bind:drawerHidden />
	<Sidebar bind:drawerHidden />
    <Main />
</Router>
