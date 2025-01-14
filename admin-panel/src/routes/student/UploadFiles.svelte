<script lang="ts">
    import { Breadcrumb, BreadcrumbItem, Fileupload, Label, Button, Helper, Heading, A, Pagination } from 'flowbite-svelte';
    import { TableHeadCell, Table, TableBody, TableBodyCell, TableBodyRow, TableHead } from 'flowbite-svelte';
    import { onMount } from "svelte";
    import http from "../../utils/http";
    import { navigate } from "svelte-routing";
  
    let student_files;
    let student_success = "";
    let modeus_files;
    let modeus_success = "";
    let courses_files;
    let courses_success = "";

    let status = http.status();
    let history = [];
    let limit = -1;

    onMount(async () => {
        status = status.start_load();
        history = (await http.get("/parser/upload/history?limit=" + limit.toString(), status)) ?? [];
        status = status.end_load();
    });

    async function download_file(link) {
        navigate(link);
    }
  
      async function send_students() {
      if (student_files.length <= 0) return;
  
      student_success = "Отправка...";
  
      let file = student_files[0];
  
      var data = new FormData();
      data.append("file", file);
  
      let result = await fetch('http://localhost:5000/parser/upload/student', {
        method: "POST",
        body: data,
        credentials: "include",
      });
  
      if (result.ok) {
        student_success = "Загружено";
      } else {
        student_success = "Ошибка";
      }
    }
  
    async function send_modeus() {
          if (modeus_files.length <= 0) return;
  
          modeus_success = "Отправка...";
  
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
              modeus_success = "Загружено";
          } else {
              modeus_success = "Ошибка";
          }
      }
  
      async function send_courses() {
          if (courses_files.length <= 0) return;
  
          courses_success = "Отправка...";
  
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
              courses_success = "Загружено";
          } else {
              courses_success = "Ошибка";
          }
      }



let helper = { start: 1, end: 10, total: 100 };

const previous = () => {
    if (helper.start == 1) return
    helper.start -= 10
};
const next = () => {
    if (helper.end == helper.total) return
    helper.start += 10
};
  </script>

<div class="overflow-hidden lg:flex">
    <div class="relative h-full w-full overflow-y-auto lg:ml-64 pt-[70px]">
        <div class="relative h-full w-full overflow-y-auto bg-white dark:bg-gray-800 p-4 px-6">
            <div class="grid grid-cols-1 space-y-2 dark:bg-gray-800 xl:grid-cols-3 xl:gap-3.5">
                <div class="col-span-full xl:mb-0">
                    <Breadcrumb class="mb-6">
                        <BreadcrumbItem home href="/">Главная</BreadcrumbItem>
                        <BreadcrumbItem>Загрузить файлы</BreadcrumbItem>
                    </Breadcrumb>
                </div>
            </div>
            <div class="space-y-2 mb-6">
                <div style="display: flex; flex-direction: row; gap: 30px; align-items: baseline; margin-bottom: 20px">
                    <Heading tag="h3" class="w-100 text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-s">Студенты</Heading>
                    <A class="font-medium hover:underline text-sm" for="larg_size" href="http://localhost:5000/parser/upload/student/example">Скачать пример</A>
                </div>
                <Fileupload value="" bind:files={student_files} id="larg_size" size="lg" />
                <Helper>{student_success}</Helper>
                <Button on:click={send_students}>Загрузить</Button>
            </div>
            <div class="space-y-2 mb-6">
                <div style="display: flex; flex-direction: row; gap: 30px; align-items: baseline; margin-bottom: 20px">
                    <Heading tag="h3" class="w-100 text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-l">Модеус</Heading>
                    <A class="font-medium hover:underline text-sm" for="larg_size" href="http://localhost:5000/parser/upload/choice_in_modeus/example">Скачать пример</A>
                </div>
                <Fileupload value="" bind:files={modeus_files} id="larg_size" size="lg" />
                <Helper>{modeus_success}</Helper>
                <Button on:click={send_modeus}>Загрузить</Button>
            </div>
            <div class="space-y-2 mb-6">
                <div style="display: flex; flex-direction: row; gap: 30px; align-items: baseline; margin-bottom: 20px">
                    <Heading tag="h3" class="w-100 text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-l">Курсы</Heading>
                    <A class="font-medium hover:underline text-sm" for="larg_size" href="http://localhost:5000/parser/upload/report_online_course/example">Скачать пример</A>
                </div>
                <Fileupload value="" bind:files={courses_files} id="larg_size" size="lg" />
                <Helper>{courses_success}</Helper>
                <Button on:click={send_courses}>Загрузить</Button>
            </div>
            <Heading tag="h2" class="text-xl font-semibold text-gray-900 dark:text-gray-100 sm:text-2xl">
                История
            </Heading>
        </div>
        <Table hoverable={true} class="mb-5">
            <TableHead>
              <TableHeadCell class="px-8">Название файла</TableHeadCell>
              <TableHeadCell class="px-8 w-1/6" defaultSort>Дата загрузки</TableHeadCell>
              <TableHeadCell class="px-8">Тип</TableHeadCell>
              <TableHeadCell class="px-8">Статус загрузки</TableHeadCell>
              <TableHeadCell class="px-8">Скачать</TableHeadCell>
            </TableHead>
            <TableBody>
                {#each history as history_item}
                    <TableBodyRow
                        role="link"
                        class="contrast">
                        <TableBodyCell class="px-8">{history_item.name_file}</TableBodyCell>
                        <TableBodyCell class="px-8">{history_item.date.slice(0, 10) + " " + history_item.date.slice(11, 19)}</TableBodyCell>
                        <TableBodyCell class="px-8">{history_item.type == "student" ? "Загрузка студентов" : history_item.type == "modeus" ? "Выгрузка модеус" : "Онлайн курс"}</TableBodyCell>
                        <TableBodyCell class="px-8">{history_item.status_upload == null ? "Загружается" : history_item.status_upload}</TableBodyCell>
                        <TableBodyCell class="px-8">
                            <A class='flex ml-2' href={history_item.link}>Скачать</A>
                        </TableBodyCell>
                    </TableBodyRow>
              {/each}
            </TableBody>
        </Table>
        <!-- <div class="flex flex-col items-center justify-center gap-2">
            <div class="text-sm text-gray-700 dark:text-gray-400">
              Showing <span class="font-semibold text-gray-900 dark:text-white">{helper.start}</span>
              to
              <span class="font-semibold text-gray-900 dark:text-white">{helper.end}</span>
              of
              <span class="font-semibold text-gray-900 dark:text-white">{helper.total}</span>
              Entries
            </div>
          
            <Pagination table large>
              <span slot="prev">Prev</span>
            </Pagination>
        </div> -->
    </div>
</div>