<script lang="ts">
	export let when: string = '';
</script>

<div on:click
	class="flex gap-2 border-b px-4 py-3 hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-600"
>
	<div class="w-full pl-3">
		<div class="mb-1.5 text-sm font-normal text-gray-500 dark:text-gray-400">
			<slot />
		</div>
		{#if when}
			<div class="text-xs font-medium text-primary-700 dark:text-primary-400">{when}</div>
		{/if}
	</div>
</div>
