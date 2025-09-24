// src/services/caseManagementService.ts

import apiClient from '@/api/ApiClient';
import type { AxiosResponse } from 'axios';
// [修改] 引入 ManualCaseData 介面，如果它不存在，我們就在這裡定義它
import type { CaseItem, SearchFilters } from '@/stores/caseManagement';

// [新增] 定義人工起單表單的資料介面
export interface ManualCaseData {
  formId: string; // 新增 formId
  branch: string;
  accountNumber: string;
  businessType: string;
  reviewReason: string;
  otherReason?: string;
}

// [新增] 定義來自後端 API 的成功回應介面
interface SuccessResponse {
  message: string;
}

// 定義 API 回應的標準格式
interface ApiResponse<T> {
  status: boolean;
  data: T;
  message?: string;
}

// 建立並匯出 Service 物件
export const caseManagementService = {
  /**
   * 查詢案件列表
   * @param filters - 篩選條件物件
   */
  async searchCases(filters: SearchFilters): Promise<CaseItem[]> {
    try {
      const response: AxiosResponse<ApiResponse<CaseItem[]>> = await apiClient.post(
        '/CaseInfo/Search', 
        filters
      );

      if (response.data && response.data.status) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || '後端回報查詢失敗');
      }
    } catch (error) {
      throw error;
    }
  },

  /**
   * [新增] 建立人工起單案件
   * @param caseData - 人工起單的表單資料
   */
  async createCase(caseData: ManualCaseData): Promise<string> {
    try {
      // 使用 apiClient 發送 POST 請求到新的端點
      const response: AxiosResponse<ApiResponse<SuccessResponse>> = await apiClient.post(
        '/CaseInfo/CreateForm', 
        caseData
      );

      if (response.data && response.data.status) {
        // API 回應成功，回傳成功訊息
        return response.data.message || '操作成功';
      } else {
        // API 請求成功，但後端告知操作失敗
        throw new Error(response.data.message || '後端回報建立失敗');
      }
    } catch (error) {
      // 向上拋出錯誤，讓呼叫者 (Store) 也能處理
      throw error;
    }
  },
};