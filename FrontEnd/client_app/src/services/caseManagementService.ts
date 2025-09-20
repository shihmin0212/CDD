import apiClient from '@/api/ApiClient';
import type { AxiosResponse } from 'axios';
import type { CaseItem, SearchFilters } from '@/stores/caseManagement';

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
      // 使用 apiClient 發送 POST 請求
      const response: AxiosResponse<ApiResponse<CaseItem[]>> = await apiClient.post(
        '/DueDiligence/Search', 
        filters
      );

      if (response.data && response.data.status) {
        return response.data.data; // 成功時，回傳資料陣列
      } else {
        // API 請求成功，但後端告知操作失敗
        throw new Error(response.data.message || '後端回報查詢失敗');
      }
    } catch (error) {
      // 向上拋出錯誤，讓呼叫者 (Store) 也能處理
      throw error;
    }
  },
};