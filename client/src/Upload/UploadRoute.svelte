<script>
    import FileUpload from "./FileUpload.svelte";
</script>

<main>
    <FileUpload label="Students" url="/parser/upload/student" />

    <hr />

    <FileUpload label="Modeus" url="/parser/upload/choice_in_modeus" />

    <hr />

    <FileUpload label="Courses" url="/parser/upload/report_online_course" />
</main>

<style>
</style>
