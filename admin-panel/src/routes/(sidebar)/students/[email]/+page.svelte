<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { List, Li } from 'flowbite-svelte';

	import { onMount } from "svelte";
    import http from "../../../../utility/http";
    import { traverseObject } from "../../../../utility/helper";
    import { page } from '$app/stores';

    let email = $page.params.email;

    let article;
    let student = {};
    let status = http.status();

    onMount(async () => {
        student =
            (await http.get("/parser/student/?email=" + email, status)) ?? {};

        article.appendChild(traverseObject(student));
    });
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem href="/students/all">Все студенты</BreadcrumbItem>
			<BreadcrumbItem>Детали</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3">
			Email: {student.email}
		</Heading>
        <List class="text-gray-900 dark:text-white">
            <div class="li" bind:this={article}></div>
        </List>
	</div>
</main>