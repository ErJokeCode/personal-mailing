<script lang="ts">
	import {
		Breadcrumb,
		BreadcrumbItem,
		Button,
		Checkbox,
		Heading
	} from 'flowbite-svelte';
	import { Dropzone, Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
	import { TableHeadCell, Textarea, Helper } from 'flowbite-svelte';

	import { onMount } from "svelte";

    let login_status = "";
    let send_status = "";
    let content = "";
    let files = [];
    let ids = [];

    let activeStudents = [];

    onMount(async () => {
        let response;

        try {
            response = await fetch('http://localhost:5000/core/student', {
                credentials: "include",
            });
        } catch (err) {
            login_status = "Not Logged In";
        }

        let json = await response?.json();
        activeStudents = json;
    });

    function add_id(checked: any, id: any) {
        if (checked) {
            ids.push(id);
        } else {
            ids = ids.filter((fid) => fid != id);
        }
    }

    async function send() {
        if (ids.length === 0) return
        send_status = "Sending...";

        let response;

        try {
            let data = new FormData();

            let body = JSON.stringify({
                content: content,
                studentIds: ids,
            });

            data.append("body", body);
            for (let file of files) {
                data.append("file", file);
            }

            response = await fetch('http://localhost:5000/core/notification', {
                method: "Post",
                body: data,
                credentials: "include",
            });
        } catch (err) {
            send_status = "Something went wrong! " + err;
        }

        if (response?.ok) {
            send_status = "You successfully sent a notification!";
        }
    }
	
	
	let value: any[] = [];

  const dropHandle = (event) => {
    value = [];
    event.preventDefault();
    if (event.dataTransfer.items) {
      [...event.dataTransfer.items].forEach((item, i) => {
        if (item.kind === 'file') {
          const file = item.getAsFile();
          value.push(file.name);
          value = value;
        }
      });
    } else {
      [...event.dataTransfer.files].forEach((file, i) => {
        value = file.name;
      });
    }
  };

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

    if (concat.length > 200) {
        concat = concat.slice(0, 200);
        concat += '...';
    }
    return concat;
  };
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
			<BreadcrumbItem href="/notifications">Рассылки</BreadcrumbItem>
			<BreadcrumbItem>Отправить рассылку</BreadcrumbItem>
		</Breadcrumb>
		<form class="mb-4">
			<div class="items-center px-3 py-2 rounded-lg bg-gray-50 dark:bg-gray-700">
				<Dropzone
					class="mb-4"
					id="dropzone"
					on:drop={dropHandle}
					on:dragover={(event) => {
						event.preventDefault();
					}}
					on:change={handleChange}
					multiple bind:files>
					<svg aria-hidden="true" class="mb-3 w-10 h-10 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" /></svg>
					{#if value.length === 0}
						<p class="mb-2 text-sm text-gray-500 dark:text-gray-400"><span class="font-semibold">Click to upload</span> or drag and drop</p>
						<p class="text-xs text-gray-500 dark:text-gray-400">SVG, PNG, JPG or GIF (MAX. 800x400px)</p>
					{:else}
						<p>{showFiles(value)}</p>
					{/if}
				</Dropzone>
				<Textarea id="editor" rows="6" class="mb-2 bg-white dark:bg-gray-800" placeholder="Введите текст" bind:value={content}></Textarea>
				<Button class="mb-2" on:click={send}>Отправить рассылку</Button>
				<Helper>{send_status}</Helper>
			</div>
		</form>
		<Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Все студенты
      {login_status}
		</Heading>
	</div>
	<Table>
		<TableHead class="border-y border-gray-200 bg-gray-100 dark:border-gray-700">
			{#each ['Выбрать', 'Id', 'Электронная почта'] as title}
				<TableHeadCell class="w-4 p-4 px-6 font-medium">{title}</TableHeadCell>
			{/each}
		</TableHead>
		<TableBody>
			{#each activeStudents as student}
				<TableBodyRow class="text-base">
					<TableBodyCell class="w-4 p-4 px-10"><Checkbox on:click={(event) => add_id(event.target?.checked, student.id)}/></TableBodyCell>
					<TableBodyCell class="mr-12 px-10 flex items-center space-x-6 whitespace-nowrap p-4">
						<div class="text-sm font-normal text-gray-500 dark:text-gray-400">
							<div class="text-base font-semibold text-gray-900 dark:text-white">{student.id}</div>
							<div class="text-sm font-normal text-gray-500 dark:text-gray-400"></div>
						</div>
					</TableBodyCell>
					<TableBodyCell class="max-w-sm px-10 overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
						{student.email}
					</TableBodyCell>
				</TableBodyRow>
			{/each}
		</TableBody>
	</Table>
</main>