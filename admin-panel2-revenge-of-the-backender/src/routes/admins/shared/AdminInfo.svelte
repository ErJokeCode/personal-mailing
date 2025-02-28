<script lang="ts">
    import { TabItem, Tabs } from "flowbite-svelte";

    let { body } = $props();
</script>

<Tabs
    tabStyle="underline"
    class="ml-4"
    contentClass="p-4 bg-white rounded-lg dark:bg-gray-800 m-4">
    <TabItem open title="Информация">
        <p class="dark:text-white text-xl p-4">
            <b>Почта:</b>
            {body.email}
        </p>
        <p class="dark:text-white text-xl p-4">
            <b>Дата:</b>
            {body.createdAt}
        </p>
    </TabItem>

    <TabItem title="Группы">
        <ul>
            {#each ["100", "200", "300"] as group}
                <li class="dark:text-white text-xl p-4">{group}</li>
            {/each}
        </ul>
    </TabItem>
</Tabs>
