<script lang="ts">
    import {
        Button,
        Card,
        Helper,
        Input,
        Label,
        type InputType,
    } from "flowbite-svelte";
    import { serverUrl } from "/src/stores/server.svelte";

    let form = $state({ Email: "", Password: "" });
    let errors = $state({ Email: "", Password: "" });
    let status = $state("");

    async function keypress(event) {
        if (event.key == "Enter") {
            await login();
        }
    }

    async function login() {
        try {
            let res = await fetch(`${serverUrl}/core/admins/login`, {
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
            status = "Что-то пошло не так...";
        }
    }
</script>

<div class="flex h-full items-center justify-center">
    <Card class="max-w-lg">
        {@render input("Email", "email", "Почта", "example@mail.com")}
        {@render input("Password", "password", "Пароль", "••••••••")}

        <Button class="max-w-fit" size="lg" on:click={login}>Войти</Button>

        <Helper class="mt-2 text-lg" color="red">
            {status}
        </Helper>
    </Card>
</div>

{#snippet input(
    name: string,
    type: InputType,
    content: string,
    placeholder: string,
)}
    <div class="mb-6">
        <Label for={name} class="mb-2 text-xl">{content}</Label>
        <Input
            on:keypress={keypress}
            bind:value={form[name]}
            size="lg"
            id={name}
            {type}
            {name}
            {placeholder}
            required />

        <Helper class="mt-2 text-lg" color="red">
            {errors[name]}
        </Helper>
    </div>
{/snippet}
