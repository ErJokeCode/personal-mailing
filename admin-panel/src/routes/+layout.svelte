<script>
	import modeobserver from './utils/modeobserver';
	import { onMount } from 'svelte';
    import { admin } from "./utils/store";
    import http from "./utils/http";
	import { goto } from '$app/navigation';

	onMount(modeobserver);

    onMount(async () => {
        let the_admin = await http.get("/core/admin/me", http.status());
        if (the_admin) {
            admin.update((_) => the_admin);
        } else {
            goto('/login')
        }
    })
</script>
<slot />
