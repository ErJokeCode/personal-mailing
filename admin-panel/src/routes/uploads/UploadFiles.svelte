<script lang="ts">
    import {
        Fileupload,
        Button,
        Helper,
        Heading,
        A,
        TableHeadCell,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
    } from "flowbite-svelte";
    import {
        ChevronLeftOutline,
        ChevronRightOutline,
    } from "flowbite-svelte-icons";
    import { onDestroy, onMount } from "svelte";
    import http from "/src/lib/utils/http";
    import { ServerUrl } from "/src/lib/server";
    import { signal } from "/src/lib/utils/signal";

    let student_files;
    let student_success = "";
    let modeus_files;
    let modeus_success = "";
    let courses_files;
    let courses_success = "";

    let status = http.status();
    let history = [];
    let limit = -1;

    let maxPage = 0;
    let curPage = 0;
    let amountPage = 10;
    let select;

    onMount(async () => {
        status = status.start_load();
        history =
            (await http.get(
                "/parser/upload/history?limit=" + limit.toString(),
                status,
            )) ?? [];
        status = status.end_load();
        if (history.length !== 0) {
            curPage = 1;
        }
        maxPage = Math.ceil(history.length / amountPage);

        signal.on("FileUploaded", handleUploadDone);
    });

    onDestroy(async () => {
        signal.off("FileUploaded", handleUploadDone);
    });

    async function handleUploadDone(message) {
        status = status.start_load();
        history =
            (await http.get(
                "/parser/upload/history?limit=" + limit.toString(),
                status,
            )) ?? [];
        status = status.end_load();
        if (history.length !== 0) {
            curPage = 1;
        }
        maxPage = Math.ceil(history.length / amountPage);
    }

    async function send_students() {
        if (student_files.length <= 0) return;

        student_success = "Отправка...";

        let file = student_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch(`${ServerUrl}/parser/upload/student`, {
            method: "POST",
            body: data,
            credentials: "include",
        });

        if (result.ok) {
            student_success = "Загружено";
            status = status.start_load();
            history =
                (await http.get(
                    "/parser/upload/history?limit=" + limit.toString(),
                    status,
                )) ?? [];
            status = status.end_load();
            if (history.length !== 0) {
                curPage = 1;
            }
            maxPage = Math.ceil(history.length / amountPage);
        } else {
            student_success = "Ошибка";
        }
    }

    async function send_modeus() {
        if (modeus_files.length <= 0) return;

        modeus_success = "Отправка...";

        let file = modeus_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch(
            `${ServerUrl}/parser/upload/choice_in_modeus`,
            {
                method: "POST",
                body: data,
                credentials: "include",
            },
        );

        if (result.ok) {
            modeus_success = "Загружено";
            status = status.start_load();
            history =
                (await http.get(
                    "/parser/upload/history?limit=" + limit.toString(),
                    status,
                )) ?? [];
            status = status.end_load();
            if (history.length !== 0) {
                curPage = 1;
            }
            maxPage = Math.ceil(history.length / amountPage);
        } else {
            modeus_success = "Ошибка";
        }
    }

    async function send_courses() {
        if (courses_files.length <= 0) return;

        courses_success = "Отправка...";

        let file = courses_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch(
            `${ServerUrl}/parser/upload/report_online_course`,
            {
                method: "POST",
                body: data,
                credentials: "include",
            },
        );

        if (result.ok) {
            courses_success = "Загружено";
            status = status.start_load();
            history =
                (await http.get(
                    "/parser/upload/history?limit=" + limit.toString(),
                    status,
                )) ?? [];
            status = status.end_load();
            if (history.length !== 0) {
                curPage = 1;
            }
            maxPage = Math.ceil(history.length / amountPage);
        } else {
            courses_success = "Ошибка";
        }
    }

    async function toPage(page) {
        if (page < 0) {
            page = 0;
        } else if (page < 1) {
            page = 1;
        } else if (page > maxPage) {
            page = maxPage;
        }

        select.value = page;
        curPage = page;
    }

    async function update_inf() {
        let result = await fetch(
            `${ServerUrl}/parser/upload/report_online_course/site_inf`,
            {
                method: "POST",
                credentials: "include",
            },
        );

        if (result.ok) {
            courses_success = "Загружено";
        } else {
            courses_success = "Ошибка";
        }
    }
</script>

<div class="p-4 px-6">
    <div class="space-y-2 mb-6">
        <div
            style="display: flex; flex-direction: row; gap: 12px; align-items: baseline; margin-bottom: 20px">
            <Heading
                tag="h3"
                class="max-w-fit text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-s"
                >Студенты</Heading>
            <A
                class="font-medium hover:underline text-sm"
                href={ServerUrl + "/parser/upload/student/example"}
                >Скачать пример</A>
        </div>
        <Fileupload
            value=""
            bind:files={student_files}
            id="larg_size"
            size="lg" />
        <Helper>{student_success}</Helper>
        <Button on:click={send_students}>Загрузить</Button>
    </div>
    <div class="space-y-2 mb-6">
        <div
            style="display: flex; flex-direction: row; gap: 30px; align-items: baseline; margin-bottom: 20px">
            <Heading
                tag="h3"
                class="max-w-fit text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-l"
                >Модеус</Heading>
            <A
                class="font-medium hover:underline text-sm"
                href={ServerUrl + "/parser/upload/choice_in_modeus/example"}
                >Скачать пример</A>
        </div>
        <Fileupload
            value=""
            bind:files={modeus_files}
            id="larg_size"
            size="lg" />
        <Helper>{modeus_success}</Helper>
        <Button on:click={send_modeus}>Загрузить</Button>
    </div>
    <div class="space-y-2 mb-6">
        <div
            style="display: flex; flex-direction: row; gap: 30px; align-items: baseline; margin-bottom: 20px">
            <Heading
                tag="h3"
                class="max-w-fit text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-l"
                >Курсы</Heading>
            <A
                class="font-medium hover:underline text-sm"
                href={ServerUrl + "/parser/upload/report_online_course/example"}
                >Скачать пример</A>
            <A class="font-medium hover:underline text-sm" on:click={update_inf}
                >Обновить информацию с сайта inf-urfu</A>
        </div>
        <Fileupload
            value=""
            bind:files={courses_files}
            id="larg_size"
            size="lg" />
        <Helper>{courses_success}</Helper>
        <Button on:click={send_courses}>Загрузить</Button>
    </div>
    <Heading
        tag="h2"
        class="text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-2xl mb-5">
        История
    </Heading>
    <div class="flex space-x-2">
        <Button on:click={() => toPage(curPage - 1)}
            ><ChevronLeftOutline size="lg" /></Button>
        <select
            name="select"
            aria-label="Select"
            on:change={() => toPage(select.value)}
            bind:this={select}
            class="w-auto pr-8 bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 p-3 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
            {#each [...Array(maxPage + 1).keys()] as page}
                {#if page !== 0 || history.length === 0}
                    <option value={page}>{page}</option>
                {/if}
            {/each}
        </select>
        <Button on:click={() => toPage(curPage + 1)}
            ><ChevronRightOutline size="lg" /></Button>
    </div>
</div>
<Table hoverable={true} class="mb-5">
    <TableHead>
        <TableHeadCell class="px-8">Название файла</TableHeadCell>
        <TableHeadCell class="px-8">Дата загрузки</TableHeadCell>
        <TableHeadCell class="px-8">Тип</TableHeadCell>
        <TableHeadCell class="px-8">Статус загрузки</TableHeadCell>
        <TableHeadCell class="px-8">Скачать</TableHeadCell>
    </TableHead>
    <TableBody>
        {#each history.slice((curPage - 1) * amountPage, curPage * amountPage + amountPage - 1) as history_item}
            <TableBodyRow role="link" class="contrast">
                <TableBodyCell
                    class="px-8 overflow-hidden text-ellipsis"
                    style="max-width: 15dvw"
                    >{history_item.name_file}</TableBodyCell>
                <TableBodyCell class="px-8"
                    >{history_item.date.slice(0, 10) +
                        " " +
                        history_item.date.slice(11, 19)}</TableBodyCell>
                <TableBodyCell class="px-8"
                    >{history_item.type == "student"
                        ? "Загрузка студентов"
                        : history_item.type == "modeus"
                          ? "Выгрузка модеус"
                          : history_item.type == "site_inf"
                            ? "Cписок курсов с сайта inf-urfu"
                            : "Онлайн курс"}</TableBodyCell>
                <TableBodyCell class="px-8"
                    >{history_item.status_upload == null
                        ? "Загружается"
                        : history_item.status_upload}</TableBodyCell>
                <TableBodyCell class="px-8">
                    {#if history_item.type === "site_inf"}
                        <A
                            class="flex"
                            href="https://inf-online.urfu.ru/ru/onlain-kursy/"
                            >Сайт</A>
                    {:else}
                        <A class="flex" href={history_item.link}>Скачать</A>
                    {/if}
                </TableBodyCell>
            </TableBodyRow>
        {/each}
    </TableBody>
</Table>
