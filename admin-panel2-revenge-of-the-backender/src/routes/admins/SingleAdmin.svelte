<script lang="ts">
    import { goto, type Route } from "@mateothegreat/svelte5-router";
    import { Button, Heading, Spinner, TabItem, Tabs } from "flowbite-svelte";
    import AdminInfo from "./shared/AdminInfo.svelte";
    import { ArrowLeftOutline } from "flowbite-svelte-icons";
    import { GeneralError } from "/src/lib/errors";
    import { AdminsApi } from "/src/lib/server";

    let { route }: { route: Route } = $props();
    let adminId = route.params["adminId"];
    let returnUrl = route.query === undefined ? null : route.query["returnUrl"];
    let errorMessage = $state(GeneralError);

    async function get() {
        let res = await fetch(`${AdminsApi}/${adminId}`, {
            credentials: "include",
        });

        let body = await res.json();

        if (!res.ok) {
            errorMessage = body.detail;
            throw new Error();
        }

        return body;
    }

    async function returnBack() {
        goto(returnUrl ?? "/admins");
    }
</script>

{#await get()}
    <Spinner size="8" class="m-4" />
{:then body}
    <div class="m-4">
        {@render backButton()}

        <Heading class="inline align-middle" tag="h2">
            Профиль {body.email}
        </Heading>
    </div>

    <AdminInfo {body} />
{:catch}
    <div class="m-4">
        {@render backButton()}
    </div>

    <Heading tag="h2" class="m-4">
        {errorMessage}
    </Heading>
{/await}

{#snippet backButton()}
    <Button class="mr-2 align-middle" on:click={returnBack}>
        <ArrowLeftOutline />
    </Button>
{/snippet}
