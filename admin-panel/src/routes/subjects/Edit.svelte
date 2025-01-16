<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Button, Heading, Select } from 'flowbite-svelte';
	import { Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
	import { TableHeadCell } from 'flowbite-svelte';

	import { onMount } from "svelte";
    import { Link } from "svelte-routing";
    import { CirclePlusOutline } from 'flowbite-svelte-icons';
    import http from "../../utils/http";
    import { server_url } from "../../utils/store.js"

    let dictNames = $state([])
    let allCourseNamesInFile = $state([])
    let allCourseNamesInSite = $state([])
    let allSubjectNamesInModeus = $state([])

    let studentStatus = http.status();

    onMount(async () => load());
    
    const load = async () => {
        studentStatus = studentStatus.start_load();
        dictNames = (await http.get("/parser/course/dict_names", studentStatus)) ?? [];
        allCourseNamesInFile = (await http.get("/parser/course/in_file/names", studentStatus)) ?? [];
        allSubjectNamesInModeus = (await http.get("/parser/subject/names", studentStatus)) ?? [];
        allCourseNamesInSite = (await http.get("/parser/course/names", studentStatus)) ?? [];
        studentStatus = studentStatus.end_load();
    }
  
    async function save(id, value, type) {
        let name = value

        for (let course of dictNames) {
            console.log(course)
            if (course._id === id) {
                if (type === "f") {
                    course.file_course = name;
                } 
                else if (type === "s") {
                    course.site_inf = name;
                }
                else if (type === "m") {
                    course.modeus = name;
                }

                studentStatus = studentStatus.start_load();
                await http.put_json(`/parser/course/dict_names/one`, course, studentStatus);
                studentStatus = studentStatus.end_load();
                break;
            }
        }
    }

    async function add() {
        let dict = {
            modeus: allSubjectNamesInModeus[0],
            site_inf: allCourseNamesInSite[0],
            file_course: allCourseNamesInFile[0]
        }

        studentStatus = studentStatus.start_load();
        let res = await http.post(`/parser/course/dict_names?modeus=${dict.modeus}&site_inf=${dict.site_inf}&file_course=${dict.file_course}`, {}, studentStatus);
        dictNames = (await http.get("/parser/course/dict_names", studentStatus)) ?? [];
        studentStatus = studentStatus.end_load();
    }

    async function deleteOne(id) {
        await fetch(`${server_url}/parser/course/dict_names/${id}`, {
            method: "DELETE",
            credentials: "include",
        });
        dictNames = (await http.get("/parser/course/dict_names", studentStatus)) ?? [];
    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="p-4 px-6">
                <Breadcrumb class="mb-5">
                    <Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link>
                    <BreadcrumbItem>Редактор соотношений</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
                    Соотношения
                </Heading>
            </div>
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
                                <select on:change={(event) => save(dict._id, event.target.value, "f")} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                                    {#each allCourseNamesInFile as course}
                                        {#if course === dict.file_course}
                                            <option value={course} selected>{course}</option>
                                        {:else}
                                            <option value={course}>{course}</option>
                                        {/if}
                                    {/each}
                                </select>
                            </TableBodyCell>
                            <TableBodyCell class="px-8">
                                <select on:change={(event) => save(dict._id, event.target.value, "s")} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                                    {#each allCourseNamesInSite as course}
                                        {#if course === dict.site_inf}
                                            <option value={course} selected>{course}</option>
                                        {:else}
                                            <option value={course}>{course}</option>
                                        {/if}
                                    {/each}
                                </select>
                            </TableBodyCell>
                            <TableBodyCell class="px-8">
                                <select on:change={(event) => save(dict._id, event.target.value, "m")} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                                    {#each allSubjectNamesInModeus as course}
                                        {#if course === dict.modeus}
                                            <option value={course} selected>{course}</option>
                                        {:else}
                                            <option value={course}>{course}</option>
                                        {/if}
                                    {/each}
                                </select>
                            </TableBodyCell>
                            <TableBodyCell class="px-8">
                                <Button on:click={() => deleteOne(dict._id)}>Удалить</Button>
                            </TableBodyCell>
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
            <div class='flex justify-center'>
                <Button class='p-3 mt-2 mb-5' pill size='sm' on:click={add}><CirclePlusOutline size='xl'/></Button>
                <!-- <GradientButton size='xl' class='mb-5 mt-2' outline shadow pill color="purpleToPink" on:click={() => add()}>Добавить</GradientButton> -->
            </div>
        </div>
    </div>
</div>