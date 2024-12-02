<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading, List } from 'flowbite-svelte';

    import http from "../../../../utility/http";
    import { onMount } from "svelte";
    import { traverseObject } from "../../../../utility/helper";
    import { page } from '$app/stores';

    let id = $page.params.id;

    let notification = {};
    let status = http.status();
    let article;

    onMount(async () => {
        notification =
            (await http.get(`/core/notification/${id}`, status)) ?? {};

        article.appendChild(traverseObject(notification));
    });
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem href="/notifications">Рассылки</BreadcrumbItem>
			<BreadcrumbItem>Детали</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3">
			{notification.date}
		</Heading>
        <List class="text-gray-900 dark:text-white">
            <div bind:this={article}></div>
        </List>
	</div>
</main>