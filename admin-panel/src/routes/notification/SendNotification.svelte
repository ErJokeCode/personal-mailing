<script lang="ts">
	import {
		Breadcrumb,
		BreadcrumbItem,
		Button,
		Checkbox,
		Heading,
        Label,
        Input
	} from 'flowbite-svelte';
	import { Dropzone, Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
	import { TableHeadCell, Textarea, Helper } from 'flowbite-svelte';
    import { Link } from "svelte-routing";
	import { onMount } from "svelte";

    let send_status = "";
    let content = "";
    let files: FileList;
    let ids = [];

    let activeStudents = [];

    onMount(async () => {
        load_students();
    });

    const load_students = async () => {
        let response;
        let url = new URL('http://localhost:5000/core/student?')
        url.searchParams.append('notOnCourse', notOnCourse);
        url.searchParams.append('lowScore', lowScore);
        if (course !== 'Выберите курс') {
            url.searchParams.append('course', course);
        }
        if (group !== '') {
            url.searchParams.append('group', group);
        }
        if (typeOfEducation !== 'Выберите форму') {
            url.searchParams.append('typeOfEducation', typeOfEducation);
        }
        if (typeOfCost !== 'Выберите тип') {
            url.searchParams.append('typeOfCost', typeOfCost);
        }
        if (onlineCourse !== '') {
            url.searchParams.append('onlineCourse', onlineCourse);
        }
        if (subject !== '') {
            url.searchParams.append('subject', subject);
        }
        if (team !== '') {
            url.searchParams.append('team', team);
        }

        try {
            response = await fetch(url, {
                credentials: "include",
            });
        } catch (err) {
            console.log("Not Logged In");
        }

        let json = await response?.json();
        activeStudents = json.items;
    }

    function add_id(checked: any, id: any) {
        if (checked) {
            if (ids.includes(id)) return;
            ids.push(id);
        } else {
            ids = ids.filter((fid) => fid != id);
        }
    }

    async function send() {
        let table = document.querySelector('#table')
        let checkboxes = table.querySelectorAll('input[type=checkbox]')
        for (let i = 1; i < checkboxes.length; i++) {
            add_id(checkboxes[i].checked, checkboxes[i].value);
        }
        if (ids.length === 0) return;
        send_status = "Sending...";

        let response;

        try {
            let data = new FormData();

            let body = JSON.stringify({
                content: content,
                studentIds: ids,
            });

            data.append("body", body);
            if (files !== undefined) {
                for (let file of files) {
                    data.append("file", file);
                }
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

  function toggle(source) {
        let table = document.querySelector('#table')
        let checkboxes = table.querySelectorAll('input[type=checkbox]')
        for (let i = 0; i < checkboxes.length; i++) {
            checkboxes[i].checked = source.checked;
        }
    }
    
    function remove() {
        let table = document.querySelector('#table')
        let checkboxes = table.querySelectorAll('input[type=checkbox]')
        let counter = 0;
        for (let i = 1; i < checkboxes.length; i++) {
            if (!checkboxes[i].checked) {
                checkboxes[0].checked = false;
            } else {
                counter++;
            }
        }
        if (counter === checkboxes.length - 1) checkboxes[0].checked = true;
    }

    let pageIndex = 0;
    let pageSize = 10;
    let notOnCourse = false;
    let lowScore = false;
    let course = 'Выберите курс';
    let group = '';
    let typeOfEducation = 'Выберите форму';
    let typeOfCost = 'Выберите тип';
    let onlineCourse = '';
    let subject = '';
    let team = '';
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="p-4 px-6">
                <Breadcrumb class="mb-5">
                    <li class="inline-flex items-center"><Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link></li>
                    <li class="inline-flex items-center"><svg class="w-6 h-6 text-gray-400 rtl:-scale-x-100" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0
                        z" clip-rule="evenodd"></path></svg>
                    <Link class="ml-0 ms-1 text-sm font-medium text-gray-700 hover:text-gray-900 md:ms-2 dark:text-gray-400 dark:hover:text-white" to="/notifications">Рассылки</Link></li>
                    <BreadcrumbItem>Отправить рассылку</BreadcrumbItem>
                </Breadcrumb>
                <div class="mb-4 flex">
                    <form class="w-full">
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
                                    <p class="text-xs text-gray-500 dark:text-gray-400">{showFiles(value)}</p>
                                {/if}
                            </Dropzone>
                            <Textarea id="editor" rows=6 class="mb-2 bg-white dark:bg-gray-800" placeholder="Введите текст" bind:value={content}></Textarea>
                            <Button class="mb-2" on:click={send}>Отправить рассылку</Button>
                            <Helper>{send_status}</Helper>
                        </div>
                    </form>
                    <div class="w-1/2 ml-6">
                        <Heading tag="h1" class="mb-4 text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
                            Фильтры
                        </Heading>
                        <div class="ml-2 flex">
                            <Checkbox class="mb-4 mr-5 w-fit" bind:checked={notOnCourse}>Нет на курсе</Checkbox>
                            <Checkbox class='mb-4 w-fit' bind:checked={lowScore}>Низкий балл</Checkbox>
                        </div>
                        <div class="ml-2 mb-4 flex">
                            <div class="mr-6 w-full space-y-4">
                                <form>
                                    <Label class="space-y-2 mb-2">Номер курса</Label>
                                    <select bind:value={course} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                                        <option selected>Выберите курс</option>
                                        <option value=1>1</option>
                                        <option value=2>2</option>
                                        <option value=3>3</option>
                                        <option value=4>4</option>
                                        <option value=5>5</option>
                                        <option value=6>6</option>
                                    </select>
                                </form>
                                <form>
                                    <Label class="space-y-2 mb-2">Тип затрат</Label>
                                    <select bind:value={typeOfCost} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                                        <option selected>Выберите тип</option>
                                        <option value='бюджет'>Бюджет</option>
                                        <option value='контракт'>Контракт</option>
                                    </select>
                                </form>
                                <form>
                                    <Label class="space-y-2 mb-2">Форма обучения</Label>
                                    <select bind:value={typeOfEducation} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                                        <option selected>Выберите форму</option>
                                        <option value='Очная'>Очная</option>
                                        <option value='Очно-заочная'>Очно-заочная</option>
                                        <option value='Заочная'>Заочная</option>
                                    </select>
                                </form>
                            </div>
                            <div class="w-full space-y-4">
                                <Label class="space-y-2">
                                    <span>Группа</span>
                                    <Input bind:value={group} type="text" placeholder="РИ-123456" size="md" />
                                </Label>
                                <Label class="space-y-2">
                                    <span>Онлайн курс</span>
                                    <Input bind:value={onlineCourse} type="text" placeholder="Введите название курса" size="md" />
                                </Label>
                                <Label class="space-y-2">
                                    <span>Предмет</span>
                                    <Input bind:value={subject} type="text" placeholder="Введите название предмета" size="md" />
                                </Label>
                            </div>
                        </div>
                        <Label class="space-y-2 ml-2 mb-6">
                            <span>Команда</span>
                            <Input bind:value={team} type="text" placeholder="Введите название команды" size="md" />
                        </Label>
                        <Button class='flex ml-2' on:click={load_students}>Применить</Button>
                    </div>
                </div>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
                    Все студенты
                </Heading>
            </div>
            <Table id='table'>
                <TableHead class="border-y border-gray-200 bg-gray-100 dark:border-gray-700">
                    <TableHeadCell class="w-4 p-4 pl-8 font-medium"><Checkbox on:click={(e) => toggle(e.target)} /></TableHeadCell>
                    <TableHeadCell class="w-4 p-4 font-medium">Курс</TableHeadCell>
                    <TableHeadCell class="w-4 p-4 font-medium">Имя</TableHeadCell>
                    <TableHeadCell class="w-4 p-4 font-medium">Фамилия</TableHeadCell>
                    <TableHeadCell class="w-4 p-4 font-medium">Отчество</TableHeadCell>
                    <TableHeadCell class="w-1/12 p-4 font-medium">Группа</TableHeadCell>
                    <TableHeadCell class="w-1/6 p-4 font-medium">Направление</TableHeadCell>
                    <TableHeadCell class="w-1/12 p-4 font-medium">Форма</TableHeadCell>
                    <TableHeadCell class="w-1/12 p-4 font-medium">Тип</TableHeadCell>
                    <TableHeadCell class="w-4 p-4 font-medium">Электронная почта</TableHeadCell>
                </TableHead>
                <TableBody>
                    {#each activeStudents as student}
                        <TableBodyRow class="text-base">
                            <TableBodyCell class="p-4 pl-8"><Checkbox value={student.id} on:click={remove} /></TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.info.group.numberCourse}
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.info.name}
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.info.surname}
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.info.patronymic}
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.info.group.number}
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm flex items-center space-x-6 whitespace-nowrap p-4">
                                <div class="text-sm font-normal text-gray-500 dark:text-gray-400">
                                    <div class="text-base font-semibold text-gray-900 dark:text-white">{student.info.group.directionCode}</div>
                                    <div class="text-sm font-normal text-gray-500 dark:text-gray-400">{student.info.group.nameSpeciality}</div>
                                </div>
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.info.typeOfEducation}
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.info.typeOfCost}
                            </TableBodyCell>
                            <TableBodyCell class="max-w-sm overflow-hidden truncate p-4 text-base font-normal text-gray-500 dark:text-gray-400 xl:max-w-xs">
                                {student.email}
                            </TableBodyCell>
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
        </div>
    </div>
</div>
