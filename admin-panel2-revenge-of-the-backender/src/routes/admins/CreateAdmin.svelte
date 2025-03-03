<script lang="ts">
    import { GeneralError } from "/src/lib/errors";
    import { Button, Helper, Spinner, Toast } from "flowbite-svelte";
    import Panel from "/src/lib/components/Panel.svelte";
    import { AdminsApi } from "/src/lib/server";
    import BackButton from "/src/lib/components/BackButton.svelte";
    import {
        goto,
        QueryString,
        type Route,
    } from "@mateothegreat/svelte5-router";
    import InputHelper from "/src/lib/components/InputHelper.svelte";
    import ToastNotifications from "/src/lib/components/ToastNotifications.svelte";

    let notifications: ToastNotifications;
    let form = $state({ Email: "", Password: "" });
    let errors = $state({ Email: "", Password: "" });

    let status = $state("");
    let buttonText = $state("Создать");

    let { route }: { route: Route } = $props();

    function clear() {
        form = { Email: "", Password: "" };
        errors = { Email: "", Password: "" };
        status = "";
        buttonText = "Создать";
    }

    async function create() {
        try {
            buttonText = null;

            let res = await fetch(`${AdminsApi}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify({
                    email: form["Email"],
                    password: form["Password"],
                }),
            });

            let body = await res.json();

            if (!res.ok) {
                errors = body.errors ?? {};
                status = body.detail;
            } else {
                notifications.add({
                    type: "ok",
                    text: `Админ ${body.email} создан`,
                    content: notification,
                    params: body.id,
                    autohide: false,
                });

                clear();
            }
        } catch {
            status = GeneralError;
        }

        buttonText = "Создать";
    }

    function single(id) {
        let query = new QueryString();
        query.set("returnUrl", window.location.pathname);

        goto(`/admins/${id}?${query.toString()}`);
    }
</script>

<ToastNotifications bind:this={notifications} />

{#snippet notification(id)}
    <Button color="green" size="sm" on:click={() => single(id)}>Профиль</Button>
{/snippet}

<BackButton fallback="/admins" title="Создать админа" {route} class="m-4" />

<Panel class="m-4">
    <InputHelper
        enterPressed={create}
        bind:error={errors["Email"]}
        bind:value={form["Email"]}
        label="Почта"
        name="Email"
        placeholder="example@mail.com"
        type="email"
        required />

    <InputHelper
        enterPressed={create}
        bind:error={errors["Password"]}
        bind:value={form["Password"]}
        label="Пароль"
        name="Password"
        placeholder="••••••••"
        type="password"
        required />

    <Button class="max-w-fit" size="lg" on:click={create}>
        {#if buttonText}
            {buttonText}
        {:else}
            <Spinner size="6" color="white" />
        {/if}
    </Button>

    <Helper class="mt-2 text-lg" color="red">
        {status}
    </Helper>
</Panel>
