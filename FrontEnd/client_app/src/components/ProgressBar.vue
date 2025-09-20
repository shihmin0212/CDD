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

interface Stage {
  title: string;
  user: string;
  date: string;
}

const props = defineProps<{
  stages: Stage[];
  currentStageIndex: number;
}>();

const { stages, currentStageIndex } = toRefs(props);

const getStageClass = (index: number) => {
  if (index < currentStageIndex.value) return 'done';
  if (index === currentStageIndex.value) return 'now';
  return 'next';
};
</script>

<style scoped>
/* 玉山風格的進度條 CSS */
.progressbar-container { display: flex; justify-content: center; list-style: none; padding: 0; margin: 0 auto; position: relative; max-width: 800px; }
.progressbar-container li { text-align: center; flex: 1; position: relative; }
.progressbar-container li:not(:last-child)::after { content: ""; height: 3px; width: calc(100% - 30px); position: absolute; left: calc(50% + 15px); top: 38px; }
.progress-title { font-weight: bold; font-size: 1rem; color: #979797; }
li.now .progress-title { color: #45B29D; }
li.done .progress-title { color: #3C4C5E; }
.progress-dot { width: 30px; height: 30px; border-radius: 50%; margin: 5px auto; box-sizing: border-box; }
li.now .progress-dot { border: 3px solid #45B29D; position: relative; }
li.now .progress-dot::after { content: ""; width: 18px; height: 18px; border-radius: 50%; background: #45B29D; position: absolute; top: 3px; left: 3px; }
li.done .progress-dot { background-color: #3C4C5E; background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="white"><path d="M438.6 105.4c12.5 12.5 12.5 32.8 0 45.3l-256 256c-12.5 12.5-32.8 12.5-45.3 0l-128-128c-12.5-12.5-12.5-32.8 0-45.3s32.8-12.5 45.3 0L160 338.7 393.4 105.4c12.5-12.5 32.8-12.5 45.3 0z"/></svg>'); background-size: 16px; background-position: center; background-repeat: no-repeat; }
li.next .progress-dot { width: 20px; height: 20px; background-color: #CECECE; margin-top: 10px; }
.progress-note { font-size: 0.8rem; color: #6c757d; min-height: 2.4em; }
.progressbar-container li.done::after { background-color: #3C4C5E; }
.progressbar-container li.now::after, .progressbar-container li.next::after { background-color: #CECECE; }
</style>