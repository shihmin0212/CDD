import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useProxyQueryStore = defineStore('proxyQuery', () => {
  // 模擬資料與狀態
  const searchForm = ref({
    branch: '',
    personnel: ''
  });

  const editingRows = ref<{ [key: string]: boolean }>({});
  const originalData = ref<{ [key: string]: any }>({});
  const currentPage = ref(1);
  const itemsPerPage = 5;

  const branchOptions = ref(['經紀本部', '雙和分公司', '士林分公司', '信義分公司', '左營分公司', '數位分公司']);
  const personnelOptions = ref(['陳靜香', '林大熊', '王小明', '張大華']);
  const agentOptions = ref([
    { id: '71235', name: '王OO' },
    { id: '71239', name: '陳OO' },
    { id: '71103', name: '李OO' },
    { id: '71083', name: '黃OO' }
  ]);

  const proxyList = ref<any[]>([]);

  // 計算屬性
  const filteredList = computed(() => {
    return proxyList.value.filter(item => {
      return (
        (searchForm.value.branch === '' || item.branch === searchForm.value.branch) &&
        (searchForm.value.personnel === '' || item.name === searchForm.value.personnel)
      );
    });
  });

  const paginatedList = computed(() => {
    const startIndex = (currentPage.value - 1) * itemsPerPage;
    return filteredList.value.slice(startIndex, startIndex + itemsPerPage);
  });

  const totalPages = computed(() => {
    return Math.ceil(filteredList.value.length / itemsPerPage);
  });

  // 方法
  const getAgentName = (agentId: string) => {
    const agent = agentOptions.value.find(a => a.id === agentId);
    return agent ? agent.name : '';
  };

  const setSearchForm = (form: any) => {
    searchForm.value = form;
  };

  const setCurrentPage = (page: number) => {
    currentPage.value = page;
  };

  const handleSearch = () => {
    currentPage.value = 1;
  };

  const editRow = (employeeId: string) => {
    editingRows.value[employeeId] = true;
    originalData.value[employeeId] = { ...proxyList.value.find(item => item.employeeId === employeeId) };
  };

  const saveRow = (item: any) => {
    return new Promise<string>((resolve) => {
      editingRows.value[item.employeeId] = false;
      resolve('代理資訊已成功儲存！');
    });
  };

  const cancelEdit = (employeeId: string) => {
    return new Promise<string>((resolve) => {
      const originalItem = originalData.value[employeeId];
      if (originalItem) {
        const index = proxyList.value.findIndex(item => item.employeeId === employeeId);
        if (index !== -1) {
          proxyList.value[index] = originalItem;
        }
      }
      editingRows.value[employeeId] = false;
      resolve('已取消編輯。');
    });
  };

  const goToPage = (page: number) => {
    currentPage.value = page;
  };

  const mockFetchData = () => {
    proxyList.value = [
      { branch: '數位分公司', employeeId: '71235', name: '林OO', agentId: '', applicationDate: '', lastModifiedUser: '', lastModifiedTime: '' },
      { branch: '數位分公司', employeeId: '71236', name: '張OO', agentId: '71239', applicationDate: '2025/08/06', lastModifiedUser: '林OO(71235)', lastModifiedTime: '2025-05-23 00:00' },
      { branch: '數位分公司', employeeId: '71237', name: '李OO', agentId: '', applicationDate: '', lastModifiedUser: '', lastModifiedTime: '' },
      { branch: '數位分公司', employeeId: '71238', name: '王OO', agentId: '', applicationDate: '', lastModifiedUser: '', lastModifiedTime: '' },
      { branch: '數位分公司', employeeId: '71239', name: '陳OO', agentId: '', applicationDate: '', lastModifiedUser: '', lastModifiedTime: '' },
      { branch: '經紀本部', employeeId: '80001', name: '趙OO', agentId: '71235', applicationDate: '2025/08/07', lastModifiedUser: '趙OO(80001)', lastModifiedTime: '2025-05-24 10:00' },
      { branch: '經紀本部', employeeId: '80002', name: '錢OO', agentId: '', applicationDate: '', lastModifiedUser: '', lastModifiedTime: '' },
      { branch: '經紀本部', employeeId: '80003', name: '孫OO', agentId: '', applicationDate: '', lastModifiedUser: '', lastModifiedTime: '' },
    ];
  };

  return {
    searchForm,
    editingRows,
    originalData,
    currentPage,
    itemsPerPage,
    branchOptions,
    personnelOptions,
    agentOptions,
    proxyList,
    filteredList,
    paginatedList,
    totalPages,
    getAgentName,
    setSearchForm,
    setCurrentPage,
    handleSearch,
    editRow,
    saveRow,
    cancelEdit,
    goToPage,
    mockFetchData
  };
});
