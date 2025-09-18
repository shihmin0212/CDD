<template>
  <div id="pageContentContainer">
    <div class="breadcrumbContainer">
      <ul>
        <li>首頁</li>
        <li>客戶盡職審查</li>
        <li class="now">顧客盡職審查</li>
      </ul>
    </div>
    <header>客戶盡職審查系統</header>
    <ul class="progressbar-container">
      <li v-for="(stage, index) in customerReviewStore.caseData.stages" :key="stage.title" :class="getStageClass(index)">
        <div class="progress-title">{{ stage.title }}</div>
        <div class="progress-dot"></div>
        <div class="progress-note">
          <div>{{ stage.user }}</div>
          <div>{{ stage.date }}</div>
        </div>
      </li>
    </ul>

    <div id="problemCaseManager_UnsolvedForm">
      <div class="action-buttons">
        <button class="btn btn-danger mx-1" @click="returnToPrevious" :disabled="customerReviewStore.caseData.currentStageIndex === 0 || customerReviewStore.isLoading">退回</button>
        <button class="btn btn-secondary mx-1" @click="save" :disabled="customerReviewStore.isLoading">暫存</button>
        <button class="btn btn-success mx-1" @click="sendToNext" :disabled="isLastStep || customerReviewStore.isLoading">傳送</button>
      </div>

      <div class="formContainer">
        <div class="formSectionTitle collapsible" @click="toggleCard('customerInfo')">
          <label>顧客資訊</label>
          <span class="collapse-icon" :class="{ 'expanded': expandedCards.has('customerInfo') }"></span>
        </div>
        <transition name="collapse">
          <div v-show="expandedCards.has('customerInfo')" class="form-content customer-info-grid">
            <div class="form-row"><div class="form-title">顧客帳號</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.accountNumber }}</div><div class="form-title">身分證字號</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.idNumber }}</div></div>
            <div class="form-row"><div class="form-title">顧客姓名</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.name }}</div><div class="form-title">國籍</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.nationality }}</div></div>
            <div class="form-row"><div class="form-title">連絡電話</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.contactPhone }}</div><div class="form-title">行動電話</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.mobilePhone }}</div></div>
            <div class="form-row"><div class="form-title">出生年月日</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.birthDate }}</div><div class="form-title">年齡</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.age }}</div></div>
            <div class="form-row"><div class="form-title">身分類別</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.identityType }}</div><div class="form-title">職業類別</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.occupationCategory }}</div></div>
            <div class="form-row"><div class="form-title">公司名稱</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.companyName }}</div><div class="form-title">公司電話</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.companyPhone }}</div></div>
            <div class="form-row full-width"><div class="form-title">戶籍地址</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.householdAddress }}</div></div>
            <div class="form-row full-width"><div class="form-title">公司地址</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.companyAddress }}</div></div>
            <div class="form-row full-width"><div class="form-title">通訊地址</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.mailingAddress }}</div></div>
            <div class="form-row"><div class="form-title">銀行帳號</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.bankAccount }}</div><div class="form-title">證券投資上限</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.investmentLimit }}</div></div>
            <div class="form-row"><div class="form-title">緊急聯絡人</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.emergencyContact }}</div><div class="form-title">緊急連絡人電話</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.emergencyContactPhone }}</div></div>
            <div class="form-row"><div class="form-title">買賣代理人</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.tradingAgent }}</div><div class="form-title">法定代理人</div><div class="form-value">{{ customerReviewStore.caseData.customerInfo.legalRepresentative }}</div></div>
          </div>
        </transition>
      </div>
      
      <ReviewItemsForm :is-expanded="expandedCards.has('reviewItems')" @toggle="toggleCard('reviewItems')" />

      <FileUpload v-if="isSalespersonStage" :is-expanded="expandedCards.has('fileUpload')" @toggle="toggleCard('fileUpload')" />

      <div class="formContainer" v-if="isManagerStage">
        <div class="formSectionTitle collapsible" @click="toggleCard('managerReview')">
          <label>單位主管(經理人)審核</label>
          <span class="collapse-icon" :class="{ 'expanded': expandedCards.has('managerReview') }"></span>
        </div>
        <transition name="collapse">
          <div v-show="expandedCards.has('managerReview')" class="form-content">
            <div class="form-row single-row"><div class="form-title">審核結果：</div><div class="radio-group form-value"><label><input type="radio" value="Y" v-model="customerReviewStore.caseData.reviewResult"> 同意</label><label><input type="radio" value="N" v-model="customerReviewStore.caseData.reviewResult"> 不同意</label></div></div>
            <div class="form-row single-row"><div class="form-title">簽核意見：</div><div class="form-value"><textarea v-model="customerReviewStore.caseData.managerMemo" placeholder="請在此輸入簽核意見..."></textarea></div></div>
          </div>
        </transition>
      </div>

      <div class="formContainer">
        <div class="formSectionTitle collapsible" @click="toggleCard('historyLog')">
          <label>流程紀錄</label>
          <span class="collapse-icon" :class="{ 'expanded': expandedCards.has('historyLog') }"></span>
        </div>
        <transition name="collapse">
          <div v-show="expandedCards.has('historyLog')">
            <table class="history-table">
              <thead><tr><th>順序</th><th>處理人單位</th><th>處理人員</th><th>處理狀態</th><th>取件時間</th><th>完成時間</th><th>簽核意見</th></tr></thead>
              <tbody><tr v-if="formattedHistory.length === 0"><td colspan="7" style="text-align: center;">沒有流程紀錄。</td></tr><tr v-for="item in formattedHistory" :key="item.sequence"><td>{{ item.sequence }}</td><td>{{ item.department || '-' }}</td><td>{{ item.user || '-' }}</td><td>{{ item.status || '-' }}</td><td>{{ item.startTime || '-' }}</td><td>{{ item.endTime || '-' }}</td><td>{{ item.memo || '-' }}</td></tr></tbody>
            </table>
          </div>
        </transition>
      </div>
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

const getStageClass = (index: number) => {
  if (index < customerReviewStore.caseData.currentStageIndex) return 'done';
  if (index === customerReviewStore.caseData.currentStageIndex) return 'now';
  return 'next';
};

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
.formSectionTitle.collapsible { cursor: pointer; display: flex; justify-content: space-between; align-items: center; user-select: none; }
.collapse-icon { width: 16px; height: 16px; transition: transform 0.3s ease; background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="%23555"><path d="M201.4 342.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 274.7 86.6 137.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"/></svg>'); background-size: contain; background-repeat: no-repeat; background-position: center; }
.collapse-icon.expanded { transform: rotate(180deg); }
.collapse-enter-active, .collapse-leave-active { transition: all 0.3s ease-in-out; overflow: hidden; }
.collapse-enter-from, .collapse-leave-to { max-height: 0; opacity: 0; margin-top: -1rem; padding-top: 0; padding-bottom: 0; }
.collapse-enter-to, .collapse-leave-from { max-height: 1500px; opacity: 1; }
.form-content { padding: 10px; margin-top: 1rem; }
.customer-info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0 20px; }
.form-row { display: grid; grid-template-columns: 120px 1fr; align-items: center; border-bottom: 1px solid #f0f0f0; padding: 8px 0; }
.form-row.full-width { grid-column: 1 / -1; }
.form-title { font-weight: bold; color: #555; text-align: right; padding-right: 15px; }
.form-value { text-align: left; }
.form-row.single-row { grid-template-columns: 120px 1fr; }
.radio-group label { margin-right: 15px; }
textarea { width: 100%; min-height: 80px; padding: 8px; border: 1px solid #ddd; border-radius: 4px; }
.history-table { width: 100%; border-collapse: collapse; margin-top: 1rem; }
.history-table th, .history-table td { border: 1px solid #ddd; padding: 8px 12px; text-align: left; vertical-align: middle; font-size: 14px; }
.history-table th { background-color: #f2f2f2; font-weight: bold; }
</style>