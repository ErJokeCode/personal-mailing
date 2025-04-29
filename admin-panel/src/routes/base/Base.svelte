<script lang="ts">
    import {
        Heading,
        Button,
        Table,
        TableBodyRow,
        TableBody,
        TableBodyCell,
        Label,
        Accordion,
        AccordionItem,
        Modal,
        Search,
        Input,
        Helper,
        Select,
    } from "flowbite-svelte";
    import {
        ArrowRightOutline,
        ExclamationCircleOutline,
    } from "flowbite-svelte-icons";
    import { goto } from "@mateothegreat/svelte5-router";
    import Panel from "/src/lib/components/Panel.svelte";
    import Breadcrumbs from "/src/lib/components/Breadcrumbs.svelte";
    import { onMount } from "svelte";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";
    import { Base } from "/src/lib/server";

    let notifications: ToastNotifications;

    let categories = $state([]);
    let questions = $state([]);

    let searchTerm = $state("");
    let searched = $state(false);
    let selected = $state();

    let addOpen = $state(false);
    let newCategory = $state("");
    let newDescription = $state("");
    let errAddCategory = $state("");

    let editOpen = $state(false);
    let changeCategory = $state("");
    let changeDescription = $state("");
    let errChangeCategory = $state("");

    let deleteOpen = $state(false);

    onMount(async () => {
        await getCategories();
        await getQuestions();
    });

    const getCategories = async () => {
        let response = await fetch(`${Base}/category/`, {
            method: 'GET',
            credentials: "include",
        });
        let json = await response?.json();
        categories = json;
    };

    const getQuestions = async () => {
        let response = await fetch(`${Base}/question/`, {
            method: 'GET',
            credentials: "include",
        });
        let json = await response?.json();
        questions = json;
    };
    
    const addCategory = async () => {
        if (newCategory.length < 1) {
            errAddCategory = 'Длина должна быть больше или равна 1 символу';
            return;
        }
        let body = {
            name: newCategory,
            description: newDescription,
        };
        let response = await fetch(`${Base}/category/`, {
            method: 'POST',
            headers: {
                Accept: "application/json, */*",
                "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
            credentials: "include",
        });
        if (!response.ok) {
            errAddCategory = response.status + ' ' + response.statusText;
            return;
        }
        newCategory = "";
        newDescription = "",
        getCategories();
        addOpen = false;
    };

    const editCategory = async (id) => {
        if (changeCategory.length < 1) {
            errChangeCategory = 'Длина должна быть больше или равна 1 символу';
            return;
        }
        let body = {
            name: changeCategory,
            description: changeDescription,
        };
        let response = await fetch(`${Base}/category/${id}`, {
            method: 'PATCH',
            headers: {
                Accept: "application/json, */*",
                "Content-Type": "application/json",
            },
            credentials: "include",
            body: JSON.stringify(body)
        });
        if (!response.ok) {
            errChangeCategory = response.status + ' ' + response.statusText;
            return;
        }
        changeCategory = "";
        changeDescription = "",
        getCategories();
        editOpen = false;
    };
    
    const deleteCategory = async (id) => {
        await fetch(`${Base}/category/${id}`, {
            method: 'DELETE',
            credentials: "include",
        });
        getCategories();
    };

    const enter = (event) => {
        if (event.key == "Enter") search();
    }

    const search = async () => {
        searched = true;
        if (searchTerm === '') {
            searched = false;
            getQuestions();
            return;
        }
        let url = new URL(`${Base}/question/`);

        url.searchParams.append("search", searchTerm);
        if (selected !== "Все категории") {
            url.searchParams.append("id_category", document.getElementById('selected').value );
        }
        
        let response = await fetch(url, { credentials: "include" });
        let json = await response?.json();
        questions = json;
    };

    function fullInfo(id) {
        goto(`/base-edit/${id}`);
    }

    function edit(category, description) {
        editOpen = true;
        changeCategory = category;
        changeDescription = description;
    }
</script>

<ToastNotifications bind:this={notifications} />

<Breadcrumbs pathItems={[{ isHome: true }, { name: "База знаний" }]} />

<Panel class="m-4">
    <Heading
        tag="h1"
        class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
        База знаний
    </Heading>

    <div class="flex justify-between mb-3">
        <div class="block w-1/2">
            <Search
                placeholder="Поиск"
                bind:value={searchTerm}
                on:keypress={enter}>
                <Button class="me-1" on:click={() => search()}>Поиск</Button>
            </Search>
        </div>
        <div class="block">
            <Button on:click={() => goto("/base-add")} size='lg'>
                Добавить вопрос
                <ArrowRightOutline class="w-6 h-6 ms-2" />
            </Button>
        </div>
    </div>
    
    <Label class="space-y-2 mb-2">Категория</Label>
    {#if categories.total_record !== 0}
        <select id="selected"
            bind:value={selected}
            class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg mb-5
                focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600
                dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
            <option selected value='Все категории'>Все категории</option>
            {#each categories.content as category}
                <option value={category.id}>{category.name}</option>
            {/each}
        </select>
    {:else}
        <Select disabled placeholder='Нет категорий' />
    {/if}

    {#if searched}
        <Table hoverable>
            <TableBody>
                {#each questions.content as question}
                    <TableBodyRow on:click={() => fullInfo(question.id)}>
                        <TableBodyCell
                            >{question.question}</TableBodyCell>
                    </TableBodyRow>
                {/each}
            </TableBody>
        </Table>
    {:else}
        <Accordion
            class="mb-5"
            multiple
            classActive="dark:bg-gray-600 dark:focus:ring-gray-700">
            {#each categories.content as category}
                <AccordionItem
                    borderOpenClass="p-1 border-s border-e border-b border-gray-200 dark:border-gray-700">
                    <span slot="header">{category.name}</span>
                    <div class="mx-3 mt-3">
                        <b>Описание: </b>
                        {category.description}
                    </div>
                    <div class="mx-3 space-x-3">
                        <Button class='my-3' on:click={() => edit(category.name, category.description)}
                            >Редактировать категорию</Button>
                        <Button class='mb-3' on:click={() => deleteOpen = true}
                            >Удалить категорию</Button>
                    </div>
                    <Modal
                        title="Редактировать категорию"
                        bind:open={editOpen}>
                        <Label class="flex flex-col space-y-2">
                            <span>Название категории</span>
                            <Input
                                bind:value={changeCategory}
                                type="text"
                                placeholder="Введите название"
                                size="md" />
                            <Helper class="text-md" color="red">{errChangeCategory}</Helper>
                        </Label>
                        <Label class="flex flex-col space-y-2">
                            <span>Описание</span>
                            <Input
                                bind:value={changeDescription}
                                type="text"
                                placeholder="Введите описание"
                                size="md" />
                        </Label>
                        <svelte:fragment slot="footer">
                            <Button class='ml-auto'
                                on:click={() => editCategory(category.id)}
                                >Подтвердить</Button>
                            <Button color="alternative" on:click={() => { editOpen = false; changeCategory = ''; errChangeCategory = ''; }}
                                >Отменить</Button>
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
                                on:click={() => deleteCategory(category.id)}
                                >Да, удалить</Button>
                            <Button color="alternative">Нет, не удалять</Button>
                        </div>
                    </Modal>
                    <Table hoverable>
                        <TableBody>
                            {#each questions.content as question}
                                {#if category.id === question.id_category}
                                    <TableBodyRow on:click={() => fullInfo(question.id)}>
                                        <TableBodyCell
                                            >{question.question}</TableBodyCell>
                                    </TableBodyRow>
                                {/if}
                            {/each}
                        </TableBody>
                    </Table>
                </AccordionItem>
            {/each}
        </Accordion>
    {/if}

    <div class="absolute bottom-6 right-6">
        <Button on:click={() => (addOpen = true)} size="lg"
            >Добавить категорию</Button>
        <Modal title="Добавить категорию" bind:open={addOpen}>
            <Label class="flex flex-col space-y-2">
                <span>Название категории</span>
                <Input
                    bind:value={newCategory}
                    type="text"
                    placeholder="Введите название"
                    size="md" />
                <Helper class="text-md" color="red">{errAddCategory}</Helper>
            </Label>
            <Label class="flex flex-col space-y-2">
                <span>Описание</span>
                <Input
                    bind:value={newDescription}
                    type="text"
                    placeholder="Введите описание"
                    size="md" />
            </Label>
            <svelte:fragment slot="footer">
                <Button class='ml-auto' on:click={() => addCategory()}>Добавить</Button>
                <Button color="alternative" on:click={() => {addOpen = false; errAddCategory = ''; newCategory = ''}}>Отменить</Button>
            </svelte:fragment>
        </Modal>
    </div>
</Panel>
