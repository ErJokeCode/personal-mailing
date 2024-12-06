<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { Button } from 'flowbite-svelte';
	import { ArrowRightOutline } from 'flowbite-svelte-icons';

	import { onMount } from "svelte";
    import { goto } from '$app/navigation';
    import http from "../../../utils/http";
    import { traverseObject } from "../../../utils/helper";
    import { page } from '$app/stores';

    let id = $page.params.id;

    let admin = {};
    let status = http.status();
    let article;

    onMount(async () => {
        admin = (await http.get(`/core/admin/${id}`, status)) ?? {};

        article.appendChild(traverseObject(admin));
    });
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem href="/admin">Администраторы</BreadcrumbItem>
			<BreadcrumbItem>Детали</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3">
			Email: {admin.email}
		</Heading>
        <div class="text-gray-900 dark:text-white" bind:this={article}></div>
	</div>
</main>