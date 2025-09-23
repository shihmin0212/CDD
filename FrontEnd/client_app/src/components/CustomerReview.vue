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

<FloatingHelper title="自然人附件說明">
  <div class="table-responsive">
    <table class="table table-bordered table-sm attachment-info-table">
      <thead class="table-light">
        <tr>
          <th style="width: 25%;">項目</th>
          <th style="width: 37.5%;">說明</th>
          <th style="width: 37.5%;">檢附文件</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(info, index) in naturalPersonAttachmentInfo" :key="index">
          <td class="fw-bold">{{ info.item }}</td>
          <td>{{ info.description }}</td>
          <td>{{ info.documents }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</FloatingHelper>
    
      <nav aria-label="breadcrumb">
      </nav>

      <div class="sticky-header">
      </div>

      <div class="content-body">
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
import FloatingHelper from './FloatingHelper.vue';

// 【新增】定義附件說明的資料
const naturalPersonAttachmentInfo = [
  {
    item: '市場上開立多戶',
    description: '(1) 證券及國外商品開立9戶(含)以上\n(2) 信用開立4戶(含)以上\n(3) 期貨開立5戶(含)以上',
    documents: '1. 證券、信用及海外商品：請檢附B27資料\n2. 期貨：請檢附交易人開戶家數查詢'
  },
  {
    item: '與金融相關之負面新聞或重大案件',
    description: 'RC100',
    documents: '' // 此處無內容
  },
  {
    item: '顧客或買賣代理人年齡已達65歲以上',
    description: '對標出生年、月、日，滿65歲',
    documents: '符合高齡者須填寫高齡友善關懷辨識問券，除來源為線上開戶，系統會自動起單以外，其餘皆檢附高齡友善關懷問券'
  },
  {
    item: '職業是否為說明欄所述之項目',
    description: '(1) 職業項目為12、15、16、22~34、36~37者。\n(2) 職業項目為其他，且無填寫公司相關欄位。\n(3) 指定非金融事業或人員：不動產業、律師、會計師、代書(地政士)、銀樓業、公證人、信託(非金融之信託業者)及公司服務提供業註。\n註：\n1.關於法人之籌備或設立事項。\n2.擔任或安排他人擔任公司董事或秘書、合夥之合夥人或在其他法人組織之類似職位。\n3.提供公司、合夥、信託、其他法人或協議註冊之辦公室、營業地址、居住所、通訊或管理地址。\n4.擔任或安排他人擔任信託或其他類似契約性質之受託人或其他相同角色。\n5擔任或安排他人擔任實質持股股東。',
    documents: '' // 此處無內容
  },
  {
    item: '有擔任買賣代理人且代理多戶或顧客之買賣代理人是否代理多戶',
    description: '(1) 證券代理10戶(含)以上。\n(2) 期貨及海外商品代理5戶(含)以上。',
    documents: '1. 證券：【w_esrf6買賣代理戶數查詢表】、【w_csry1買賣代理人清冊】。\n2. 期貨：【KCCSR199被授權人資料清冊】。\n3. 複委託：【B9-01本人、代理人、實質受益人及關係人查詢表】。\n4. OSU：【B9-01本人、代理人、實質受益人及關係人查詢表】。'
  }
];

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

/* 【新增】附件說明表格的專屬樣式 */
.attachment-info-table td {
  white-space: pre-line; /* 讓 \n 換行符號生效 */
  font-size: 0.85rem;    /* 讓字體小一點，容納更多內容 */
  vertical-align: middle;
}
</style>