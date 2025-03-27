<script lang="ts">
    import BackButton from "/src/lib/components/BackButton.svelte";
    import { type Route } from "@mateothegreat/svelte5-router";
    import { Heading, Spinner } from "flowbite-svelte";
    import AdminInfo from "./shared/AdminInfo.svelte";
    import { GeneralError } from "/src/lib/errors";
    import { AdminsApi } from "/src/lib/server";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
    import Get from "/src/lib/components/Get.svelte";

    let { route }: { route: Route } = $props();
    let adminId = route.params["adminId"];
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
</script>

<BackButton class="inline-block m-4 mr-0 align-middle" />

<Get {get} bind:error={errorMessage}>
    {#snippet children(body)}
        <Heading class="inline align-middle" tag="h2">
            Профиль {body.email}
        </Heading>

        <AdminInfo {body} />
    {/snippet}
</Get>
