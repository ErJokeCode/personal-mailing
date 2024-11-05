<script>
    import FileUpload from "./FileUpload.svelte";
</script>

<main>
    <FileUpload label="Students" url="/parser/upload/students"></FileUpload>

    <hr />

    <FileUpload label="Modeus" url="/parser/upload/choice_in_modeus"
    ></FileUpload>

    <hr />

    <FileUpload label="Courses" url="/parser/upload/report_online_course"
    ></FileUpload>
</main>

<style>
</style>
