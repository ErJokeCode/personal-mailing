<script lang="ts">
    import { Card, Indicator, Popover, ToolbarButton } from "flowbite-svelte";
    import { signal } from "/src/lib/utils/signal";
    import { BellSolid } from "flowbite-svelte-icons";
    import { ChatsApi, PageSize } from "../lib/server";
    import { onDestroy, onMount } from "svelte";
    import { goto } from "@mateothegreat/svelte5-router";

    let chats = $state([]);

    signal.on("ChatReadReceived", getUnread);
    signal.on("MessageReceived", getUnread);

    onMount(async () => {
        await getUnread();
    });

    onDestroy(() => {
        signal.off("ChatReadReceived", getUnread);
        signal.off("MessageReceived", getUnread);
    });

    async function getUnread() {
        let url = new URL(ChatsApi);
        url.searchParams.set("OnlyUnread", "true");
        url.searchParams.set("PageSize", PageSize.toString());

        let res = await fetch(url, {
            credentials: "include",
        });

        let body = await res.json();

        chats = body.items;
    }

    function toChat(studentId) {
        goto(`chats/${studentId}`);
    }
</script>

<ToolbarButton class="mr-2 relative" id="bell">
    <BellSolid size="lg" />

    {#if chats.length > 0}
        <Indicator color="red" border size="xl" placement="top-right">
            <span class="text-white text-xs font-bold">{chats.length}</span>
        </Indicator>
    {/if}
</ToolbarButton>

<Popover title="Уведомления" triggeredBy="#bell" trigger="click">
    {#if chats.length <= 0}
        Нет уведомлений
    {:else}
        <div class="flex flex-col gap-2">
            {#each chats as chat}
                <Card
                    on:click={() => toChat(chat.student.id)}
                    class="hover:bg-gray-100 dark:hover:bg-gray-600 hover:cursor-pointer">
                    <div><strong>{chat.student.email}:</strong></div>
                    <p>
                        "{chat.messages[0].content.length > 20
                            ? chat.messages[0].content.slice(0, 20)
                            : chat.messages[0].content}"
                    </p>
                </Card>
            {/each}
        </div>
    {/if}
</Popover>
