<script lang="ts">
    import { GeneralError } from "/src/lib/errors";
    import { Button, Helper } from "flowbite-svelte";
    import Panel from "/src/lib/components/Panel.svelte";
    import { AdminsApi } from "/src/lib/server";
    import BackButton from "/src/lib/components/BackButton.svelte";
    import type { Route } from "@mateothegreat/svelte5-router";
    import InputHelper from "/src/lib/components/InputHelper.svelte";

    let form = $state({ Email: "", Password: "" });
    let errors = $state({ Email: "", Password: "" });
    let status = $state("");

    let { route }: { route: Route } = $props();

    async function create() {
        try {
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

            if (!res.ok) {
                let body = await res.json();
                errors = body.errors ?? {};
                status = body.detail;
            }
        } catch {
            status = GeneralError;
        }
    }
</script>

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

    <Button class="max-w-fit" size="lg" on:click={create}>Создать</Button>

    <Helper class="mt-2 text-lg" color="red">
        {status}
    </Helper>
</Panel>
