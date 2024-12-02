<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading} from 'flowbite-svelte';
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home>Главная</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Главная
        </Heading>
	</div>
</main>