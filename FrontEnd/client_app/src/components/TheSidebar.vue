<template>
  <aside class="gemini-sidebar">
    <div class="sidebar-header">
      <h5 class="mb-0 sidebar-title">客戶盡職審查系統</h5>
    </div>
    <ul class="menu-list">
      <li class="menu-item" :class="{ 'active': route.path.startsWith('/customer-review') }" @click="navigateTo('/customer-review')" title="客戶盡職審查">
        <span class="menu-icon">📄</span>
        <span class="menu-text">客戶盡職審查</span>
      </li>
      <li class="menu-item" :class="{ 'active': route.path.startsWith('/proxy-query') }" @click="navigateTo('/proxy-query')" title="當日代理查詢">
        <span class="menu-icon">👥</span>
        <span class="menu-text">當日代理查詢</span>
      </li>
      <li class="menu-item" :class="{ 'active': route.path.startsWith('/case-management') }" @click="navigateTo('/case-management')" title="案件管理">
        <span class="menu-icon">🗂️</span>
        <span class="menu-text">案件管理</span>
      </li>
    </ul>
  </aside>
</template>

<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

const navigateTo = (path: string) => {
  router.push(path);
};
</script>

<style scoped>
/* 側邊欄容器 */
.gemini-sidebar {
  width: 260px; /* 展開時的寬度 */
  flex-shrink: 0;
  background-color: #f0f4f9;
  border-right: 1px solid #e0e0e0;
  display: flex;
  flex-direction: column;
  padding: 1rem;
  overflow: hidden; /* 隱藏超出寬度的內容 */
  transition: width 0.3s ease-in-out; /* 寬度變化的動畫效果 */
}

/* 當 .app-layout 有 .sidebar-collapsed class 時，側邊欄的樣式 */
.app-layout.sidebar-collapsed .gemini-sidebar {
  width: 80px; /* 收合時的寬度 */
}

.sidebar-header {
  display: flex;
  align-items: center;
  height: 48px; /* 固定高度以對齊按鈕 */
  padding: 0.5rem;
  color: #1f1f1f;
  flex-shrink: 0;
}

.sidebar-title {
  transition: opacity 0.3s ease-in-out;
  white-space: nowrap; /* 防止文字換行 */
}
.app-layout.sidebar-collapsed .sidebar-title {
  opacity: 0; /* 收合時標題消失 */
}

/* 選單列表 */
.menu-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

/* 選單項目 */
.menu-item {
  display: flex;
  align-items: center;
  gap: 16px; /* 圖示與文字的間距 */
  height: 48px;
  padding: 0 16px;
  margin-bottom: 4px;
  border-radius: 24px; /* 圓角膠囊形狀 */
  color: #1f1f1f;
  font-weight: 500;
  font-size: 0.9rem;
  cursor: pointer;
  text-decoration: none;
  transition: background-color 0.2s ease-in-out;
  white-space: nowrap; /* 防止文字換行 */
}

.menu-icon {
  font-size: 1.2rem;
  flex-shrink: 0;
}

.menu-text {
  transition: opacity 0.2s ease-in-out;
}

/* 收合時，選單文字消失 */
.app-layout.sidebar-collapsed .menu-text {
  opacity: 0;
  pointer-events: none; /* 讓滑鼠事件穿透 */
}

/* 滑鼠懸停效果 */
.menu-item:hover {
  background-color: #e8eaed;
}

/* 選中狀態 */
.menu-item.active {
  background-color: #d5e2f6;
  color: #0b57d0;
  font-weight: bold;
}
</style>