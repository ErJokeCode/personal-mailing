<script>
	import { Breadcrumb, BreadcrumbItem, Heading, Button, Textarea } from 'flowbite-svelte';
	import { Label, Modal } from 'flowbite-svelte';
    import { Link } from "svelte-routing";
    import { ExclamationCircleOutline } from "flowbite-svelte-icons";
    import { onMount } from "svelte";
    import http from "../../utils/http";

    let topic;
    let email;

    let question = 'ааааааааааааааа';
    let answer = 'уууууууууууууу';

    let sendOpen = false;
    let deleteOpen = false;
    

    let chats = [];
    let status = http.status();

    onMount(async () => {
        status = status.start_load();
        chats = (await http.get("/core/admin/chats", status)).items ?? [];
        status = status.end_load();
        console.log(chats)
    });
</script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
            <div class="pt-4 px-6">
                <Breadcrumb class="mb-5">
                    <li class="inline-flex items-center"><Link class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"
                        to="/"><svg class="w-4 h-4 me-2" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 01
                        1 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>Главная</Link></li>
                    <li class="inline-flex items-center"><svg class="w-6 h-6 text-gray-400 rtl:-scale-x-100" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                        <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0
                        z" clip-rule="evenodd"></path></svg>
                    <Link class="ml-0 ms-1 text-sm font-medium text-gray-700 hover:text-gray-900 md:ms-2 dark:text-gray-400 dark:hover:text-white" to="/base">База знаний</Link></li>
                    <BreadcrumbItem>Редактировать вопрос</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                    Редактировать вопрос
                </Heading>
                <form class="mb-5">
                    <Label class="space-y-2 mb-2">Тема</Label>
                    <div class="flex space-x-3">
                        <select bind:value={topic} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg mr-auto
                                focus:ring-orange-500 focus:border-orange-500 block w-1/2 p-2.5 dark:bg-gray-700 dark:border-gray-600
                                dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                            <option selected value='Общие вопросы'>Общие вопросы</option>
                            <option value='Технические вопросы'>Технические вопросы</option>
                            <option value='Административные вопросы'>Административные вопросы</option>
                        </select>
                        <Button size='md' on:click={() => sendOpen = true}>Отправить</Button>
                        <Button size='md' class='text-nowrap' on:click={() => deleteOpen = true}>Удалить вопрос</Button>
                    </div>
                    <Modal title="Отправить ответ на вопрос студенту" bind:open={sendOpen} autoclose>
                        <form>
                            <Label class="space-y-2 mb-2">Выберите чат</Label>
                            <select bind:value={email} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg
                                    focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600
                                    dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                                {#each chats as chat}
                                    <option value={chat.student.email}>{chat.student.email}</option>
                                {/each}
                            </select>
                        </form>
                        <svelte:fragment slot="footer">
                            <Button>Отправить</Button>
                            <Button color="alternative">Отменить</Button>
                        </svelte:fragment>
                    </Modal>
                    <Modal bind:open={deleteOpen} size="xs" autoclose>
                        <div class="text-center">
                            <ExclamationCircleOutline class="mx-auto mb-4 text-gray-400 w-12 h-12 dark:text-gray-200" />
                            <h3 class="mb-5 text-lg font-normal text-gray-500 dark:text-gray-400">Вы уверены, что хотите удалить вопрос?</h3>
                            <Button color="red" class="me-2" on:click={() => alert('ауауауауа')}>Да, уверен</Button>
                            <Button color="alternative">Нет, не уверен</Button>
                        </div>
                    </Modal>
                </form>
                <div class='mb-5'>
                    <Label for="textarea-id" class="mb-2">Вопрос</Label>
                    <Textarea bind:value={question} id="textarea-id" placeholder="Введите текст" rows="4" name="message" />
                </div>
                <div class='mb-5'>
                    <Label for="textarea-id" class="mb-2">Ответ</Label>
                    <Textarea bind:value={answer} id="textarea-id" placeholder="Введите текст" rows="4" name="message" />
                </div>
                <div class='flex justify-center'>
                    <Button size='lg' class="mb-10">Сохранить</Button>
                </div>
            </div>
        </div>
    </div>
</div>