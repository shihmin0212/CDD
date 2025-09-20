import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

// 定義資料介面
export interface ProxyItem {
  branch: string;
  employeeId: string;
  name: string;
  agentId: string | null;
  applicationDate: string | null;
  lastModifiedUser: string | null;
  lastModifiedTime: string | null;
}

export const useProxyQueryStore = defineStore('proxyQuery', () => {
  // --- STATE ---
  const searchForm = ref({
    branch: '',
    personnel: ''
  });

  const editingRows = ref<{ [key: string]: boolean }>({});
  const originalData = ref<{ [key: string]: ProxyItem }>({});
  const currentPage = ref(1);
  const itemsPerPage = 5;

  const branchOptions = ref(['', '經紀本部', '雙和分公司', '士林分公司', '信義分公司', '左營分公司', '數位分公司']);
  const personnelOptions = ref(['', '陳靜香', '林大熊', '王小明', '張大華', '林OO', '張OO', '李OO', '王OO', '陳OO', '趙OO', '錢OO', '孫OO']);
  const agentOptions = ref([
    { id: '71235', name: '王OO' },
    { id: '71239', name: '陳OO' },
    { id: '71103', name: '李OO' },
    { id: '71083', name: '黃OO' }
  ]);
  
  const proxyList = ref<ProxyItem[]>([]);

  // --- GETTERS (Computed) ---
  const filteredList = computed(() => {
    // ***** 修正點 *****
    // 移除了對 totalPages 的依賴，解除了循環
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
    if (filteredList.value.length === 0) return 1;
    return Math.ceil(filteredList.value.length / itemsPerPage);
  });

  // --- ACTIONS ---
  const getAgentName = (agentId: string | null) => {
    if (!agentId) return '';
    const agent = agentOptions.value.find(a => a.id === agentId);
    return agent ? agent.name : '';
  };

  // handleSearch 已經負責重置頁碼，這是正確的做法
  const handleSearch = () => {
    currentPage.value = 1;
  };

  const editRow = (employeeId: string) => {
    const itemToEdit = proxyList.value.find(item => item.employeeId === employeeId);
    if (itemToEdit) {
      originalData.value[employeeId] = { ...itemToEdit };
      editingRows.value[employeeId] = true;
    }
  };

  const saveRow = (itemToSave: ProxyItem) => {
    return new Promise<string>((resolve) => {
      const index = proxyList.value.findIndex(item => item.employeeId === itemToSave.employeeId);
      if (index !== -1) {
        const agentName = getAgentName(itemToSave.agentId);
        proxyList.value[index] = {
          ...itemToSave,
          lastModifiedUser: 'currentUser (00001)',
          lastModifiedTime: new Date().toLocaleString('sv-SE'),
        };
      }
      delete originalData.value[itemToSave.employeeId];
      editingRows.value[itemToSave.employeeId] = false;
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
      delete originalData.value[employeeId];
      editingRows.value[employeeId] = false;
      resolve('已取消編輯。');
    });
  };

  const goToPage = (page: number) => {
    if (page >= 1 && page <= totalPages.value) {
      currentPage.value = page;
    }
  };

  const mockFetchData = () => {
    proxyList.value = [
      { branch: '數位分公司', employeeId: '71235', name: '林OO', agentId: null, applicationDate: null, lastModifiedUser: null, lastModifiedTime: null },
      { branch: '數位分公司', employeeId: '71236', name: '張OO', agentId: '71239', applicationDate: '2025-08-06', lastModifiedUser: '林OO(71235)', lastModifiedTime: '2025-05-23 00:00:00' },
      { branch: '數位分公司', employeeId: '71237', name: '李OO', agentId: null, applicationDate: null, lastModifiedUser: null, lastModifiedTime: null },
      { branch: '數位分公司', employeeId: '71238', name: '王OO', agentId: null, applicationDate: null, lastModifiedUser: null, lastModifiedTime: null },
      { branch: '數位分公司', employeeId: '71239', name: '陳OO', agentId: null, applicationDate: null, lastModifiedUser: null, lastModifiedTime: null },
      { branch: '經紀本部', employeeId: '80001', name: '趙OO', agentId: '71235', applicationDate: '2025-08-07', lastModifiedUser: '趙OO(80001)', lastModifiedTime: '2025-05-24 10:00:00' },
      { branch: '經紀本部', employeeId: '80002', name: '錢OO', agentId: null, applicationDate: null, lastModifiedUser: null, lastModifiedTime: null },
      { branch: '經紀本部', employeeId: '80003', name: '孫OO', agentId: null, applicationDate: null, lastModifiedUser: null, lastModifiedTime: null },
    ];
  };

  return {
    searchForm, editingRows, currentPage, itemsPerPage, branchOptions, personnelOptions, agentOptions, proxyList,
    filteredList, paginatedList, totalPages,
    getAgentName, handleSearch, editRow, saveRow, cancelEdit, goToPage, mockFetchData
  };
});