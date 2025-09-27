// src/stores/layout.ts

import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useLayoutStore = defineStore('layout', () => {
  // 控制側邊欄收合的狀態
  const isSidebarCollapsed = ref(false);

  // 切換側邊欄狀態的方法
  const toggleSidebar = () => {
    isSidebarCollapsed.value = !isSidebarCollapsed.value;
  };

  return {
    isSidebarCollapsed,
    toggleSidebar,
  };
});