// src/stores/caseManagement.ts

import { defineStore } from 'pinia';
import { ref, reactive, toRaw } from 'vue';
// 【修改後】從我們新建的 Service 引入
import { caseManagementService } from '@/services/caseManagementService';

// 將介面定義放在這裡，或是一個共用的 types 檔案
export interface CaseItem { id: string; formId: string; branch: string; customerAccount: string; idNumber: string; customerName: string; salesperson: string; reviewStatus: string; source: string; enhancedReview: '是' | '否' | ''; applicationDate: string; lastModifiedTime: string; currentProcessor: string; amlStatus: 'queried' | 'pending'; b27Status: 'queried' | 'pending'; detailsLink: string | null; selectable: boolean; selected?: boolean; }
export interface SearchFilters { branch: string; specialist: string; source: string; accountNumber: string; idNumber: string; dateFrom: string; dateTo: string; reviewStatus: string; processor: string; enhancedReview: 'all' | 'yes' | 'no'; formId: string; }

export const useCaseManagementStore = defineStore('caseManagement', () => {
  const filters = reactive<SearchFilters>({
    branch: '全部', specialist: '全部', source: '全部', accountNumber: '', idNumber: '', dateFrom: '', dateTo: '', reviewStatus: '全部', processor: '全部', enhancedReview: 'all', formId: ''
  });
  
  const caseList = ref<CaseItem[]>([]);
  const isLoading = ref(false);
  const searchSummary = ref('');

  const searchCases = async () => {
    isLoading.value = true;
    caseList.value = [];
    searchSummary.value = '';
    
    try {
      // 【修改後】呼叫 Service 層的方法，邏輯變得非常乾淨
      const results = await caseManagementService.searchCases(toRaw(filters));
      
      caseList.value = results;
      searchSummary.value = `查詢結果共 ${results.length} 筆`;

    } catch (error: any) {
      // 錯誤已在 ApiClient 被 console.error 記錄
      // Store 只需要更新 UI 狀態來提示使用者
      searchSummary.value = error.message || '查詢時發生未知錯誤';
    } finally {
      isLoading.value = false;
    }
  };

  return {
    filters,
    caseList,
    isLoading,
    searchSummary,
    searchCases,
  };
});