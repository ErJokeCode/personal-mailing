<script lang="ts">
    import { Label, Input, Button, Card } from "flowbite-svelte";
    import http from "../../utils/http";
    import { admin } from "../../utils/store.js";
    import { navigate } from "svelte-routing";
    import { signal } from "../../utils/signal.js";

    let email = "";
    let password = "";
    let status = http.status();

    async function login() {
        if (email == "" || password == "") return;
        status = status.start_load();

        await http.post_json(
            "/core/admins/login",
            {
                email,
                password,
            },
            status,
        );
        status = status.end_load();

        let the_admin = await http.get("/core/admins/me", http.status());
        admin.update((_) => the_admin);

        await signal.stop();
        await signal.start();

        if (status.value === "✓") navigate("/profile");
    }

    async function handle_keypress(event) {
        if (event.key == "Enter") {
            login();
        }
    }
</script>

<main class="bg-gray-50 dark:bg-gray-900 w-full">
    <div
        class="flex flex-col items-center justify-center px-6 pt-8 mx-auto md:h-screen pt:mt-0 dark:bg-gray-900"
    >
        <Card class="w-full" size="md" border={false}>
            <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
                Войти
            </h1>
            <form class="mt-8 space-y-6">
                <div>
                    <Label for="email" class="mb-2 dark:text-white"
                        >Электронная почта</Label
                    >
                    <Input
                        type="text"
                        name="email"
                        id="email"
                        placeholder="name@company.com"
                        required
                        class="border outline-none dark:border-gray-600 dark:bg-gray-700"
                        bind:value={email}
                        on:keypress={handle_keypress}
                    />
                </div>
                <div>
                    <Label for="password" class="mb-2 dark:text-white"
                        >Пароль</Label
                    >
                    <Input
                        type="password"
                        name="password"
                        id="password"
                        placeholder="••••••••"
                        required
                        class="border outline-none dark:border-gray-600 dark:bg-gray-700"
                        bind:value={password}
                        on:keypress={handle_keypress}
                    />
                </div>
                <Button onclick={login} size="lg"
                    >{status.value} Войти в аккаунт</Button
                >
            </form>
        </Card>
    </div>
</main>
