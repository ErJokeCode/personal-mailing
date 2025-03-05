<script lang="ts">
    import { PageNotFound } from "/src/lib/errors";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
    import BackButton from "/src/lib/components/BackButton.svelte";
</script>

<BackButton class="m-4 mr-0" />

<ErrorAlert class="m-4" title="404">{PageNotFound}</ErrorAlert>
