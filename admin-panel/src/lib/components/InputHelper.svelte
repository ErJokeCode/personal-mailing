<script lang="ts">
    import { Helper, Input, Label, type InputType } from "flowbite-svelte";

    interface Props {
        name: string;
        value: any;
        error: string;
        type: InputType;
        label: string;
        placeholder: string;
        required?: boolean;
        enterPressed?: () => void;
    }

    let {
        name,
        value = $bindable(),
        error = $bindable(),
        type,
        label,
        placeholder,
        required,
        enterPressed,
    }: Props = $props();

    function keypress(event) {
        if (event.key == "Enter") {
            enterPressed();
        }
    }
</script>

<div class="mb-6">
    <Label for={name} class="mb-2 text-xl">{label}</Label>
    <Input
        on:keypress={keypress}
        bind:value
        size="lg"
        id={name}
        {type}
        {name}
        {placeholder}
        {required} />

    <Helper class="mt-2 text-lg" color="red">
        {error}
    </Helper>
</div>
