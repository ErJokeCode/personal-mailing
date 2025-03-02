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
    import { CheckOutline } from "flowbite-svelte-icons";

    let form = $state({ Email: "", Password: "" });
    let errors = $state({ Email: "", Password: "" });
    let status = $state("");
    let createdId: string = $state(null);

    let buttonText = $state("Создать");

    let { route }: { route: Route } = $props();

    function clear() {
        form = { Email: "", Password: "" };
        errors = { Email: "", Password: "" };
        status = "";
        buttonText = "Создать";
    }

    const delay = (delayInms) => {
        return new Promise((resolve) => setTimeout(resolve, delayInms));
    };

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
                createdId = body.id;
                clear();
            }
        } catch {
            status = GeneralError;
        }

        buttonText = "Создать";
    }

    function single() {
        let query = new QueryString();
        query.set("returnUrl", "/create-admin");

        goto(`/admins/${createdId}?${query.toString()}`);
    }
</script>

{#if createdId}
    <Toast position="bottom-right" align={false} color="green">
        <CheckOutline slot="icon" />
        <div class="font-semibold text-md mb-3">Админ создан успешно</div>
        <Button color="green" size="sm" on:click={single}>Профиль</Button>
    </Toast>
{/if}

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
