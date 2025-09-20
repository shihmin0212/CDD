<template>
  <div class="formContainer">
    <div class="formSectionTitle collapsible" @click="$emit('toggle')">
      <label>流程紀錄</label>
      <span class="collapse-icon" :class="{ 'expanded': isExpanded }"></span>
    </div>
    <transition name="collapse">
      <div v-show="isExpanded" class="form-content">
        <table class="history-table">
          <thead>
            <tr>
              <th>#</th>
              <th>部門</th>
              <th>處理人員</th>
              <th>狀態</th>
              <th>開始時間</th>
              <th>結束時間</th>
              <th>備註</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="history.length === 0">
              <td colspan="7">沒有流程紀錄。</td>
            </tr>
            <tr v-for="item in history" :key="item.sequence">
              <td>{{ item.sequence }}</td>
              <td>{{ item.department }}</td>
              <td>{{ item.user }}</td>
              <td>{{ item.status }}</td>
              <td>{{ item.startTime }}</td>
              <td>{{ item.endTime }}</td>
              <td>{{ item.memo }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  history: any[];
  isExpanded: boolean;
}>();

defineEmits(['toggle']);
</script>

<style scoped>
/* 統一的容器與標題樣式 */
.formContainer { margin-bottom: 1rem; background-color: #fff; border: 1px solid #ddd; border-radius: 8px; overflow: hidden; }
.formSectionTitle { padding: 1rem; background-color: #00a19b; font-size: 1.1rem; font-weight: 600; border-bottom: 1px solid #ddd; }.formSectionTitle.collapsible { cursor: pointer; display: flex; justify-content: space-between; align-items: center; user-select: none; }

/* 統一的摺疊圖示與動畫 */
.collapse-icon { width: 16px; height: 16px; transition: transform 0.3s ease; background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="%23555"><path d="M201.4 342.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 274.7 86.6 137.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"/></svg>'); background-size: contain; background-repeat: no-repeat; background-position: center; }
.collapse-icon.expanded { transform: rotate(180deg); }
.collapse-enter-active, .collapse-leave-active { transition: all 0.3s ease-in-out; overflow: hidden; }
.collapse-enter-from, .collapse-leave-to { max-height: 0; opacity: 0; }
.collapse-enter-to, .collapse-leave-from { max-height: 1000px; opacity: 1; }

/* 內容與表格樣式 */
.form-content { padding: 1rem; }
.history-table { width: 100%; border-collapse: collapse; }
.history-table th, .history-table td { border: 1px solid #ddd; padding: 8px 12px; text-align: left; vertical-align: middle; font-size: 14px; }
.history-table th { background-color: #f2f2f2; font-weight: bold; }
.history-table td[colspan="7"] { text-align: center; color: #888; }
</style>