<script lang="ts">
    import {
        Button,
        Heading,
    } from "flowbite-svelte";
    import {
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
    } from "flowbite-svelte";
    import { TableHeadCell } from "flowbite-svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
    import { onMount } from "svelte";
    import { CirclePlusOutline } from "flowbite-svelte-icons";
    import http from "/src/lib/utils/http";
    import { ServerUrl } from "/src/lib/server";

    let dictNames = $state([]);
    let allCourseNamesInFile = $state([]);
    let allCourseNamesInSite = $state([]);
    let allSubjectNamesInModeus = $state([]);

    let studentStatus = http.status();

    onMount(async () => load());

    const load = async () => {
        studentStatus = studentStatus.start_load();
        dictNames =
            (await http.get("/parser/course/dict_names", studentStatus)) ?? [];
        allCourseNamesInFile =
            (await http.get("/parser/course/in_file/names", studentStatus)) ??
            [];
        allSubjectNamesInModeus =
            (await http.get("/parser/subject/names", studentStatus)) ?? [];
        allCourseNamesInSite =
            (await http.get("/parser/course/names", studentStatus)) ?? [];
        studentStatus = studentStatus.end_load();
    };

    async function save(id, value, type) {
        let name = value;

        for (let course of dictNames) {
            console.log(course);
            if (course._id === id) {
                if (type === "f") {
                    course.file_course = name;
                } else if (type === "s") {
                    course.site_inf = name;
                } else if (type === "m") {
                    course.modeus = name;
                }

                studentStatus = studentStatus.start_load();
                await http.put_json(
                    `/parser/course/dict_names/one`,
                    course,
                    studentStatus,
                );
                studentStatus = studentStatus.end_load();
                break;
            }
        }
    }

    async function add() {
        let dict = {
            modeus: allSubjectNamesInModeus[0],
            site_inf: allCourseNamesInSite[0],
            file_course: allCourseNamesInFile[0],
        };

        studentStatus = studentStatus.start_load();
        let res = await http.post(
            `/parser/course/dict_names?modeus=${dict.modeus}&site_inf=${dict.site_inf}&file_course=${dict.file_course}`,
            {},
            studentStatus,
        );
        dictNames =
            (await http.get("/parser/course/dict_names", studentStatus)) ?? [];
        studentStatus = studentStatus.end_load();
    }

    async function deleteOne(id) {
        await fetch(`${ServerUrl}/parser/course/dict_names/${id}`, {
            method: "DELETE",
            credentials: "include",
        });
        dictNames =
            (await http.get("/parser/course/dict_names", studentStatus)) ?? [];
    }
</script>

<Breadcrumbs pathItems={[{ isHome: true }, { name: "Выбор соотношений" }]} />

<Heading
    tag="h2"
    class="mb-4 mx-4">
    Соотношения
</Heading>

<Table hoverable={true}>
    <TableHead>
        <TableHeadCell class="px-8">Название в файле</TableHeadCell>
        <TableHeadCell class="px-8">Название на сайте</TableHeadCell>
        <TableHeadCell class="px-8">Название в модеусе</TableHeadCell>
        <TableHeadCell class="px-8">Удалить</TableHeadCell>
    </TableHead>
    <TableBody>
        {#each dictNames as dict}
            <TableBodyRow>
                <TableBodyCell class="px-8">
                    <select
                        onchange={(event) =>
                            // @ts-ignore
                            save(dict._id, event.target.value, "f")}
                        class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                        {#each allCourseNamesInFile as course}
                            {#if course === dict.file_course}
                                <option value={course} selected
                                    >{course}</option>
                            {:else}
                                <option value={course}>{course}</option>
                            {/if}
                        {/each}
                    </select>
                </TableBodyCell>
                <TableBodyCell class="px-8">
                    <select
                        onchange={(event) =>
                            // @ts-ignore
                            save(dict._id, event.target.value, "s")}
                        class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                        {#each allCourseNamesInSite as course}
                            {#if course === dict.site_inf}
                                <option value={course} selected
                                    >{course}</option>
                            {:else}
                                <option value={course}>{course}</option>
                            {/if}
                        {/each}
                    </select>
                </TableBodyCell>
                <TableBodyCell class="px-8">
                    <select
                        onchange={(event) =>
                            // @ts-ignore
                            save(dict._id, event.target.value, "m")}
                        class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                        {#each allSubjectNamesInModeus as course}
                            {#if course === dict.modeus}
                                <option value={course} selected
                                    >{course}</option>
                            {:else}
                                <option value={course}>{course}</option>
                            {/if}
                        {/each}
                    </select>
                </TableBodyCell>
                <TableBodyCell class="px-8">
                    <Button on:click={() => deleteOne(dict._id)}
                        >Удалить</Button>
                </TableBodyCell>
            </TableBodyRow>
        {/each}
    </TableBody>
</Table>
<div class="flex justify-center">
    <Button class="p-3 mt-2 mb-5" pill size="sm" on:click={add}
        ><CirclePlusOutline size="xl" /></Button>
    <!-- <GradientButton size='xl' class='mb-5 mt-2' outline shadow pill color="purpleToPink" on:click={() => add()}>Добавить</GradientButton> -->
</div>
