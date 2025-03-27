<script>
    import {
        Breadcrumb,
        BreadcrumbItem,
        Heading,
        Button,
        Table,
        TableBodyRow,
        TableBody,
        TableBodyCell,
        Label,
    } from "flowbite-svelte";
    import {
        Accordion,
        AccordionItem,
        Modal,
        Search,
        Input,
    } from "flowbite-svelte";
    import {
        ArrowRightOutline,
        ExclamationCircleOutline,
        GoToPrevCellOutline,
    } from "flowbite-svelte-icons";
    import { goto, route } from "@mateothegreat/svelte5-router";
    import Panel from "/src/lib/components/Panel.svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";

    let topics = [
        "Общие вопросы",
        "Технические вопросы",
        "Административные вопросы",
        "Безопасность",
    ];
    let searchTerm = "";

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

    $: filtered = topics.filter(
        (item) => item.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1,
    );

    function fullInfo(id) {
        goto(`/${id}`);
    }

    let addOpen = false;
    let editOpen = false;
    let deleteOpen = false;

    let name = "Уаэуауаээуауэ";
    let newTopic;

    const add = () => {
        topics.push(newTopic);
        filtered = topics;
        newTopic = null;
    };
</script>

<Panel class="m-4">
    <Breadcrumbs pathItems={[{ isHome: true }, { name: "База знаний" }]} />

    <Heading
        tag="h1"
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
        База знаний
    </Heading>

    <div class="flex justify-between">
        <div class="block w-1/2">
            <Search
                size="md"
                class="block p-2.5 ps-10 text-sm mb-4 w-full"
                placeholder="Поиск"
                bind:value={searchTerm} />
        </div>
        <div class="block">
            <Button on:click={() => goto("/base-add")} class="mb-3">
                Добавить вопрос
                <ArrowRightOutline class="w-6 h-6 ms-2" />
            </Button>
        </div>
    </div>

    <Accordion
        class="mb-5"
        multiple
        classActive="dark:bg-gray-600 dark:focus:ring-gray-700">
        {#each filtered as topic}
            <AccordionItem
                borderOpenClass="p-1 border-s border-e border-b border-gray-200 dark:border-gray-700">
                <span slot="header">{topic}</span>
                <div class="m-3 space-x-3">
                    <Button on:click={() => (editOpen = true)}
                        >Редактировать категорию</Button>
                    <Button on:click={() => (deleteOpen = true)}
                        >Удалить категорию</Button>
                </div>
                <Modal
                    title="Редактировать категорию"
                    bind:open={editOpen}
                    autoclose>
                    <Label class="flex flex-col space-y-2">
                        <span>Название категории</span>
                        <Input
                            bind:value={topic}
                            type="text"
                            placeholder="Введите название"
                            size="md" />
                    </Label>
                    <svelte:fragment slot="footer">
                        <Button>Подтвердить</Button>
                        <Button
                            on:click={() => (topic = name)}
                            color="alternative">Отменить</Button>
                    </svelte:fragment>
                </Modal>
                <Modal bind:open={deleteOpen} size="xs" autoclose>
                    <div class="text-center">
                        <ExclamationCircleOutline
                            class="mx-auto mb-4 text-gray-400 w-12 h-12 dark:text-gray-200" />
                        <h3
                            class="mb-5 text-lg font-normal text-gray-500 dark:text-gray-400">
                            Вы уверены, что хотите удалить категорию?
                        </h3>
                        <Button
                            color="red"
                            class="me-2"
                            on:click={() => alert("ауауауауа")}
                            >Да, удалить</Button>
                        <Button color="alternative">Нет, не удалять</Button>
                    </div>
                </Modal>
                <Table hoverable>
                    <TableBody>
                        <TableBodyRow on:click={() => fullInfo("base-edit")}>
                            <TableBodyCell
                                >Как создать/удалить пользователя?</TableBodyCell>
                        </TableBodyRow>
                        <TableBodyRow on:click={() => fullInfo("base-edit")}>
                            <TableBodyCell
                                >Как создать/удалить пользователя?</TableBodyCell>
                        </TableBodyRow>
                        <TableBodyRow on:click={() => fullInfo("base-edit")}>
                            <TableBodyCell
                                >Как создать/удалить пользователя?</TableBodyCell>
                        </TableBodyRow>
                    </TableBody>
                </Table>
            </AccordionItem>
        {/each}
    </Accordion>

    <div class="absolute bottom-6 right-6">
        <Button on:click={() => (addOpen = true)} size="lg"
            >Добавить категорию</Button>
        <Modal title="Добавить категорию" bind:open={addOpen} autoclose>
            <Label class="flex flex-col space-y-2">
                <span>Название категории</span>
                <Input
                    bind:value={newTopic}
                    type="text"
                    placeholder="Введите название"
                    size="md" />
            </Label>
            <svelte:fragment slot="footer">
                <Button on:click={() => add()}>Добавить</Button>
                <Button color="alternative">Отменить</Button>
            </svelte:fragment>
        </Modal>
    </div>
</Panel>