<template>
  <div class="floating-helper-container">
    <transition name="helper-fade">
      <div v-if="isOpen" class="helper-panel card shadow-lg">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h6 class="mb-0">{{ title }}</h6>
          <button type="button" class="btn-close" @click="isOpen = false"></button>
        </div>
        <div class="card-body">
          <slot></slot>
        </div>
      </div>
    </transition>

    <button
      class="helper-button btn btn-warning rounded-circle d-flex align-items-center justify-content-center"
      @click="togglePanel"
      title="顯示說明"
    >
      <i class="bi bi-exclamation-lg"></i>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

defineProps({
  title: {
    type: String,
    default: '輔助說明'
  }
});

const isOpen = ref(false);

const togglePanel = () => {
  isOpen.value = !isOpen.value;
};
</script>

<style scoped>
.floating-helper-container {
  position: fixed;
  top: 150px; 
  right: 25px;
  z-index: 1040;
  display: flex;
  align-items: flex-start;
}

.helper-button {
  width: 50px;
  height: 50px;
  font-size: 1.5rem;
  color: white;
  background-color: #f0aa3c;
  border: 2px solid white;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  position: relative;
  z-index: 2;
}

.helper-panel {
  /* 【修改】加大寬度以容納表格 */
  width: 800px; 
  margin-right: 15px;
  border: none;

  /* 【新增】限制最大高度並設定 flexbox 佈局，為內容滾動做準備 */
  max-height: 80vh; /* 最高佔螢幕高度的 80% */
  display: flex;
  flex-direction: column;
}

.card-body {
  font-size: 0.9rem;
  line-height: 1.6;
  white-space: pre-line;

  /* 【新增】當內容超出時，自動產生垂直卷軸 */
  overflow-y: auto;
}

/* Vue 的 Transition 動畫效果 */
.helper-fade-enter-active,
.helper-fade-leave-active {
  transition: all 0.3s ease;
}

.helper-fade-enter-from,
.helper-fade-leave-to {
  opacity: 0;
  transform: translateX(20px);
}
</style>