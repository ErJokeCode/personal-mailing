<script>
    import { Link } from "svelte-routing";
    import Accordion from "src/components/Accordion.svelte";
</script>

<aside class="pico-background-slate-50">
    <nav class="container">
        <Accordion summary="Студенты">
            <li><Link to="/student/upload">Загрузить файлы</Link></li>
            <li><Link to="/student">Все студенты</Link></li>
            <li><Link to="/student/active">Активные студенты</Link></li>
        </Accordion>

        <Accordion summary="Рассылки">
            <li>
                <Link to="/notification/send">Отправить рассылку</Link>
                <Link to="/notification">Мои рассылки</Link>
            </li>
        </Accordion>

        <Accordion summary="Чаты">
            <li><Link to="/chat">Мои чаты</Link></li>
        </Accordion>

        <Accordion summary="Админы">
            <li><Link to="/admin">Все админы</Link></li>
            <li>
                <Link to="/admin/create">Создать админа</Link>
            </li>
        </Accordion>
    </nav>
</aside>

<style>
    aside {
        padding: 1.2rem;
        height: 100%;
        overflow: scroll;
    }
</style>
