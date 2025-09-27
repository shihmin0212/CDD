<template>
  <div class="app-layout" :class="{ 'sidebar-collapsed': layoutStore.isSidebarCollapsed }">
    <TheSidebar />

    <main class="main-content">
      <TheHeader />
      <div class="page-content">
        <router-view />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import TheSidebar from '@/components/TheSidebar.vue';
import TheHeader from '@/components/TheHeader.vue';
import { useLayoutStore } from '@/stores/layout';

const layoutStore = useLayoutStore();
</script>

<style scoped>
/* 整體佈局 */
.app-layout {
  display: flex;
  height: 100vh;
  width: 100vw;
  background-color: #fff;
}

/* 主內容區域 */
.main-content {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  /* 移除 transition，因為寬度不再由 main-content 控制 */
}

.page-content {
  padding: 2rem;
  flex-grow: 1;
  /* height: 100% 移除，改用 flex-grow 撐開 */
  overflow-y: auto; /* 內容過多時可滾動 */
}

/*
  注意：大部分樣式都已經被移到 TheSidebar.vue 和 TheHeader.vue 中了。
  這裡只保留最外層的佈局樣式。
*/
</style>