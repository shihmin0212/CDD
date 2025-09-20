<template>
  <div class="formContainer">
    <div class="formSectionTitle collapsible" @click="$emit('toggle')">
      <label>單位主管(經理人)審核</label>
      <span class="collapse-icon" :class="{ 'expanded': isExpanded }"></span>
    </div>
    <transition name="collapse">
      <div v-show="isExpanded" class="form-content">
        <!-- 之後可在此處加入審核表單內容 -->
        <p>此處為主管審核區塊。</p>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  isExpanded: boolean;
}>();

defineEmits(['toggle']);
</script>

<style scoped>
/* 從 CustomerInfo.vue 複製通用樣式 */
.formContainer { margin-bottom: 1rem; background-color: #fff; border: 1px solid #ddd; border-radius: 8px; overflow: hidden; }
.formSectionTitle { padding: 1rem; background-color: #00a19b; font-size: 1.1rem; font-weight: 600; border-bottom: 1px solid #ddd; }.formSectionTitle.collapsible { cursor: pointer; display: flex; justify-content: space-between; align-items: center; user-select: none; }
.collapse-icon { width: 16px; height: 16px; transition: transform 0.3s ease; background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="%23555"><path d="M201.4 342.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 274.7 86.6 137.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"/></svg>'); background-size: contain; background-repeat: no-repeat; background-position: center; }
.collapse-icon.expanded { transform: rotate(180deg); }
.collapse-enter-active, .collapse-leave-active { transition: all 0.3s ease-in-out; overflow: hidden; }
.collapse-enter-from, .collapse-leave-to { max-height: 0; opacity: 0; }
.collapse-enter-to, .collapse-leave-from { max-height: 500px; opacity: 1; }
.form-content { padding: 1rem; }
</style>