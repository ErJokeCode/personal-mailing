<script>
    import { Link } from "svelte-routing";
    import { isEmptyObject } from "src/utility/helper";
    import { admin } from "src/utility/store";
</script>

<header class="pico-background-slate-50">
    <nav class="container">
        <ul></ul>
        <ul>
            {#if !isEmptyObject($admin)}
                <li>
                    <Link class="contrast" to="/me">{$admin.email}</Link>
                </li>
            {:else}
                <li><Link to="/login">Логин</Link></li>
            {/if}
        </ul>
    </nav>
</header>

<style>
</style>
