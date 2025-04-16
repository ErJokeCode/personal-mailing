<script lang="ts">
    import Panel from "/src/lib/components/Panel.svelte";
    import {
        Button,
        Checkbox,
        Dropzone,
        Heading,
        Helper,
        Input,
        Label,
        Select,
        Spinner,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
        Textarea,
    } from "flowbite-svelte";
    import { createPaged } from "/src/lib/components/Paged.svelte";
    import PagedList from "/src/lib/components/PagedList.svelte";
    import { NotificationsApi, PageSize, StudentsApi } from "/src/lib/server";
    import BackButton from "/src/lib/components/BackButton.svelte";
    import { CloudArrowUpOutline } from "flowbite-svelte-icons";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import { goto } from "@mateothegreat/svelte5-router";

    const courseItems = [
        { name: 0, value: 0 },
        { name: 1, value: 1 },
        { name: 2, value: 2 },
        { name: 3, value: 3 },
        { name: 4, value: 4 },
        { name: 5, value: 5 },
        { name: 6, value: 6 },
    ];

    const costItems = [
        { name: "Не выбрано", value: "" },
        { name: "Бюджет", value: "Бюджет" },
        { name: "Контракт", value: "Контракт" },
    ];

    const educationItems = [
        { name: "Не выбрано", value: "" },
        { name: "Очная", value: "Очная" },
        { name: "Очно-заочная", value: "Очно-заочная" },
        { name: "Заочная", value: "Заочная" },
    ];

    let notifications: ToastNotifications;
    let files: FileList = $state(null);
    let errors = $state({ Content: null, StudentIds: null });
    let selected = new Map<string, object>();
    let trigger = $state(false);
    let content = $state("");
    let sendText = $state("Отправить");

    let allStudents = $state({
        paged: createPaged(),
        search: "",
        selectAll: false,

        group: "",
        courseNumber: 0,
        typeOfCost: "",
        typeOfEducation: "",
        onlineCourse: "",
        subject: "",
        team: "",
    });

    let filters = $state({
        group: "",
        courseNumber: 0,
        typeOfCost: "",
        typeOfEducation: "",
        onlineCourse: "",
        subject: "",
        team: "",
    });

    async function getStudents(_) {
        let url = new URL(StudentsApi);

        url.searchParams.append(
            "courseNumber",
            filters.courseNumber.toString(),
        );
        url.searchParams.append("group", filters.group);
        url.searchParams.append("typeOfEducation", filters.typeOfEducation);
        url.searchParams.append("typeOfCost", filters.typeOfCost);
        url.searchParams.append("onlineCourse", filters.onlineCourse);
        url.searchParams.append("subject", filters.subject);
        url.searchParams.append("team", filters.team);

        url.searchParams.append("search", allStudents.search);
        url.searchParams.append("page", allStudents.paged.page.toString());
        url.searchParams.append("pageSize", PageSize.toString());

        let res = await fetch(url, { credentials: "include" });
        let body = await res.json();

        Object.assign(allStudents.paged, body);

        return body;
    }

    function select(student, checked) {
        if (checked) {
            selected.set(student.id, student);
        } else {
            selected.delete(student.id);
        }
    }

    async function selectAll(checked: boolean) {
        try {
            let url = new URL(StudentsApi);

            url.searchParams.append(
                "courseNumber",
                filters.courseNumber.toString(),
            );
            url.searchParams.append("group", filters.group);
            url.searchParams.append("typeOfEducation", filters.typeOfEducation);
            url.searchParams.append("typeOfCost", filters.typeOfCost);
            url.searchParams.append("onlineCourse", filters.onlineCourse);
            url.searchParams.append("subject", filters.subject);
            url.searchParams.append("team", filters.team);

            url.searchParams.append("search", allStudents.search);
            url.searchParams.append("page", allStudents.paged.page.toString());
            url.searchParams.append("pageSize", PageSize.toString());

            let res = await fetch(url, { credentials: "include" });
            let body = await res.json();

            if (checked) {
                for (let item of body.items) {
                    selected.set(item.id, item);
                }
            } else {
                for (let item of body.items) {
                    selected.delete(item.id);
                }
            }

            trigger = !trigger;
        } catch {
            notifications.add({
                type: "error",
                text: "Ошибка при выборе всех студентов",
            });
        }
    }

    function clearFiles() {
        files = null;
    }

    function clearFilters() {
        allStudents.group = "";
        allStudents.courseNumber = 0;
        allStudents.typeOfCost = "";
        allStudents.typeOfEducation = "";
        allStudents.onlineCourse = "";
        allStudents.subject = "";
        allStudents.team = "";

        applyFilters();
    }

    function applyFilters() {
        Object.assign(filters, allStudents);
    }

    function keypress(event) {
        if (event.key == "Enter") {
            applyFilters();
        }
    }

    function clearSelection() {
        allStudents.selectAll = false;
        selected.clear();
        trigger = !trigger;
    }

    async function send() {
        sendText = null;

        try {
            let data = new FormData();

            let body = JSON.stringify({
                content: content,
                studentIds: selected.keys().toArray(),
            });

            data.append("body", body);

            if (files !== null) {
                for (let file of files) {
                    data.append("documents", file);
                }
            }

            let res = await fetch(NotificationsApi, {
                method: "Post",
                body: data,
                credentials: "include",
            });

            if (res.ok) {
                errors = { Content: null, StudentIds: null };
                let body = await res.json();

                notifications.add({
                    type: "ok",
                    text: "Рассылка успешно отправлена",
                    content: notification,
                    params: body.id,
                    autohide: false,
                });
            } else {
                let body = await res.json();
                errors = body.errors;
            }
        } catch (err) {
            notifications.add({
                type: "error",
                text: "Ошибка при отправке рассылки",
            });
        }

        sendText = "Отправить";
    }

    function single(id) {
        goto(`/notifications/${id}`);
    }
</script>

{#snippet notification(id)}
    <Button color="green" size="sm" on:click={() => single(id)}>Детали</Button>
{/snippet}

<ToastNotifications bind:this={notifications} />

<BackButton title="Отправить рассылку" class="m-4" />

<div class="mx-4 flex gap-4 mt-4 flex-wrap lg:flex-nowrap">
    <Panel class="flex-1">
        <Dropzone bind:files multiple class="mb-4">
            {#if !files || files.length < 0}
                <CloudArrowUpOutline class="dark:text-white" size="xl" />

                <p class="mb-2 text-sm text-gray-500 dark:text-gray-400">
                    <span class="font-semibold">Перетащите сюда файлы</span>
                </p>

                <p class="text-xs text-gray-500 dark:text-gray-400">
                    Либо нажмите, чтобы загрузить
                </p>
            {:else}
                {#each files as file}
                    <p class="text-xl dark:text-white">
                        {file.name}
                    </p>
                {/each}
            {/if}
        </Dropzone>

        <Textarea
            bind:value={content}
            class="mb-4"
            rows={6}
            placeholder="Введите текст" />

        {#if errors["Content"]}
            <Helper color="red" class="text-lg mb-4"
                >{errors["Content"]}</Helper>
        {/if}

        <div class="flex gap-2">
            <Button on:click={send}>
                {#if sendText}
                    {sendText}
                {:else}
                    <Spinner size="6" color="white" />
                {/if}
            </Button>
            <Button on:click={clearFiles}>Сбросить вложения</Button>
        </div>
    </Panel>

    <Panel class="flex-1">
        <Heading tag="h3" class="mb-2">Фильтры</Heading>

        <div class="flex gap-4 mb-4">
            {@render inputSelect(
                "course",
                "Номер курса",
                "courseNumber",
                courseItems,
            )}

            {@render inputText("group", "Группа", "group", "РИ-123456")}
        </div>

        <div class="flex gap-4 mb-4">
            {@render inputSelect("cost", "Тип затрат", "typeOfCost", costItems)}

            {@render inputText(
                "online",
                "Онлайн курс",
                "onlineCourse",
                "Название курса",
            )}
        </div>

        <div class="flex gap-4 mb-4">
            {@render inputSelect(
                "education",
                "Форма обучения",
                "typeOfEducation",
                educationItems,
            )}

            {@render inputText(
                "subject",
                "Предмет",
                "subject",
                "Название предмета",
            )}
        </div>

        {@render inputText("team", "Команда", "team", "Название команды")}

        <div class="flex gap-2 mt-4">
            <Button on:click={applyFilters}>Применить</Button>
            <Button on:click={clearFilters}>Сбросить</Button>
        </div>
    </Panel>
</div>

{#snippet inputText(
    name: string,
    label: string,
    bind: string,
    placeholder: string,
)}
    <div class="flex-1">
        <Label for={name} class="text-lg mb-1">{label}</Label>
        <Input
            on:keypress={keypress}
            bind:value={allStudents[bind]}
            {placeholder}
            id={name}
            type="text" />
    </div>
{/snippet}

{#snippet inputSelect(name: string, label: string, bind: string, items: any[])}
    <div class="flex-1">
        <Label for={name} class="text-lg mb-1">{label}</Label>
        <Select
            bind:value={allStudents[bind]}
            placeholder=""
            id={name}
            {items} />
    </div>
{/snippet}

{#if errors["StudentIds"]}
    <Helper color="red" class="text-lg m-4">{errors["StudentIds"]}</Helper>
{/if}

<PagedList
    get={() => getStudents(trigger)}
    bind:paged={allStudents.paged}
    bind:search={allStudents.search}>
    {#snippet before()}
        <div class="ml-4 mb-4 flex gap-2">
            <Button on:click={clearSelection}>Очистить</Button>
        </div>
    {/snippet}

    {#snippet children(body)}
        <Table>
            <TableHead>
                <TableHeadCell>
                    <Checkbox
                        bind:checked={allStudents.selectAll}
                        on:click={(event) =>
                            //@ts-ignore
                            selectAll(event.target.checked)} />
                </TableHeadCell>
                <TableHeadCell>Курс</TableHeadCell>
                <TableHeadCell>Фамилия</TableHeadCell>
                <TableHeadCell>Имя</TableHeadCell>
                <TableHeadCell>Отчество</TableHeadCell>
                <TableHeadCell>Группа</TableHeadCell>
                <TableHeadCell>Направление</TableHeadCell>
                <TableHeadCell>Форма</TableHeadCell>
                <TableHeadCell>Тип</TableHeadCell>
                <TableHeadCell>Электронная почта</TableHeadCell>
            </TableHead>
            <TableBody>
                {#each body.items as student}
                    <TableBodyRow class="text-lg">
                        <TableBodyCell>
                            <Checkbox
                                checked={selected.has(student.id)}
                                on:click={(event) => {
                                    // @ts-ignore
                                    select(student, event.target.checked);
                                }} />
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.info.group.numberCourse}
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.info.surname}
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.info.name}
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.info.patronymic}
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.info.group.number}
                        </TableBodyCell>
                        <TableBodyCell>
                            <div
                                class="text-sm font-normal text-gray-500 dark:text-gray-400">
                                <div
                                    class="text-base font-semibold text-gray-900 dark:text-white">
                                    {student.info.group.directionCode}
                                </div>
                                <div
                                    class="text-sm font-normal text-gray-500 dark:text-gray-400 overflow-hidden text-ellipsis"
                                    style="max-width: 15dvw">
                                    {student.info.group.nameSpeciality}
                                </div>
                            </div>
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.info.typeOfEducation}
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.info.typeOfCost}
                        </TableBodyCell>
                        <TableBodyCell>
                            {student.email}
                        </TableBodyCell>
                    </TableBodyRow>
                {/each}
            </TableBody>
        </Table>
    {/snippet}
</PagedList>
