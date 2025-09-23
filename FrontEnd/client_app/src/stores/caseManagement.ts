// src/stores/caseManagement.ts

import { defineStore } from 'pinia';
import { ref, reactive, toRaw } from 'vue';
// [修改] 引入 caseManagementService 和新的 ManualCaseData 介面
import { caseManagementService, type ManualCaseData } from '@/services/caseManagementService';

// 介面定義
export interface CaseItem { id: string; /* ...其他欄位... */ }
export interface SearchFilters { /* ...篩選欄位... */ }
// [新增] 提交結果的介面
export interface SubmissionResult {
  success: boolean;
  message: string;
}

export const useCaseManagementStore = defineStore('caseManagement', () => {
  const filters = reactive<SearchFilters>({ /* ... */ });
  const caseList = ref<CaseItem[]>([]);
  const isLoading = ref(false);
  const searchSummary = ref('');

  // [新增] 用於追蹤 "建立" 按鈕的狀態
  const isSubmitting = ref(false);
  const submissionResult = ref<SubmissionResult | null>(null);

  const searchCases = async () => {
    isLoading.value = true;
    caseList.value = [];
    searchSummary.value = '';
    try {
      const results = await caseManagementService.searchCases(toRaw(filters));
      caseList.value = results;
      searchSummary.value = `查詢結果共 ${results.length} 筆`;
    } catch (error: any) {
      searchSummary.value = error.message || '查詢時發生未知錯誤';
    } finally {
      isLoading.value = false;
    }
  };

  // [新增] 建立案件的 Action
  const createCase = async (caseData: ManualCaseData) => {
    isSubmitting.value = true;
    submissionResult.value = null; // 重置結果
    try {
      // 呼叫 Service 層的方法
      const successMessage = await caseManagementService.createCase(caseData);
      
      // 更新成功結果
      submissionResult.value = { success: true, message: successMessage };

      // [重要] 建立成功後，自動重新查詢列表
      await searchCases(); 
      
    } catch (error: any) {
      // 更新失敗結果
      submissionResult.value = { success: false, message: error.message || '建立案件時發生未知錯誤' };
    } finally {
      isSubmitting.value = false;
    }
  };

  return {
    filters,
    caseList,
    isLoading,
    searchSummary,
    searchCases,
    // [新增] 匯出新的狀態和方法
    isSubmitting,
    submissionResult,
    createCase,
  };
});