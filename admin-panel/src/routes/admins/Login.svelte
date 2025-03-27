<script lang="ts">
    import InputHelper from "/src/lib/components/InputHelper.svelte";
    import { Button, Card, Helper, Spinner } from "flowbite-svelte";
    import { AdminsApi } from "/src/lib/server";
    import { GeneralError } from "/src/lib/errors";

    let form = $state({ Email: "", Password: "" });
    let errors = $state({ Email: "", Password: "" });
    let status = $state("");
    let buttonText = $state("Войти");

    async function login() {
        try {
            buttonText = null;

            let res = await fetch(`${AdminsApi}/login`, {
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

            if (res.ok) {
                window.location.replace("/");
            } else {
                let body = await res.json();
                errors = body.errors ?? {};
                status = body.detail;
            }
        } catch {
            status = GeneralError;
        }

        buttonText = "Войти";
    }
</script>

<div class="flex h-full items-center justify-center">
    <Card class="max-w-lg">
        <InputHelper
            enterPressed={login}
            bind:error={errors["Email"]}
            bind:value={form["Email"]}
            label="Почта"
            name="Email"
            placeholder="example@mail.com"
            type="email"
            required />

        <InputHelper
            enterPressed={login}
            bind:error={errors["Password"]}
            bind:value={form["Password"]}
            label="Пароль"
            name="Password"
            placeholder="••••••••"
            type="password"
            required />

        <Button class="max-w-fit" size="lg" on:click={login}>
            {#if buttonText}
                {buttonText}
            {:else}
                <Spinner size="6" color="white" />
            {/if}
        </Button>

        <Helper class="mt-2 text-lg" color="red">
            {status}
        </Helper>
    </Card>
</div>
