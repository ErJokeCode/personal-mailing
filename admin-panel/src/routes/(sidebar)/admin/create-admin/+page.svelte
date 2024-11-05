<script lang="ts">
	import { Label, Input, Button, Helper } from 'flowbite-svelte';
	import { Breadcrumb, BreadcrumbItem, Heading } from 'flowbite-svelte';

	const labelClass = 'space-y-2 dark:text-white';

  let login_status = "";
  let create_status = "";
  let email = "";
  let password = "";

  async function create() {
      create_status = "Creating...";

      let response;

      try {
        response = await fetch('http://localhost:5000/core/admin/create', {
          method: "Post",
          body: JSON.stringify({
            password: password,
            email: email,
          }),
          credentials: "include",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
          },
        });
      } catch (err) {
        create_status = "Something went wrong! " + err;
      }

      if (response?.ok) {
        create_status = "You successfully created an admin!";
      }
    }
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800">
	<div class="p-4 px-6">
		<Breadcrumb class="mb-5">
			<BreadcrumbItem home href="/">Home</BreadcrumbItem>
			<BreadcrumbItem href="/admin">Admin</BreadcrumbItem>
			<BreadcrumbItem>Create Admin</BreadcrumbItem>
		</Breadcrumb>
		<Heading tag="h1" class="mb-5 text-xl font-semibold text-gray-900 dark:text-white sm:text-2xl">
			Create Admin
      {login_status}
		</Heading>
    <div class="mb-4">
      <Label class={labelClass}>
      <span>Email</span>
        <Input
          type="text"
          name="email"
          placeholder="name@company.com"
          required
          class="border outline-none dark:border-gray-600 dark:bg-gray-700"
          bind:value={email}
        />
      </Label>
    </div>
    <div class="mb-6">
      <Label class={labelClass}>
       <span>Password</span>
       <Input
          type="password"
          name="password"
          placeholder="••••••••"
          required
          class="border outline-none dark:border-gray-600 dark:bg-gray-700"
          bind:value={password}
        />
      </Label>
    </div>  
    <Button class="mb-2" on:click={create}>Create</Button>
    <Helper>{create_status}</Helper>
	</div>
</main>