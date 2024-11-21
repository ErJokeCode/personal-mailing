<script lang="ts">
	import { Label, Input } from 'flowbite-svelte';
	import SignIn from './utils/authentication/SignIn.svelte';
    import http from "../utility/http";
    import { admin } from "../utility/store.js";
    import { goto } from '$app/navigation';

    let email = "";
    let password = "";
    let status = http.status();

    async function login() {
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

        goto("/chat");
    }
    
	let title = 'Войти';
	let loginTitle = 'Войти в аккаунт';
</script>

<SignIn
	{title}
	{loginTitle}
	on:submit={login}
>
	<div>
		<Label for="email" class="mb-2 dark:text-white">Электронная почта</Label>
		<Input
			type="text"
			name="email"
			id="email"
			placeholder="name@company.com"
			required
			class="border outline-none dark:border-gray-600 dark:bg-gray-700"
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
		/>
	</div>
</SignIn>
