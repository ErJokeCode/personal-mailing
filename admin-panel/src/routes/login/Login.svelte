<script lang="ts">
	import { Label, Input, Button, Card } from 'flowbite-svelte';
    import http from "../../utils/http";
    import { admin } from "../../utils/store.js";
    import { navigate } from 'svelte-routing';

    let email = "";
    let password = "";
    let status = http.status();

    async function login() {
        if (email == '' || password == '') return;
        status = status.start_load();

        await http.post_json(
            "/login",
            {
                email,
                password,
            },
            status,
        );
        status = status.end_load();

        let the_admin = await http.get("/core/admin/me", http.status());
        admin.update((_) => the_admin);
        
        if (status.value === "✓") navigate("/profile");
    }
</script>

<main class='bg-gray-50 dark:bg-gray-900 w-full'>
	<div class='flex flex-col items-center justify-center px-6 pt-8 mx-auto md:h-screen pt:mt-0 dark:bg-gray-900'>
		<Card class="w-full" size="md" border={false}>
			<h1 class='text-2xl font-bold text-gray-900 dark:text-white'>Войти</h1>
            <form class="mt-8 space-y-6">
	            <div>
		            <Label for="email" class="mb-2 dark:text-white">Электронная почта</Label>
		            <Input
			            type="text"
			            name="email"
			            id="email"
			            placeholder="name@company.com"
			            required
			            class="border outline-none dark:border-gray-600 dark:bg-gray-700"
                        bind:value={email}
		            />
	            </div>
	            <div>
		            <Label for="password" class="mb-2 dark:text-white">Пароль</Label>
		            <Input
			            type="password"
			            name="password"
			            id="password"
			            placeholder="••••••••"
			            required
			            class="border outline-none dark:border-gray-600 dark:bg-gray-700"
                        bind:value={password}
		            />
	            </div>
				<Button onclick={login} size="lg">{status.value} Войти в аккаунт</Button>
			</form>
		</Card>
	</div>
</main>