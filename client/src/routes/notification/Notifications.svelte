<script>
    import { onMount } from "svelte";
    import http from "src/utility/http";
    import { Link, navigate } from "svelte-routing";

    let status = http.status();
    let notifications = [];

    onMount(async () => {
        status = status.start_load();
        notifications =
            (await http.get("/core/admin/notifications", status)).items ?? [];
        status = status.end_load();
    });

    async function fullInfo(id) {
        navigate(`/notification/${id}`);
    }
</script>

<h2>Мои рассылки</h2>

<table>
    <thead>
        <tr>
            <th>Содержание</th>
            <th>Дата</th>
            <th>Отправлено</th>
        </tr>
    </thead>
    <tbody>
        {#each notifications as notification}
            <tr
                role="link"
                class="contrast"
                on:click={() => fullInfo(notification.id)}
            >
                <th>{notification.content}</th>
                <th>{notification.date}</th>
                <th>
                    {#each notification.students as student}
                        {student.email},
                    {/each}
                </th>
            </tr>
        {/each}
    </tbody>
</table>

<style>
</style>
