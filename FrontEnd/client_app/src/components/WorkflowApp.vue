<template>
  <div class="app-container">
    <div class="sidebar">
      <div class="sidebar-header">
        <h1>系統選單</h1>
      </div>
      <nav class="menu-list">
        <div 
          class="menu-item" 
          @click="activeMenu = 'customer_due_diligence'" 
          :class="{ active: activeMenu === 'customer_due_diligence' }"
        >
          顧客盡職審查
        </div>
        <div 
          class="menu-item" 
          @click="activeMenu = 'proxy_query'" 
          :class="{ active: activeMenu === 'proxy_query' }"
        >
          當日代理查詢
        </div>
      </nav>
    </div>

    <div class="main-content">
      <div v-if="activeMenu === 'customer_due_diligence'">
        <h1>客戶盡職審查系統 (Vue 3 Demo)</h1>

        <div v-if="isLoading" class="loading">
          正在從後端讀取案件資料...
        </div>

        <div v-else>
          <!-- 進度條區塊 -->
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

          <!-- 按鈕區塊 -->
          <div class="action-buttons">
            <button class="btn-red" @click="returnCase" :disabled="customerReviewStore.caseData.currentStageIndex === 0 || customerReviewStore.isSavingHistory.value">退回</button>
            <button class="btn-blue" @click="save" :disabled="customerReviewStore.isSavingHistory.value">暫存</button>
            <button class="btn-green" @click="sendCase" :disabled="customerReviewStore.isLastStep || customerReviewStore.isSavingHistory.value">傳送</button>
          </div>

          <!-- 表單區塊 -->
          <div class="form-section">
            <div class="form-section-title">顧客資訊</div>
            <div class="form-content">
              <div class="form-row">
                <div class="form-title">顧客姓名：</div>
                <div class="form-value">{{ customerReviewStore.caseData.customerInfo.name }}</div>
              </div>
              <div class="form-row">
                <div class="form-title">身分證號：</div>
                <div class="form-value">{{ customerReviewStore.caseData.customerInfo.idNumber }}</div>
              </div>
            </div>
          </div>
          
          <div class="form-section">
            <div class="form-section-title">單位主管(經理人)審核</div>
            <div class="form-content">
              <div class="form-row">
                <div class="form-title">審核結果：</div>
                <div class="radio-group form-value">
                  <label><input type="radio" value="Y" v-model="customerReviewStore.caseData.reviewResult" :disabled="!customerReviewStore.isManagerStage"> 同意</label>
                  <label><input type="radio" value="N" v-model="customerReviewStore.caseData.reviewResult" :disabled="!customerReviewStore.isManagerStage"> 不同意</label>
                </div>
              </div>
              <div class="form-row">
                <div class="form-title">簽核意見：</div>
                <div class="form-value">
                  <textarea v-model="customerReviewStore.caseData.managerMemo" :disabled="!customerReviewStore.isManagerStage" placeholder="請在此輸入簽核意見..."></textarea>
                </div>
              </div>
            </div>
          </div>

          <!-- 流程紀錄區塊 -->
          <div class="history-section form-section">
            <div class="form-section-title">流程紀錄</div>
            <ul class="history-list">
              <li v-for="(log, index) in customerReviewStore.history" :key="index" class="history-item">
                <span class="history-action">{{ log.action }}</span>
                <span>從 [{{ log.fromStage }}] 傳送/退回至 [{{ log.toStage }}]</span>
                <span class="history-date">{{ log.timestamp }}</span>
              </li>
              <li v-if="customerReviewStore.history.length === 0" class="history-item" style="justify-content: center; color: #999;">
                尚無任何流程紀錄。
              </li>
            </ul>
          </div>
        </div>
      </div>
      
      <div v-else-if="activeMenu === 'proxy_query'">
        <h2>當日代理查詢</h2>
        <p>此處為「當日代理查詢」頁面的內容。</p>
      </div>

    </div>
    
    <!-- 讀取狀態提示 -->
    <div v-if="customerReviewStore.isSavingHistory.value" class="loading-overlay">
      <div class="loading-spinner"></div>
      <p>正在處理中...</p>
    </div>

    <!-- 自訂訊息視窗 -->
    <MessageBox
      :message="message"
      :show="showMessage"
      @close="closeMessage"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useCustomerReviewStore } from '../stores/customerReview';
import MessageBox from './MessageBox.vue';

const isLoading = ref(true);
const message = ref('');
const showMessage = ref(false);
const activeMenu = ref('customer_due_diligence');

const customerReviewStore = useCustomerReviewStore();

const getStageClass = (index: number) => {
  if (index < customerReviewStore.caseData.currentStageIndex) return 'done';
  if (index === customerReviewStore.caseData.currentStageIndex) return 'now';
  return 'next';
};

const showMessageBox = (msg: string) => {
  message.value = msg;
  showMessage.value = true;
};

const closeMessage = () => {
  showMessage.value = false;
};

const save = () => {
  showMessageBox(`資料已暫存！\n審核結果: ${customerReviewStore.caseData.reviewResult}\n簽核意見: ${customerReviewStore.caseData.managerMemo}`);
};

const sendCase = async () => {
  const msg = await customerReviewStore.sendToNext();
  if (msg) {
    showMessageBox(msg);
  }
};

const returnCase = async () => {
  const msg = await customerReviewStore.returnToPrevious();
  if (msg) {
    showMessageBox(msg);
  }
};

onMounted(() => {
  customerReviewStore.loadCaseData();
  setTimeout(() => {
    isLoading.value = false;
  }, 1500);
});
</script>