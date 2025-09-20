<template>
  <div>
    <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="#">首頁</a></li>
        <li class="breadcrumb-item"><a href="#">客戶盡職審查</a></li>
        <li class="breadcrumb-item active" aria-current="page">新增審查表單</li>
      </ol>
    </nav>

    <div class="sticky-header">
      <ProgressBar :stages="customerReviewStore.caseData.stages" :current-stage-index="customerReviewStore.caseData.currentStageIndex" />
      <div class="action-buttons">
        <button class="btn btn-danger mx-1" @click="returnToPrevious" :disabled="customerReviewStore.caseData.currentStageIndex === 0 || customerReviewStore.isLoading">退回</button>
        <button class="btn btn-secondary mx-1" @click="save" :disabled="customerReviewStore.isLoading">暫存</button>
        <button class="btn btn-success mx-1" @click="sendToNext" :disabled="isLastStep || customerReviewStore.isLoading">傳送</button>
      </div>
    </div>

    <div class="content-body">
      <CustomerInfo :customer-info="customerReviewStore.caseData.customerInfo" :is-expanded="expandedCards.has('customerInfo')" @toggle="toggleCard('customerInfo')" />
      
      <ReviewItemsForm :is-expanded="expandedCards.has('reviewItems')" @toggle="toggleCard('reviewItems')" />

      <FileUpload v-if="isSalespersonStage" :is-expanded="expandedCards.has('fileUpload')" @toggle="toggleCard('fileUpload')" />

      <ManagerReview v-if="isManagerStage" :is-expanded="expandedCards.has('managerReview')" @toggle="toggleCard('managerReview')" />

      <HistoryLog :history="formattedHistory" :is-expanded="expandedCards.has('historyLog')" @toggle="toggleCard('historyLog')" />
    </div>

    <MessageBox :show="showMessage" :message="message" @close="closeMessage" />
    <div v-if="customerReviewStore.isLoading" class="loading-overlay show">
      <div class="loading-spinner"></div>
      <div class="loading-text">正在處理中...</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'; // 移除 watch
import { useCustomerReviewStore } from '../stores/customerReview';
import MessageBox from './MessageBox.vue';
import ReviewItemsForm from './ReviewItemsForm.vue';
import FileUpload from './FileUpload.vue';
import ProgressBar from './ProgressBar.vue';
import CustomerInfo from './CustomerInfo.vue';
import ManagerReview from './ManagerReview.vue';
import HistoryLog from './HistoryLog.vue';

const customerReviewStore = useCustomerReviewStore();
const message = ref('');
const showMessage = ref(false);
const expandedCards = ref(new Set(['customerInfo']));

const toggleCard = (cardId: string) => {
  if (expandedCards.value.has(cardId)) {
    expandedCards.value.delete(cardId);
  } else {
    expandedCards.value.add(cardId);
  }
};

const isSalespersonStage = computed(() => customerReviewStore.caseData.stages[customerReviewStore.caseData.currentStageIndex]?.title === '營業員');
const isManagerStage = computed(() => customerReviewStore.caseData.stages[customerReviewStore.caseData.currentStageIndex]?.title === '經理人');
const isLastStep = computed(() => customerReviewStore.caseData.currentStageIndex >= customerReviewStore.caseData.stages.length - 1);

// 【修改後】移除此處的 watch
const formattedHistory = computed(() => {
  const completedRecords = customerReviewStore.history.map((item, index) => ({ sequence: index + 1, ...item }));
  let currentStepRecord = null;
  if (!isLastStep.value) {
    const currentStageIndex = customerReviewStore.caseData.currentStageIndex;
    const currentStage = customerReviewStore.caseData.stages[currentStageIndex];
    if (currentStage) {
      currentStepRecord = { sequence: completedRecords.length + 1, department: currentStage.department, user: currentStage.user, status: '待取件', startTime: null, endTime: null, memo: null };
    }
  }
  const fullHistory = currentStepRecord ? [...completedRecords, currentStepRecord] : [...completedRecords];
  return fullHistory.sort((a, b) => b.sequence - a.sequence);
});

const showMessageBox = (msg: string) => { message.value = msg; showMessage.value = true; };
const closeMessage = () => { showMessage.value = false; };
const save = async () => { const result = await customerReviewStore.save(); if (result) showMessageBox(result); };
const sendToNext = async () => { const result = await customerReviewStore.sendToNext(); if (result) showMessageBox(result); };
const returnToPrevious = async () => { const result = await customerReviewStore.returnToPrevious(); if (result) showMessageBox(result); };

onMounted(() => {
  customerReviewStore.initData();
});
</script>

<style scoped>
/* 【新增】黏性表頭的相關樣式 */
.sticky-header {
  /* 關鍵屬性，讓這個區塊黏在頂部 */
  position: sticky;
  top: 0;
  
  /* 確保它在最上層，不會被下方內容覆蓋 */
  z-index: 10;
  
  /* 給一個背景色，這樣滾動時才不會變透明 */
  background-color: #f8f9fa; /* 與頁面背景色相同 */
  
  /* 加一些內邊距和陰影，提升視覺效果 */
  padding-top: 1rem;
  padding-bottom: 0.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  margin: 0 -1rem; /* 讓背景和陰影可以延伸到頁面邊緣 */
  padding-left: 1rem;
  padding-right: 1rem;
}

/* 確保進度條和按鈕區的樣式適合新的佈局 */
.sticky-header .progressbar-container {
  margin-bottom: 0.5rem;
}
.sticky-header .action-buttons {
  margin-top: 0.5rem;
  margin-bottom: 0;
}
/* 讓下方的內容區塊與固定表頭之間有一點距離 */
.content-body {
  padding-top: 1rem;
}

/* 以下樣式已移至各子元件，故可移除 */
</style>