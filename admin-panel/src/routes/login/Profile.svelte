<script>
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Table,
        TableHead,
        TableHeadCell,
        TableBody,
        TableBodyRow,
        TableBodyCell,
    } from "flowbite-svelte";
    import { admin } from "../../utils/store";
    import { Link } from "svelte-routing";
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div
            class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800"
        >
            <div class="p-4 px-6">
                <Breadcrumb class="mb-5">
                    <Link
                        class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"
                        ><svg
                            class="w-4 h-4 me-2"
                            fill="currentColor"
                            viewBox="0 0 20 20"
                            xmlns="http://www.w3.org/2000/svg"
                        >
                            <path
                                d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"
                            ></path></svg
                        >Главная</Link
                    >
                    <BreadcrumbItem>Профиль</BreadcrumbItem>
                </Breadcrumb>
                <Heading
                    tag="h1"
                    class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl"
                >
                    Профиль
                </Heading>
            </div>
            <Table>
                <TableHead>
                    <TableHeadCell class="px-8">{$admin.email}</TableHeadCell>
                </TableHead>
                <TableBody>
                    <TableBodyRow>
                        <TableBodyCell class="px-8"
                            >{$admin.createdAt}</TableBodyCell
                        >
                    </TableBodyRow>
                </TableBody>
            </Table>
        </div>
    </div>
</div>

<style>
</style>
