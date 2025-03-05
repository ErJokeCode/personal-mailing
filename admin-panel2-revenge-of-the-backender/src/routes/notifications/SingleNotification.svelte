<script lang="ts">
    import BackButton from "/src/lib/components/BackButton.svelte";
    import {
        goto,
        QueryString,
        type Route,
    } from "@mateothegreat/svelte5-router";
    import { GeneralError } from "/src/lib/errors";
    import {
        AdminsApi,
        DocumentsApi,
        NotificationsApi,
        StudentsApi,
    } from "/src/lib/server";
    import Get from "/src/lib/components/Get.svelte";
    import { A, Button, Heading, TabItem, Tabs } from "flowbite-svelte";

    let { route }: { route: Route } = $props();
    let notificationId = route.params["notificationId"];
    let errorMessage = $state(GeneralError);

    async function get() {
        let res = await fetch(`${NotificationsApi}/${notificationId}`, {
            credentials: "include",
        });

        let body = await res.json();

        if (!res.ok) {
            errorMessage = body.detail;
            throw new Error();
        }

        return body;
    }

    function singleAdmin(adminId) {
        goto(`/admins/${adminId}`);
    }

    function singleStudent(email) {
        goto(`/students/${email}`);
    }
</script>

<BackButton class="inline-block m-4 mr-0 align-middle" />

<Get {get} bind:error={errorMessage}>
    {#snippet children(body)}
        <Heading class="inline align-middle" tag="h2">
            Рассылка {body.id}
        </Heading>

        <Tabs
            tabStyle="underline"
            class="ml-4"
            contentClass="p-4 bg-white rounded-lg dark:bg-gray-800 m-4">
            <TabItem open title="Информация">
                <p
                    class={"text-xl p-4" +
                        (body.errors.length > 0
                            ? " text-red-700 dark:text-red-400"
                            : " text-green-700 dark:text-green-400")}>
                    <b>Статус:</b>
                    {body.errors.length > 0 ? "Плохо" : "Успех"}
                </p>
                <p class="dark:text-white text-xl p-4">
                    <b>Содержание:</b>
                    {body.content}
                </p>
                <p class="dark:text-white text-xl p-4">
                    <b>Админ:</b>
                    <Button on:click={() => singleAdmin(body.admin.id)}>
                        {body.admin.email}
                    </Button>
                </p>
                <p class="dark:text-white text-xl p-4">
                    <b>Дата:</b>
                    {body.createdAt}
                </p>
            </TabItem>

            {#if body.documents.length > 0}
                <TabItem title="Вложения">
                    <div class="flex gap-1 flex-col items-baseline">
                        {#each body.documents as document}
                            <A href={`${DocumentsApi}/${document.blobId}`}>
                                {document.name}
                            </A>
                        {/each}
                    </div>
                </TabItem>
            {/if}

            <TabItem title="Студенты">
                <ul class="flex flex-col gap-4">
                    {#each body.students as student}
                        <li>
                            <Button
                                on:click={() => singleStudent(student.email)}>
                                {student.email}
                            </Button>
                        </li>
                    {/each}
                </ul>
            </TabItem>

            {#if body.errors.length > 0}
                <TabItem title="Ошибки!">
                    <ul class="flex flex-col gap-4">
                        {#each body.errors as error}
                            <li>
                                <p class="dark:text-white text-lg">
                                    {error.message}
                                </p>
                            </li>
                        {/each}
                    </ul>
                </TabItem>
            {/if}
        </Tabs>
    {/snippet}
</Get>
