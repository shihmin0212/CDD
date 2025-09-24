import axios from 'axios';
import type { AxiosInstance } from 'axios';
import MockAdapter from 'axios-mock-adapter';
import type { CaseItem } from '@/stores/caseManagement'; // 假設類型定義在 Store 中

// 1. 讀取 .env.local 的設定
const baseURL = import.meta.env.VITE_API_BASE_URL;
const useFakeData = import.meta.env.VITE_ENABLE_FAKE_DATA === 'true';

// 2. 建立一個預先設定好 baseURL 的 axios 實例
const apiClient: AxiosInstance = axios.create({
  baseURL: baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 3. 【核心】根據開關決定是否啟用 Mock
if (useFakeData) {
  console.log('%c[API Client] 啟用 Mock 模式', 'color: orange; font-weight: bold;');
  const mock = new MockAdapter(apiClient, { delayResponse: 500 });

  // 在這裡定義所有 Mock API 的回應
  const mockCaseManagementDatabase: CaseItem[] = [
      { id: 'FAKE-001', formId: 'DD-FAKE-001', customerName: '王曉明 (來自模擬資料)', branch: '經紀本部', reviewStatus: '簽核中', amlStatus: 'queried', b27Status: 'queried', detailsLink: '#', applicationDate: '2025/09/16', customerAccount: 'FAKE-123', currentProcessor: '林玉山', enhancedReview: '是', idNumber: 'A123FAKE', lastModifiedTime: '2025/09/16 14:00', salesperson: '陳靜香', selectable: true, source: '證券開戶' },
      { id: 'FAKE-002', formId: 'DD-FAKE-002', customerName: '林曉明 (來自模擬資料)', branch: '數位分公司', reviewStatus: '未處理', amlStatus: 'pending', b27Status: 'pending', detailsLink: null, applicationDate: '2025/09/15', customerAccount: 'FAKE-456', currentProcessor: '-', enhancedReview: '否', idNumber: 'B223FAKE', lastModifiedTime: '', salesperson: '林大熊', selectable: false, source: '期貨開戶' },
  ];

  // 模擬 CaseManagement 的查詢 API
  mock.onPost('/CaseInfo/Search').reply(config => {
    console.log('📠 [Mock] 攔截到查詢請求:', JSON.parse(config.data));
    return [200, {
        status: true,
        data: mockCaseManagementDatabase,
        message: '成功從 Mock API 取得資料'
    }];
  });

  // 未來可以在此處加入其他 Mock API
  // mock.onGet('/Other/GetData').reply(200, { ... });

} else {
  console.log('%c[API Client] 啟用真實 API 模式', 'color: green; font-weight: bold;');
}


// 4. 設定攔截器 (Interceptors)
apiClient.interceptors.request.use(
  (config) => {
    console.log(`🚀 發送請求至: ${config.url}`);
    return config;
  },
  (error) => Promise.reject(error)
);

apiClient.interceptors.response.use(
  (response) => {
    console.log('✅ 收到回應:', response.data);
    return response;
  },
  (error) => {
    console.error('❌ API 發生錯誤:', error.response?.data || error.message);
    alert('API 發生錯誤，請查看 Console');
    return Promise.reject(error);
  }
);

// 5. 匯出這個 apiClient 實例
export default apiClient;