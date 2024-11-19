<script lang="ts">
  import { Breadcrumb, BreadcrumbItem, Fileupload, Label, Button, Helper } from 'flowbite-svelte';

  let student_files: FileList;
  let student_success = "";
  let modeus_files: FileList;
  let modeus_success = "";
  let courses_files: FileList;
  let courses_success = "";

	async function send_students() {
    if (student_files.length <= 0) return;

    student_success = "Sending...";

    let file = student_files[0];

    var data = new FormData();
    data.append("file", file);

    let result = await fetch('http://localhost:5000/parser/upload/student', {
      method: "POST",
      body: data,
      credentials: "include",
    });

    if (result.ok) {
      student_success = "Success";
    } else {
      student_success = "Error";
    }
  }

  async function send_modeus() {
        if (modeus_files.length <= 0) return;

        modeus_success = "Sending...";

        let file = modeus_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch(
            'http://localhost:5000/parser/upload/choice_in_modeus',
            {
                method: "POST",
                body: data,
                credentials: "include",
            },
        );

        if (result.ok) {
            modeus_success = "Success";
        } else {
            modeus_success = "Error";
        }
    }

    async function send_courses() {
        if (courses_files.length <= 0) return;

        courses_success = "Sending...";

        let file = courses_files[0];

        var data = new FormData();
        data.append("file", file);

        let result = await fetch(
            'http://localhost:5000/parser/upload/report_online_course',
            {
                method: "POST",
                body: data,
                credentials: "include",
            },
        );

        if (result.ok) {
			courses_success = "Success";
        } else {
			courses_success = "Error";
        }
    }
</script>

<main class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800 p-4 px-6">
	<div class="grid grid-cols-1 space-y-2 dark:bg-gray-800 xl:grid-cols-3 xl:gap-3.5">
		<div class="col-span-full xl:mb-0">
			<Breadcrumb class="mb-6">
				<BreadcrumbItem home href="/">Главная</BreadcrumbItem>
				<BreadcrumbItem>Загрузить файлы</BreadcrumbItem>
			</Breadcrumb>
		</div>
	</div>
	<div class="space-y-2 mb-5">
		<Label for="larg_size">Студенты</Label>
		<Fileupload value="" bind:files={student_files} id="larg_size" size="lg" />
		<Helper>{student_success}</Helper>
		<Button on:click={send_students}>Загрузить</Button>
	</div>
		<div class="space-y-2 mb-5">
		<Label for="larg_size">Модеус</Label>
		<Fileupload value="" bind:files={modeus_files} id="larg_size" size="lg" />
		<Helper>{modeus_success}</Helper>
		<Button on:click={send_modeus}>Загрузить</Button>
	</div>
	<div class="space-y-2 mb-5">
		<Label for="larg_size">Курсы</Label>
		<Fileupload value="" bind:files={courses_files} id="larg_size" size="lg" />
		<Helper>{courses_success}</Helper>
		<Button on:click={send_courses}>Загрузить</Button>
	</div>
</main>