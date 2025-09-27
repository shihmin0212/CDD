<template>
  <div class="main-header">
    <button class="btn-icon" @click="layoutStore.toggleSidebar" title="收合選單">
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="3" y1="12" x2="21" y2="12"></line><line x1="3" y1="6" x2="21" y2="6"></line><line x1="3" y1="18" x2="21" y2="18"></line></svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useLayoutStore } from '@/stores/layout';

const layoutStore = useLayoutStore();
</script>

<style scoped>
.main-header {
  display: flex;
  align-items: center;
  padding: 0 1.5rem;
  height: 64px;
  flex-shrink: 0;
  border-bottom: 1px solid #e0e0e0;
}

.btn-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border: none;
  background-color: transparent;
  border-radius: 50%;
  cursor: pointer;
  transition: background-color 0.2s;
  color: #444746;
}
.btn-icon:hover {
  background-color: #e8eaed;
}
</style>