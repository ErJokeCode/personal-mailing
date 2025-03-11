<script>
	import { Breadcrumb, BreadcrumbItem, Heading, Button, Table, TableBodyRow, TableBody, TableBodyCell, Label } from 'flowbite-svelte';
	import { Accordion, AccordionItem, Modal, Search, Input } from 'flowbite-svelte';
    import { ArrowRightOutline, ExclamationCircleOutline, GoToPrevCellOutline } from 'flowbite-svelte-icons';
    import { Link, navigate } from "svelte-routing";
    import { onMount } from "svelte";
    import { server_url } from "../../utils/store";

    let topics = ['Общие вопросы', 'Технические вопросы', 'Административные вопросы', 'Безопасность'];
    let searchTerm = '';

    // onMount(async () => {
    //     let response;

    //     try {
    //         response = await fetch(`${server_url}/core/admin/notifications`, {
    //             credentials: "include",
    //         });
    //     } catch (err) { }
    //     let json = await response?.json();
    //     topics = json.items;
    // });

    $: filtered = topics.filter((item) => item.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1);

    function fullInfo(id) {
        navigate(`/base/${id}`);
    }
    
    let addOpen = false;
    let editOpen = false;
    let deleteOpen = false;

    let name = 'Уаэуауаээуауэ';
    let newTopic;

    const add = () => {
        topics.push(newTopic);
        filtered = topics;
        newTopic = null;
    }
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="pt-4 px-6">
                <Breadcrumb class="mb-5">
                    <Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link>
                    <BreadcrumbItem>База знаний</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                    База знаний
                </Heading>
                <div class="flex justify-between">
                    <div class="block w-1/2">
                        <Search size='md' class='block p-2.5 ps-10 text-sm mb-4 w-full' placeholder="Поиск" bind:value={searchTerm} />
                    </div>
                    <div class="block">
                        <Button on:click={() => navigate('/base/add')} class="mb-3">
                            Добавить вопрос
                            <ArrowRightOutline class="w-6 h-6 ms-2" />
                        </Button>
                    </div>
                </div>
                <Accordion class='mb-5' multiple classActive='dark:bg-gray-600 dark:focus:ring-gray-700'>
                    {#each filtered as topic}
                    <AccordionItem borderOpenClass='p-1 border-s border-e border-b border-gray-200 dark:border-gray-700'>
                        <span slot="header">{topic}</span>
                        <div class="m-3 space-x-3">
                            <Button on:click={() => (editOpen = true)}>Редактировать категорию</Button>
                            <Button on:click={() => (deleteOpen = true)}>Удалить категорию</Button>
                        </div>
                        <Modal title="Редактировать категорию" bind:open={editOpen} autoclose>
                            <Label class="space-y-2">
                                <span>Название категории</span>
                                <Input bind:value={topic} type="text" placeholder="Введите название" size="md" />
                            </Label>
                            <svelte:fragment slot="footer">
                                <Button>Подтвердить</Button>
                                <Button on:click={() => topic = name} color="alternative">Отменить</Button>
                            </svelte:fragment>
                        </Modal>
                        <Modal bind:open={deleteOpen} size="xs" autoclose>
                            <div class="text-center">
                                <ExclamationCircleOutline class="mx-auto mb-4 text-gray-400 w-12 h-12 dark:text-gray-200" />
                                <h3 class="mb-5 text-lg font-normal text-gray-500 dark:text-gray-400">Вы уверены, что хотите удалить категорию?</h3>
                                <Button color="red" class="me-2" on:click={() => alert('ауауауауа')}>Да, уверен</Button>
                                <Button color="alternative">Нет, не уверен</Button>
                            </div>
                        </Modal>
                        <Table hoverable>
                            <TableBody>
                                <TableBodyRow on:click={() => fullInfo('edit')}>
                                    <TableBodyCell>Как создать/удалить пользователя?</TableBodyCell>
                                </TableBodyRow>
                                <TableBodyRow on:click={() => fullInfo('edit')}>
                                    <TableBodyCell>Как создать/удалить пользователя?</TableBodyCell>
                                </TableBodyRow>
                                <TableBodyRow on:click={() => fullInfo('edit')}>
                                    <TableBodyCell>Как создать/удалить пользователя?</TableBodyCell>
                                </TableBodyRow>
                            </TableBody>
                        </Table>
                    </AccordionItem>
                    {/each}
                </Accordion>
            </div>
        </div>
    </div>
    <div class='absolute bottom-10 right-10'>
        <Button on:click={() => (addOpen = true)} size='lg'>Добавить категорию</Button>
        <Modal title="Добавить категорию" bind:open={addOpen} autoclose>
            <Label class="space-y-2">
                <span>Название категории</span>
                <Input bind:value={newTopic} type="text" placeholder="Введите название" size="md" />
            </Label>
            <svelte:fragment slot="footer">
                <Button on:click={() => add()}>Добавить</Button>
                <Button color="alternative">Отменить</Button>
            </svelte:fragment>
        </Modal>
    </div>
</div>