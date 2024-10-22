<script>
    import { Router, Link, Route } from "svelte-routing";
    import Dashboard from "./Dashboard.svelte";
    import Login from "./Login.svelte";

    export let url = "";
</script>

<Router {url}>
    <nav>
        <Link to="/login">Login</Link>
        <Link to="/dashboard">Dashboard</Link>
    </nav>

    <div>
        <Route path="/login" component={Login} />
        <Route path="/dashboard" component={Dashboard} />
    </div>
</Router>
