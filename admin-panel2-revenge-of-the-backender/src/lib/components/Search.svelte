<script lang="ts">
    import { Button, ButtonGroup, Input, InputAddon } from "flowbite-svelte";
    import { CloseOutline, SearchOutline } from "flowbite-svelte-icons";

    interface Props {
        search: string;
        page: number;
        value?: string;
        placeholder?: string;
        class?: string;
    }

    let {
        search = $bindable(),
        page = $bindable(),
        value = "",
        placeholder = "Поиск",
        ...restProps
    }: Props = $props();

    let internal = $state(value);

    function keypress(event) {
        if (event.key == "Enter") {
            page = 1;
            search = event.target.value;
        }
    }

    function handleSearch() {
        page = 1;
        search = internal;
    }

    function clear() {
        page = 1;
        internal = "";
        search = "";
    }
</script>

<div class={restProps.class}>
    <ButtonGroup class="w-full">
        <Button color="primary" on:click={handleSearch}>
            <SearchOutline />
        </Button>
        <Input
            bind:value={internal}
            size="lg"
            {placeholder}
            on:keypress={keypress} />
        <Button outline on:click={clear}>
            <CloseOutline />
        </Button>
    </ButtonGroup>
</div>
