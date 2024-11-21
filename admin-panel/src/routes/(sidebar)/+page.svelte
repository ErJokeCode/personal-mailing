<script lang="ts">
	import type { PageData } from './$types';
	import Dashboard from '../utils/dashboard/Dashboard.svelte';
	export let data: PageData;
</script>

<main class="p-4">
	<Dashboard {data} />
</main>
