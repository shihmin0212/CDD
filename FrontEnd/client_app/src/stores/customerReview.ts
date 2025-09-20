// src/stores/customerReview.ts

import { defineStore } from 'pinia';
import { ref, toRaw, computed } from 'vue';
import axios from 'axios';

// --- 介面定義 ---
export interface Attachment { id: number; type: string; fileName: string | null; file?: File | null; uploader?: string; uploadTime?: string; }
interface CustomerInfo { accountNumber: string; idNumber: string; name: string; nationality: string; contactPhone: string; mobilePhone: string; birthDate: string; age: number | null; identityType: string; occupationCategory: string; companyName: string; companyPhone: string; householdAddress: string; companyAddress: string; mailingAddress: string; bankAccount: string; investmentLimit: string; emergencyContact: string; emergencyContactPhone: string; tradingAgent: string; legalRepresentative: string; }
interface HistoryItem { user: string; department: string; status: string; startTime: string; endTime: string; memo: string; }
interface ReviewOption { id: string; text: string; isDefault?: boolean; alert?: 'EDD' | '婉拒'; }
interface ReviewSubField { id: string; type: 'text' | 'date'; label?: string; value: string; placeholder?: string; unit?: string; disabled: boolean; condition?: { onValue: string }; }
interface ReviewItem { id: string; title: string; type: 'checkbox' | 'radio' | 'group'; options?: ReviewOption[]; selected: string[] | string | null; remarks: string[]; disabled: boolean; subFields?: ReviewSubField[]; alert?: string; }
interface ReviewData { items: ReviewItem[]; notes: string; requiresEDD: 'yes' | 'no' | null; }

// --- API 呼叫函式 ---
const api = {
  getCustomerInfo: async (): Promise<CustomerInfo> => {
    console.log('📞 Calling API via Axios: GET /api/v1/crm/customerInfo');
    const response = await axios.get('/api/v1/crm/customerInfo');
    return response.data;
  },
  getReviewData: async (): Promise<ReviewData> => {
    console.log('📞 Calling API via Axios: GET /api/v1/crm/reviewItems');
    const response = await axios.get('/api/v1/crm/reviewItems');
    return response.data;
  },
  updateCase: async (action: string, data: any): Promise<any> => {
    console.log(`📞 Calling API via Axios: POST /api/v1/crm/case with action: ${action}`);
    return axios.post('/api/v1/crm/case', { action, payload: data });
  },
};

// --- Helper Functions ---
const getInitialCustomerInfo = (): CustomerInfo => ({ accountNumber: '', idNumber: '', name: '載入中...', nationality: '', contactPhone: '', mobilePhone: '', birthDate: '', age: null, identityType: '', occupationCategory: '', companyName: '', companyPhone: '', householdAddress: '', companyAddress: '', mailingAddress: '', bankAccount: '', investmentLimit: '', emergencyContact: '', emergencyContactPhone: '', tradingAgent: '', legalRepresentative: '' });
const getInitialReviewData = (): ReviewData => ({ items: [], notes: '', requiresEDD: null });

// --- 流程關卡定義 ---
const STAGES_CDD = [
  { title: '填表人', user: '', date: '', department: '經紀事業處' },
  { title: '後檯主管', user: '', date: '', department: '後檯作業處' },
  { title: '結案', user: '', date: '', department: '後檯作業處' },
];

const STAGES_EDD = [
  { title: '填表人', user: '', date: '', department: '經紀事業處' },
  { title: '營業員', user: '', date: '', department: '營業單位' },
  { title: '經理人', user: '', date: '', department: '營業單位' },
  { title: '後檯主管', user: '', date: '', department: '後檯作業處' },
  { title: '結案', user: '', date: '', department: '後檯作業處' },
];

// --- Store 定義 ---
export const useCustomerReviewStore = defineStore('customerReview', () => {
  const caseData = ref({
    currentStageIndex: 0,
    stages: [] as { title: string; user: string; date: string; department: string; }[],
    customerInfo: getInitialCustomerInfo(),
    reviewData: getInitialReviewData(),
    attachments: [] as Attachment[],
    reviewResult: 'Y',
    managerMemo: '',
  });

  const originalReviewData = ref<ReviewData | null>(null);
  const history = ref<HistoryItem[]>([]);
  const isLoading = ref(false);
  const stageActivatedTime = ref<string | null>(null);

  const getCurrentTimestamp = () => new Date().toLocaleString('sv-SE');

  const initData = async () => {
    isLoading.value = true;
    try {
      const [customerData, reviewData] = await Promise.all([ api.getCustomerInfo(), api.getReviewData() ]);
      caseData.value.customerInfo = customerData;
      caseData.value.reviewData = reviewData;
      originalReviewData.value = JSON.parse(JSON.stringify(reviewData));
      
      // 初始化時，先預設為 CDD 三流程
      caseData.value.stages = [...STAGES_CDD.map(s => ({...s}))];
      
      caseData.value.attachments = [ { id: Date.now(), type: '集保洗錢發查結果', fileName: '0801集保洗錢.pdf', uploader: '陳靜香(17654)', uploadTime: '2025/08/01 08:22:00' } ];
      history.value = [];
      caseData.value.currentStageIndex = 0;
      caseData.value.stages[0].user = '陳靜香(17654)';
      stageActivatedTime.value = getCurrentTimestamp();
    } catch (error) {
      console.error("初始化失敗:", error);
    } finally {
      isLoading.value = false;
    }
  };
  
  // 【修改後】此函式現在只根據 requiresEDD 的值來決定流程
  const updateWorkflowPath = () => {
    const isNowEdd = caseData.value.reviewData.requiresEDD === 'yes';
    const wasEdd = caseData.value.stages.length === STAGES_EDD.length;
    
    // 只有在流程需要變更，且在第一關時才執行
    if (wasEdd !== isNowEdd && caseData.value.currentStageIndex === 0) {
      console.log(`流程變更 -> 是否為EDD: ${isNowEdd}`);
      caseData.value.stages = isNowEdd ? [...STAGES_EDD.map(s => ({...s}))] : [...STAGES_CDD.map(s => ({...s}))];
      // 重置關卡人員資訊
      caseData.value.stages.forEach((stage, index) => {
        stage.user = '';
        stage.date = '';
        if (index === 0) stage.user = '陳靜香(17654)';
      });
    }
  };

  const sendToNext = async () => { 
    if (caseData.value.currentStageIndex >= caseData.value.stages.length - 1) return '';
    isLoading.value = true;
    try {
      const currentIndex = caseData.value.currentStageIndex;
      const fromStage = caseData.value.stages[currentIndex];
      const toStage = caseData.value.stages[currentIndex + 1];
      const timestamp = getCurrentTimestamp();
      await api.updateCase('傳送', toRaw(caseData.value));
      history.value.push({
        user: fromStage.user, department: fromStage.department, status: `處理完畢，傳送至 ${toStage.title}`,
        startTime: stageActivatedTime.value || '', endTime: timestamp, memo: caseData.value.managerMemo || '',
      });
      fromStage.date = new Date().toLocaleDateString('sv-SE');
      caseData.value.currentStageIndex++;
      const nextStage = caseData.value.stages[caseData.value.currentStageIndex];
      if (nextStage.title === '營業員') nextStage.user = '林大熊(17650)';
      else if (nextStage.title === '經理人') nextStage.user = '林玉山(01450)';
      else if (nextStage.title !== '結案') nextStage.user = '後檯主管(99999)';
      stageActivatedTime.value = timestamp;
      caseData.value.managerMemo = '';
      return `案件已傳送至 [${toStage.title}]`;
    } finally {
      isLoading.value = false;
    }
  };

  const returnToPrevious = async () => {
    if (caseData.value.currentStageIndex <= 0) return '';
    isLoading.value = true;
    try {
      const currentIndex = caseData.value.currentStageIndex;
      const fromStage = caseData.value.stages[currentIndex];
      const toStage = caseData.value.stages[currentIndex - 1];
      const timestamp = getCurrentTimestamp();
      await api.updateCase('退回', toRaw(caseData.value));
      history.value.push({
        user: fromStage.user, department: fromStage.department, status: `退回至 ${toStage.title}`,
        startTime: stageActivatedTime.value || '', endTime: timestamp, memo: caseData.value.managerMemo || '（退回）',
      });
      fromStage.user = '';
      fromStage.date = '';
      caseData.value.currentStageIndex--;
      caseData.value.stages[caseData.value.currentStageIndex].date = '';
      stageActivatedTime.value = timestamp;
      caseData.value.managerMemo = '';
      if (caseData.value.currentStageIndex === 0) {
        updateWorkflowPath();
      }
      return `案件已退回至 [${toStage.title}]`;
    } finally {
      isLoading.value = false;
    }
  };

  const save = async () => {
    isLoading.value = true;
    try {
      await api.updateCase('暫存', toRaw(caseData.value));
      return `資料已暫存！`;
    } finally {
      isLoading.value = false;
    }
  };
  
  const addAttachmentRow = () => { caseData.value.attachments.push({ id: Date.now(), type: '請選擇', fileName: null, file: null }); };
  const removeAttachment = (id: number) => { caseData.value.attachments = caseData.value.attachments.filter(att => att.id !== id); };
  const handleFileUpload = (id: number, file: File) => { const attachment = caseData.value.attachments.find(att => att.id === id); if (attachment) { attachment.file = file; attachment.fileName = file.name; attachment.uploader = '林大熊(17650)'; attachment.uploadTime = getCurrentTimestamp(); } };
  const resetReviewData = () => { if (originalReviewData.value) { caseData.value.reviewData = JSON.parse(JSON.stringify(originalReviewData.value)); } };

  return {
    caseData, history, isLoading, initData, sendToNext, returnToPrevious, save, resetReviewData, addAttachmentRow, removeAttachment, handleFileUpload, updateWorkflowPath,
  };
});