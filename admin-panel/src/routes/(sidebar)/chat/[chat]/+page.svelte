<script lang="ts">
	import {
		A,
		Breadcrumb,
		BreadcrumbItem,
		Button,
		Helper,
		Textarea,
		ToolbarButton
	} from 'flowbite-svelte';
	import { PaperClipOutline } from 'flowbite-svelte-icons';
	import { onDestroy, onMount } from 'svelte';
	import http from '../../../utils/http';
	import { signal } from '../../../utils/signal';
	import { page } from '$app/stores';

	let studentId = $page.params.chat;

	let content = '';
	let status = http.status();
	let messages = [];
	let chatArea;
	let student = {};
	let admin = {};

	let files = [];

	async function get_messages() {
		let json = (await http.get(`/core/chat/adminWith/${studentId}`, http.status())) ?? undefined;
		student = json?.student ?? {};
		admin = json?.admin ?? {};
		messages = json?.messages ?? [];
	}

	async function send_message() {
		if (content === '' && files.length === 0) return;
		status = status.start_load();

		var data = new FormData();
		data.append('body', JSON.stringify({ studentId, content }));
		for (let file of files) {
			data.append('file', file);
		}

		await http.post(`/core/chat/adminSend`, data, status);

		content = '';
		status = status.end_load();
		value = [];

		await get_messages();
		chatArea.scrollTo(0, chatArea.scrollHeight);
	}

	async function handle_keypress(event) {
		if (event.key == 'Enter') {
			send_message();
		}
	}

	onMount(async () => {
		await get_messages();

		chatArea.scrollTo(0, chatArea.scrollHeight);

		signal.on('StudentSentMessage', handle_student_message);
	});

	onDestroy(async () => {
		signal.off('StudentSentMessage', handle_student_message);
	});

	async function handle_student_message(message) {
		if (message.student.id == studentId) {
			await get_messages();

			chatArea.scrollTo(0, chatArea.scrollHeight);
		}
	}

	let value: any[] = [];

	const handleChange = (event) => {
		const files = event.target.files;
		if (files.length > 0) {
			for (let file of files) {
				value.push(file.name);
			}
			value = value;
		}
	};

	const showFiles = (files) => {
		let concat = '';
		files.map((file: string) => {
			concat += file;
			concat += ',';
			concat += ' ';
		});

		if (concat.length > 100) {
			concat = concat.slice(0, 100);
			concat += '...';
		}
		return concat;
	};
</script>

<main class="h-full w-full bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb>
			<BreadcrumbItem home>Главная</BreadcrumbItem>
			<BreadcrumbItem href="/chat">Все чаты</BreadcrumbItem>
			<BreadcrumbItem>Чат</BreadcrumbItem>
		</Breadcrumb>
	</div>
	<div class="mb-10 px-20 py-8">
		<div
			class="mb-5 flex w-full flex-col divide-gray-200 overflow-auto rounded-lg border
                    border-gray-200 bg-gray-50 p-4 text-gray-500 shadow-md sm:p-6 dark:divide-gray-700 dark:border-gray-700 dark:bg-gray-900 dark:text-gray-400"
			bind:this={chatArea}
		>
			<div class="h-s">
				{#each messages as message}
					{#if message.sender == 'Admin'}
						<article class="flex justify-end space-y-3">
							<div
								class="mb-5 flex w-full max-w-xl flex-col divide-gray-200 rounded-lg border
                                        border-gray-200 bg-white p-4 text-gray-500 shadow-md sm:px-5 dark:divide-gray-700 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400"
							>
								<p class="text-sm font-semibold text-gray-900 dark:text-white">
									{admin.email}
								</p>
								<div class="mt-1 flex space-y-2 break-all text-gray-900 dark:text-white">
									{message.content}
								</div>
								{#each message.documents as document}
									<hr class="my-1 h-px border-0 bg-gray-200 dark:bg-gray-700" />
									{#if document.mimeType.includes('image')}
										<img
											class="mb-1"
											src={`http://localhost:5000/core/document/${document.id}/download`}
											alt=""
										/>
									{/if}
									<div class="flex items-center">
										<Helper>{document.name}</Helper>
										<a
											class="ml-3"
											href={`http://localhost:5000/core/document/${document.id}/download`}
											>Скачать</a
										>
									</div>
								{/each}
							</div>
						</article>
					{:else}
						<article class="flex space-y-3">
							<div
								class="mb-5 flex w-full max-w-xl flex-col divide-gray-200 rounded-lg border
                                        border-gray-200 bg-white p-4 text-gray-500 shadow-md sm:px-5 dark:divide-gray-700 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400"
							>
								<p class="text-sm font-semibold text-gray-900 dark:text-white">
									{student.email}
								</p>
								<div class="mt-1 flex space-y-2 break-all text-gray-900 dark:text-white">
									{message.content}
								</div>
								{#each message.documents as document}
									<hr class="my-1 h-px border-0 bg-gray-200 dark:bg-gray-700" />
									{#if document.mimeType.includes('image')}
										<img
											class="mb-1"
											src={`http://localhost:5000/core/document/${document.id}/download`}
											alt=""
										/>
									{/if}
									<div class="flex items-center">
										<Helper>{document.name}</Helper>
										<a
											class="ml-3"
											href={`http://localhost:5000/core/document/${document.id}/download`}
											>Скачать</a
										>
									</div>
								{/each}
							</div>
						</article>
					{/if}
				{/each}
			</div>
		</div>
		<div class="flex items-center gap-3">
			<ToolbarButton name="Attach file" class="relative">
				<PaperClipOutline class="h-6 w-6 rotate-45 dark:text-white"></PaperClipOutline>
				<input
					class="absolute left-0 top-0 h-10 w-10 rounded-lg opacity-0"
					type="file"
					multiple
					bind:files
					on:change={handleChange}
				/>
			</ToolbarButton>
			<Textarea
				rows={1}
				placeholder="Введите текст"
				bind:value={content}
				on:keypress={handle_keypress}
				class="bg-gray-50 dark:bg-gray-900"
			/>
			<ToolbarButton
				type="submit"
				color="default"
				class="p-2 text-primary-600 hover:bg-primary-100"
			>
				<svg
					on:click={send_message}
					aria-hidden="true"
					class="h-6 w-6 rotate-90"
					fill="currentColor"
					viewBox="0 0 20 20"
					><path
						d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z"
					></path></svg
				>
				<span class="sr-only">Send message</span>
			</ToolbarButton>
		</div>
		<Helper class="mt-1">{showFiles(value)}</Helper>
	</div>
</main>

<style>
	.h-s {
		height: 60dvh;
	}
</style>

