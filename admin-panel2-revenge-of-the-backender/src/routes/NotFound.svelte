<script lang="ts">
    import { GeneralError, PageNotFound } from "/src/lib/errors";
    import ErrorAlert from "/src/lib/components/ErrorAlert.svelte";
</script>

<ErrorAlert class="m-4" title="404">{PageNotFound}</ErrorAlert>
