<template>
  <div class="formContainer">
    <div class="formSectionTitle collapsible" @click="$emit('toggle')">
      <div class="title-group">
        <label>審查項目</label>
        <button class="btn btn-sm btn-dark" @click.stop="customerReviewStore.resetReviewData">重置選項</button>
      </div>
      <div class="title-actions">
        <span 
          class="collapse-icon" 
          :class="{ 'expanded': isExpanded }" 
          title="收合/展開"
        ></span>
      </div>
    </div>

    <transition name="collapse">
      <div v-show="isExpanded" class="review-grid-table">
        <div class="review-grid-row header">
          <div class="review-grid-cell">項目</div>
          <div class="review-grid-cell">選項</div>
          <div class="review-grid-cell">備註</div>
        </div>

        <div v-for="item in reviewData.items" :key="item.id" class="review-grid-row">
          <div class="review-grid-cell review-title">{{ item.title }}</div>
          <div class="review-grid-cell review-options">
            <div v-if="item.type === 'checkbox'" class="options-group">
              <div v-for="option in item.options" :key="option.id" class="option-item">
                <label>
                  <input type="checkbox" :value="option.id" v-model="item.selected" :disabled="item.disabled">
                  <span>{{ option.text }}</span>
                  <span v-if="shouldShowAlert(item, option)" :class="`alert-text ${option.alert?.toLowerCase()}`"> ({{ option.alert }})</span>
                </label>
              </div>
            </div>
            <div v-if="item.type === 'radio'" class="options-group">
              <div v-for="option in item.options" :key="option.id" class="option-item">
                <label>
                  <input type="radio" :value="option.id" v-model="item.selected" :disabled="item.disabled">
                  <span>{{ option.text }}</span>
                  <span v-if="option.isDefault" class="default-text"> (系統預設值)</span>
                  <span v-if="shouldShowAlert(item, option)" :class="`alert-text ${option.alert?.toLowerCase()}`"> ({{ option.alert }})</span>
                </label>
              </div>
            </div>
            <div v-if="item.type === 'group'" class="options-group">
              <div v-for="field in item.subFields" :key="field.id" class="sub-field-item">
                <label>{{ field.label }}：</label>
                <input :type="field.type === 'date' ? 'date' : 'text'" v-model="field.value" :placeholder="field.placeholder" :disabled="field.disabled">
                <span v-if="field.unit">{{ field.unit }}</span>
              </div>
            </div>
          </div>
          <div class="review-grid-cell review-remarks">
            <div v-for="(remark, rIndex) in item.remarks" :key="rIndex" class="remark-text">{{ remark }}</div>
            <div v-for="field in item.subFields" :key="field.id" class="sub-field-item-inline">
              <template v-if="field.condition && item.selected === field.condition.onValue">
                <label>{{ field.label }}：</label>
                <input :type="field.type" v-model="field.value" :placeholder="field.placeholder" :disabled="field.disabled">
              </template>
            </div>
            <div v-if="item.alert && (item.selected === 'yes' || (Array.isArray(item.selected) && item.selected.length > 0))" class="remark-alert">{{ item.alert }}</div>
          </div>
        </div>

        <div class="review-grid-row">
          <div class="review-grid-cell review-title">記載事項及說明</div>
          <div class="review-grid-cell full-span">
            <textarea v-model="reviewData.notes" placeholder="請輸入..."></textarea>
          </div>
        </div>

        <div v-if="isEddTriggered" class="review-grid-row edd-section">
          <div class="review-grid-cell review-title">是否需進行加強審查(EDD)?</div>
          <div class="review-grid-cell full-span options-group">
            <div class="option-item">
              <label>
                <input type="radio" value="yes" v-model="reviewData.requiresEDD">
                <span>是</span>
              </label>
            </div>
            <div class="option-item">
              <label>
                <input type="radio" value="no" v-model="reviewData.requiresEDD">
                <span>否</span>
              </label>
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { computed, defineProps, defineEmits, watch } from 'vue';
import { useCustomerReviewStore } from '../stores/customerReview';

defineProps({ isExpanded: { type: Boolean, default: false } });
defineEmits(['toggle']);

const customerReviewStore = useCustomerReviewStore();
const reviewData = computed(() => customerReviewStore.caseData.reviewData);

// 【修改後】監聽目標改為 reviewData.requiresEDD，確保手動點擊才觸發
watch(() => reviewData.value.requiresEDD, () => {
  customerReviewStore.updateWorkflowPath();
});

// 這個 computed 依然用來決定是否要顯示「是否需進行加強審查」的問題
const isEddTriggered = computed(() => {
  if (!reviewData.value || !reviewData.value.items) return false;
  return reviewData.value.items.some(item =>
    item.options?.some(option =>
      option.alert === 'EDD' && shouldShowAlert(item, option)
    )
  );
});

const shouldShowAlert = (item: any, option: any) => {
  if (!option.alert) return false;
  if (Array.isArray(item.selected)) { return item.selected.includes(option.id); }
  return item.selected === option.id;
};
</script>

<style scoped>
.formSectionTitle.collapsible { cursor: pointer; user-select: none; }
.title-group { display: flex; align-items: center; gap: 1rem; }
.title-actions { display: flex; align-items: center; gap: 1rem; }
.collapse-icon { width: 16px; height: 16px; cursor: pointer; transition: transform 0.3s ease; background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="%23555"><path d="M201.4 137.4c12.5-12.5 32.8-12.5 45.3 0l160 160c12.5 12.5 12.5 32.8 0 45.3s-32.8 12.5-45.3 0L224 205.3 86.6 342.6c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3l160-160z"/></svg>'); background-size: contain; background-repeat: no-repeat; background-position: center; }
.collapse-icon.expanded { transform: rotate(180deg); }
.collapse-enter-active, .collapse-leave-active { transition: all 0.3s ease-in-out; overflow: hidden; }
.collapse-enter-from, .collapse-leave-to { max-height: 0; opacity: 0; }
.collapse-enter-to, .collapse-leave-from { max-height: 3000px; opacity: 1; }
.formSectionTitle { display: flex; justify-content: space-between; align-items: center; }
.btn-sm { padding: 5px 12px; font-size: 14px; border-radius: 4px; }
.btn-dark { background-color: #003366; color: white; border: none; cursor: pointer; }
.btn-dark:hover { background-color: #0056b3; }
.review-grid-table { border: 1px solid #ddd; border-top: none; transition: all 0.3s ease-in-out; }
.review-grid-row { display: grid; grid-template-columns: 3fr 5fr 4fr; border-bottom: 1px solid #eee; }
.review-grid-table > .review-grid-row:last-of-type { border-bottom: none; }
.review-grid-row.header { background-color: #f8f9fa; font-weight: 600; color: #495057; border-bottom: 1px solid #ddd; }
.review-grid-cell { padding: 12px 15px; display: flex; flex-direction: column; justify-content: center; gap: 8px; }
.review-grid-cell:not(:last-child) { border-right: 1px solid #eee; }
.review-grid-row.header .review-grid-cell { padding: 10px 15px; }
.review-title { font-weight: 600; }
.review-remarks { font-size: 13px; color: #666; line-height: 1.6; }
.remark-text { word-break: break-word; }
.remark-alert { color: #e53935; font-weight: bold; }
.options-group { display: flex; flex-direction: column; gap: 8px; }
.option-item label { display: flex; align-items: center; cursor: pointer; font-size: 14px; }
.option-item input { margin-right: 8px; }
.default-text { color: grey; font-size: 0.9em; margin-left: 4px; }
.alert-text { font-weight: bold; margin-left: 4px; }
.alert-text.婉拒 { color: #e53935; }
.alert-text.edd { color: #f57c00; }
.sub-field-item, .sub-field-item-inline { display: flex; align-items: center; gap: 5px; }
.sub-field-item input, .sub-field-item-inline input { padding: 4px 8px; border: 1px solid #ccc; border-radius: 4px; }
.review-grid-cell.full-span { grid-column: 2 / -1; }
textarea { width: 100%; min-height: 60px; padding: 8px; border: 1px solid #ddd; border-radius: 4px; resize: vertical; }
.edd-section { background-color: #fffbe6; border-top: 2px solid #ffe58f; }
.edd-section .review-title { color: #d46b08; font-weight: bold; }
</style>