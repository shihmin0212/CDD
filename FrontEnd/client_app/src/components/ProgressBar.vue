<template>
  <ul class="progressbar-container">
    <li v-for="(stage, index) in stages" 
        :key="stage.title" 
        :class="getStageClass(index)">

      <div class="progress-title">{{ stage.title }}</div>
      <div class="progress-dot"></div>
      <div class="progress-note">
        <div>{{ stage.user }}</div>
        <div>{{ stage.date }}</div>
      </div>
    </li>
  </ul>
</template>

<script setup lang="ts">
import { toRefs } from 'vue';

// 定義傳入的 props 型別
interface Stage {
  title: string;
  user: string;
  date: string;
  department?: string;
}

const props = defineProps<{
  stages: Stage[];
  currentStageIndex: number;
}>();

// 使用 toRefs 保持 props 的響應性
const { stages, currentStageIndex } = toRefs(props);

// 從父元件移過來的邏輯
const getStageClass = (index: number) => {
  if (index < currentStageIndex.value) return 'done';
  if (index === currentStageIndex.value) return 'now';
  return 'next';
};
</script>

<style scoped>
/* 將進度條相關樣式移至此處 */
.progressbar-container {
  display: flex;
  justify-content: space-between;
  list-style: none;
  padding: 0;
  margin: 0;
  position: relative;
  width: 100%;
}

.progressbar-container::before {
  content: '';
  position: absolute;
  top: 20px;
  left: 0;
  right: 0;
  height: 4px;
  background-color: #e0e0e0;
  transform: translateY(-50%);
  z-index: -1;
}

.progressbar-container li {
  flex: 1;
  text-align: center;
  position: relative;
}

.progress-title {
  font-size: 14px;
  font-weight: bold;
  margin-bottom: 8px;
}

.progress-dot {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background-color: #ccc;
  margin: 0 auto;
  border: 2px solid #fff;
  position: relative;
  z-index: 1;
}

.progress-note {
  font-size: 12px;
  color: #666;
  margin-top: 8px;
}

/* 狀態樣式 */
li.done .progress-title, li.now .progress-title {
  color: #28a745;
}
li.done .progress-dot {
  background-color: #28a745;
}
li.now .progress-dot {
  background-color: #007bff;
  transform: scale(1.2);
}
li.next .progress-title {
  color: #6c757d;
}
li.next .progress-dot {
  background-color: #e0e0e0;
}
</style>