<script>
	import { Breadcrumb, BreadcrumbItem, Heading, Button, Textarea } from 'flowbite-svelte';
	import { Accordion, AccordionItem, Input, Label, Search } from 'flowbite-svelte';
    import { Link, navigate } from "svelte-routing";

    let topic;
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
                    <BreadcrumbItem>Добавить вопрос</BreadcrumbItem>
                </Breadcrumb>
                <Heading tag="h1" class="text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl mb-4">
                    Добавить вопрос
                </Heading>
                <form class="mb-5">
                    <Label class="space-y-2 mb-2">Тема</Label>
                    <select bind:value={topic} class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg
                            focus:ring-orange-500 focus:border-orange-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600
                            dark:placeholder-gray-400 dark:text-white dark:focus:ring-orange-500 dark:focus:border-orange-500">
                        <option selected value='Общие вопросы'>Общие вопросы</option>
                        <option value='Технические вопросы'>Технические вопросы</option>
                        <option value='Административные вопросы'>Административные вопросы</option>
                    </select>
                </form>
                <div class='mb-5'>
                    <Label for="textarea-id" class="mb-2">Вопрос</Label>
                    <Textarea id="textarea-id" placeholder="Введите текст" rows="4" name="message" />
                </div>
                <div class='mb-5'>
                    <Label for="textarea-id" class="mb-2">Ответ (опционально)</Label>
                    <Textarea id="textarea-id" placeholder="Введите текст" rows="4" name="message" />
                </div>
                <div class='flex justify-center'>
                    <Button size='lg' class="mb-10">Добавить вопрос</Button>
                </div>
            </div>
        </div>
    </div>
</div>