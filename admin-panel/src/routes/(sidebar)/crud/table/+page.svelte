<script>
    import { onMount } from 'svelte';
    import { Table, TableBody, TableBodyCell, TableBodyRow, TableHead, TableHeadCell, TableSearch, Button, Dropdown, DropdownItem, Checkbox, ButtonGroup } from 'flowbite-svelte';
    import { Section } from 'flowbite-svelte-blocks';
    import { PlusOutline, ChevronDownOutline, FilterSolid, ChevronRightOutline, ChevronLeftOutline } from 'flowbite-svelte-icons';
  
    let divClass='bg-white dark:bg-gray-800 relative shadow-md sm:rounded-lg overflow-hidden';
    let innerDivClass='flex flex-col md:flex-row items-center justify-between space-y-3 md:space-y-0 md:space-x-4 p-4';
    let searchClass='w-full md:w-1/2 relative';
    let svgDivClass='absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none';
    let classInput="text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2  pl-10";
  
    let searchTerm = '';
    let currentPosition = 0;
    const itemsPerPage = 10;
    const showPage = 5;
    let totalPages = 0;
    /**
	 * @type {any[]}
	 */
    let pagesToShow = [];
    let paginationData = [
        { id: 1, maker: 'Toyota', type: 'ABC', make: 2017 },
        { id: 2, maker: 'Ford', type: 'CDE', make: 2018 },
        { id: 3, maker: 'Volvo', type: 'FGH', make: 2019 },
        { id: 4, maker: 'Saab', type: 'IJK', make: 2020 },
        { id: 5, maker: 'Toyota', type: 'ABC', make: 2017 },
        { id: 6, maker: 'Ford', type: 'CDE', make: 2018 },
        { id: 7, maker: 'Volvo', type: 'FGH', make: 2019 },
        { id: 8, maker: 'Saab', type: 'IJK', make: 2020 },
        { id: 9, maker: 'Toyota', type: 'ABC', make: 2017 },
        { id: 10, maker: 'Ford', type: 'CDE', make: 2018 },
        { id: 11, maker: 'Volvo', type: 'FGH', make: 2019 },
        { id: 12, maker: 'Saab', type: 'IJK', make: 2020 },
        { id: 13, maker: 'Toyota', type: 'ABC', make: 2017 },
        { id: 14, maker: 'Ford', type: 'CDE', make: 2018 },
        { id: 15, maker: 'Volvo', type: 'FGH', make: 2019 },
        { id: 16, maker: 'Saab', type: 'IJK', make: 2020 },
        { id: 17, maker: 'Toyota', type: 'ABC', make: 2017 },
        { id: 19, maker: 'Ford', type: 'CDE', make: 2018 },
        { id: 20, maker: 'Volvo', type: 'FGH', make: 2019 },
        { id: 21, maker: 'Saab', type: 'IJK', make: 2020 },
        { id: 22, maker: 'Toyota', type: 'ABC', make: 2017 },
        { id: 23, maker: 'Ford', type: 'CDE', make: 2018 },
        { id: 24, maker: 'Volvo', type: 'FGH', make: 2019 },
        { id: 25, maker: 'Saab', type: 'IJK', make: 2020 }
    ];
    let totalItems = paginationData.length;
    /**
	 * @type {number}
	 */
    let startPage;
    // @ts-ignore
    /**
	 * @type {number}
	 */
    let endPage;
  
    const updateDataAndPagination = () => {
      const currentPageItems = paginationData.slice(currentPosition, currentPosition + itemsPerPage);
      renderPagination(currentPageItems.length);
    }
  
    const loadNextPage = () => {
      if (currentPosition + itemsPerPage < paginationData.length) {
        currentPosition += itemsPerPage;
        updateDataAndPagination();
      }
    }
  
    const loadPreviousPage = () => {
      if (currentPosition - itemsPerPage >= 0) {
        currentPosition -= itemsPerPage;
        updateDataAndPagination();
      }
    }
  
    const renderPagination = (/** @type {number} */ totalItems) => {
      totalPages = Math.ceil(paginationData.length / itemsPerPage);
      const currentPage = Math.ceil((currentPosition + 1) / itemsPerPage);
  
      startPage = currentPage - Math.floor(showPage / 2);
      startPage = Math.max(1, startPage);
      endPage = Math.min(startPage + showPage - 1, totalPages);
  
      pagesToShow = Array.from({ length: endPage - startPage + 1 }, (_, i) => startPage + i);
    }
  
    const goToPage = (/** @type {number} */ pageNumber) => {
      currentPosition = (pageNumber - 1) * itemsPerPage;
      updateDataAndPagination();
    }
  
    $: startRange = currentPosition + 1;
    $: endRange = Math.min(currentPosition + itemsPerPage, totalItems);
  
    onMount(() => {
      // Call renderPagination when the component initially mounts
      renderPagination(paginationData.length);
    });
  
    $: currentPageItems = paginationData.slice(currentPosition, currentPosition + itemsPerPage);
    $: filteredItems = paginationData.filter((item) => item.maker.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1);
  </script>
  
  <Section name="advancedTable" classSection='bg-gray-50 dark:bg-gray-900 p-3 sm:p-10'>
      <TableSearch placeholder="Search" hoverable={true} bind:inputValue={searchTerm} {divClass} {innerDivClass} {searchClass} >
  
      <div slot="header" class="w-full md:w-auto flex flex-col md:flex-row space-y-2 md:space-y-0 items-stretch md:items-center justify-end md:space-x-3 flex-shrink-0">
        <Button>
          <PlusOutline class="h-3.5 w-3.5 mr-2" />Add product
        </Button>
        <Button color='alternative'>Actions<ChevronDownOutline class="w-3 h-3 ml-2 " /></Button>
          <Dropdown class="w-44 divide-y divide-gray-100">
            <DropdownItem>Mass Edit</DropdownItem>
            <DropdownItem>Delete all</DropdownItem>
          </Dropdown>
        <Button color='alternative'>Filter<FilterSolid class="w-3 h-3 ml-2 " /></Button>
          <Dropdown class="w-48 p-3 space-y-2 text-sm">
            <h6 class="mb-3 text-sm font-medium text-gray-900 dark:text-white">Choose brand</h6>
            <li>
              <Checkbox>Apple (56)</Checkbox>
            </li>
            <li>
              <Checkbox>Microsoft (16)</Checkbox>
            </li>
            <li>
              <Checkbox>Razor (49)</Checkbox>
            </li>
            <li>
              <Checkbox>Nikon (12)</Checkbox>
            </li>
            <li>
              <Checkbox>BenQ (74)</Checkbox>
            </li>
          </Dropdown>
      </div>
        <TableHead>
          <TableHeadCell padding="px-4 py-3" scope="col">Id</TableHeadCell>
          <TableHeadCell padding="px-4 py-3" scope="col">Maker</TableHeadCell>
          <TableHeadCell padding="px-4 py-3" scope="col">Type</TableHeadCell>
          <TableHeadCell padding="px-4 py-3" scope="col">Name</TableHeadCell>
        </TableHead>
        <TableBody class="divide-y">
          {#if searchTerm !== ''}
            {#each filteredItems as item (item.id)}
              <TableBodyRow>
                <TableBodyCell tdClass="px-4 py-3">{item.id}</TableBodyCell>
                <TableBodyCell tdClass="px-4 py-3">{item.maker}</TableBodyCell>
                <TableBodyCell tdClass="px-4 py-3">{item.type}</TableBodyCell>
                <TableBodyCell tdClass="px-4 py-3">{item.make}</TableBodyCell>
              </TableBodyRow>
            {/each}
          {:else}
            {#each currentPageItems as item (item.id)}
              <TableBodyRow>
                <TableBodyCell tdClass="px-4 py-3">{item.id}</TableBodyCell>
                <TableBodyCell tdClass="px-4 py-3">{item.maker}</TableBodyCell>
                <TableBodyCell tdClass="px-4 py-3">{item.type}</TableBodyCell>
                <TableBodyCell tdClass="px-4 py-3">{item.make}</TableBodyCell>
              </TableBodyRow>
            {/each}
          {/if}
        </TableBody>
        <div slot="footer" class="flex flex-col md:flex-row justify-between items-start md:items-center space-y-3 md:space-y-0 p-4" aria-label="Table navigation">
        <span class="text-sm font-normal text-gray-500 dark:text-gray-400">
          Showing
          <span class="font-semibold text-gray-900 dark:text-white">{startRange}-{endRange}</span>
          of
          <span class="font-semibold text-gray-900 dark:text-white">{totalItems}</span>
        </span>
          <ButtonGroup>
            <Button on:click={loadPreviousPage} disabled={currentPosition === 0}><ChevronLeftOutline size='xs' class='m-1.5'/></Button>
            {#each pagesToShow as pageNumber}
              <Button on:click={() => goToPage(pageNumber)}>{pageNumber}</Button>
            {/each}
            <Button on:click={loadNextPage} disabled={ totalPages === endPage }><ChevronRightOutline size='xs' class='m-1.5'/></Button>
          </ButtonGroup>
        </div>
      </TableSearch>
  </Section>