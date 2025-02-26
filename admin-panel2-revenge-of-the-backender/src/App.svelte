<script lang="ts">
  import { Router, type Route } from "@mateothegreat/svelte5-router";
  import Header from "./Header.svelte";
  import Sidebar from "./Sidebar.svelte";
  import Profile from "./routes/admins/Profile.svelte";

  const routes: Route[] = [
    {
      path: "",
    },
    {
      path: "/profile",
      component: Profile,
    },
  ];
</script>

<div class="h-full flex flex-col">
  <Header />

  <div class="flex overflow-hidden">
    <Sidebar />

    <div class="flex-1 overflow-scroll">
      <Router basePath="/" {routes} />
    </div>
  </div>
</div>
