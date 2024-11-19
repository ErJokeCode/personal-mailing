<script lang="ts">
  import thickbars from '../graphs/thickbars';
  import ChartWidget from '../widgets/ChartWidget.svelte';
  import type { PageData } from '../../(sidebar)/$types';
  import users from '../graphs/users';
  import DarkChart from '../widgets/DarkChart.svelte';
  import { onMount } from 'svelte';
  import getChartOptions from '../../(sidebar)/dashboard/chart_options';
  import Chat from './Chat.svelte';

  export let data: PageData;

  let chartOptions = getChartOptions(false);
  chartOptions.series = data.series;

  let dark = false;

  function handler(ev: Event) {
    if ('detail' in ev && typeof ev.detail === 'boolean') {
      chartOptions = getChartOptions(ev.detail);
      chartOptions.series = data.series;
      dark = !!ev.detail;
    }
  }

  onMount(() => {
    document.addEventListener('dark', handler);
    return () => document.removeEventListener('dark', handler);
  });
</script>

<div class="mt-px space-y-4 flex items-center">
    <Chat />
</div>
