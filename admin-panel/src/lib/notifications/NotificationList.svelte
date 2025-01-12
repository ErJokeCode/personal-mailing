<script>
	import { ToolbarButton, Indicator } from 'flowbite-svelte';
	import { BellSolid } from 'flowbite-svelte-icons';
	import Notification from './Notification.svelte';
	import { onDestroy, onMount } from 'svelte';
	import { notifications } from '../../utils/store';
	import { signal } from '../../utils/signal';

    let notificationList = notifications;
    let count = 0;

    signal.on('StudentSentMessage', handle_notification);

	onDestroy(async () => {
		signal.off('StudentSentMessage', handle_notification);
	});

	async function handle_notification(message) {
        console.log(message)
		notifications.push(message);
        notificationList = notifications
        count++;
	}

    function view() {
        let e = document.getElementById('pop');
        if (e.style.display === 'block') {
            e.style.display = 'none';
        } else {
            e.style.display = 'block';
            count = 0;
        }
    }
</script>

<ToolbarButton size="lg" class="-mx-0.5 hover:text-gray-900 dark:hover:text-white relative" on:click={view}>
	<BellSolid size="lg" />
    {#if count !== 0}
        <Indicator color="red" border size="xl" placement="top-left">
            <span class="text-white text-xs font-bold">{count}</span>
        </Indicator>
    {/if}
</ToolbarButton>
<div id='pop' role="tooltip" tabindex="-1" class="bg-white dark:bg-gray-800 text-gray-500 dark:text-gray-400 rounded-
    lg border-gray-200 dark:border-gray-700 divide-gray-200 dark:divide-gray-700 shadow-md dark:!border-gray-6
    00 max-w-sm border-0" style="position: absolute; right: 0; top: 67px; display: none; min-width: 236px">
    <div class="py-2 px-3 bg-gray-100 rounded-t-md border-b border-gray-200 dark:border-gray-600 dark:bg-gray-700">
        <div class="rounded text-center ">Уведомления</div>
    </div>
    <div class="p-0" style='max-height: 50dvh; overflow-y: auto'>
        <div class="bg-50 dark:bg-gray-700">
            {#if notificationList.length !== 0}
                {#each notificationList.toReversed() as notification}
                    <Notification
                        when={notification.message.date}
                    >
                        Новое сообщение от <span class="font-semibold text-gray-900 dark:text-white">
                            {notification.student.info.surname} {notification.student.info.name} {notification.student.info.patronymic}</span>
                        : "{notification.message.content}"
                    </Notification>
                {/each}
            {:else}
                <div class="flex gap-2 border-b px-4 py-3 dark:border-gray-600">
                    <div class="w-full pl-3">
                        <div class="mb-1.5 text-sm font-normal text-gray-500 dark:text-gray-400">Нет уведомлений</div>
                    </div>
                </div>
            {/if}
        </div>
    </div>
</div>