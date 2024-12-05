<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';
	import { Button } from 'flowbite-svelte';
	import { List, Li } from 'flowbite-svelte';
	import { ArrowRightOutline } from 'flowbite-svelte-icons';

	import { onMount } from "svelte";
    import { goto } from '$app/navigation';
    import http from "../../../../utils/http";
    import { traverseObject } from "../../../../utils/helper";
    import { page } from '$app/stores';

    let id = $page.params.id;

    let student = {};
    let status = http.status();
    let article;

    onMount(async () => {
        student = await http.get(`/core/student/${id}`, status);
        article.appendChild(traverseObject(student));
    });

    async function start_chat(studentId) {
        goto(`/chat/${studentId}`);
    }
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem href="/students/active">Активные студенты</BreadcrumbItem>
			<BreadcrumbItem>Детали</BreadcrumbItem>
		</Breadcrumb>
        <Button class="mb-3"
            on:click={() => start_chat(student.id)}>
            Начать чат
            <ArrowRightOutline class="w-6 h-6 ms-2" />
        </Button>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-3">
			Email: {student.email}
		</Heading>
        <div class="text-gray-900 dark:text-white" bind:this={article}></div>
	</div>
</main>