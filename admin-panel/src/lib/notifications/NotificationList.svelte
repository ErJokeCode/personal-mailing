<script>
    import { ToolbarButton, Indicator } from "flowbite-svelte";
    import { BellSolid } from "flowbite-svelte-icons";
    import Notification from "./Notification.svelte";
    import { onDestroy, onMount } from "svelte";
    import { signal } from "../../utils/signal";
    import http from "../../utils/http";
    import { navigate } from "svelte-routing";

    let count = 0;
    let chats = [];

    getUnreadChats();

    signal.on("MessageReceived", handleMessage);
    signal.on("ChatReadReceived", handleMessage);

    onDestroy(async () => {
        signal.off("MessageReceived", handleMessage);
        signal.off("ChatReadReceived", handleMessage);
    });

    async function handleMessage(message) {
        getUnreadChats();
    }

    function getUnreadChats() {
        http.get("/core/chats", http.status()).then((result) => {
            chats = result.items.filter((ch) => ch.unreadCount > 0);
            count = chats.length;
        });
    }

    function view() {
        let e = document.getElementById("pop");
        if (e.style.display === "block") {
            e.style.display = "none";
        } else {
            e.style.display = "block";
        }
    }

    async function handleSelect(studentId) {
        view();
        navigate("/chat/" + studentId);
    }
</script>

<ToolbarButton
    size="lg"
    class="-mx-0.5 hover:text-gray-900 dark:hover:text-white relative"
    on:click={view}
>
    <BellSolid size="lg" />
    {#if count !== 0}
        <Indicator color="red" border size="xl" placement="top-left">
            <span class="text-white text-xs font-bold">{count}</span>
        </Indicator>
    {/if}
</ToolbarButton>
<div
    id="pop"
    role="tooltip"
    tabindex="-1"
    class="bg-white dark:bg-gray-800 text-gray-500 dark:text-gray-400 rounded-
    lg border-gray-200 dark:border-gray-700 divide-gray-200 dark:divide-gray-700 shadow-md dark:!border-gray-6
    00 max-w-sm border-0"
    style="position: absolute; right: 0; top: 67px; display: none; min-width: 236px"
>
    <div
        class="py-2 px-3 bg-gray-100 rounded-t-md border-b border-gray-200 dark:border-gray-600 dark:bg-gray-700"
    >
        <div class="rounded text-center">Уведомления</div>
    </div>
    <div class="p-0" style="max-height: 50dvh; overflow-y: auto">
        <div class="bg-50 dark:bg-gray-700">
            {#if chats.length !== 0}
                {#each chats as chat}
                    <Notification
                        when={chat.messages[0].date}
                        on:click={() => handleSelect(chat.student.id)}
                    >
                        Новое сообщение от <span
                            class="font-semibold text-gray-900 dark:text-white"
                        >
                            {chat.student.email}</span
                        >
                        : "{chat.messages[0].content}"
                    </Notification>
                {/each}
            {:else}
                <div class="flex gap-2 border-b px-4 py-3 dark:border-gray-600">
                    <div class="w-full pl-3">
                        <div
                            class="mb-1.5 text-sm font-normal text-gray-500 dark:text-gray-400"
                        >
                            Нет уведомлений
                        </div>
                    </div>
                </div>
            {/if}
        </div>
    </div>
</div>
