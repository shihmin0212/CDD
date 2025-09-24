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
  {
    id: "C001",
    formId: "DD20250425014",
    branch: "經紀本部",
    customerAccount: "尚未開戶",
    idNumber: "A123456789",
    customerName: "王曉明",
    salesperson: "陳靜香",
    reviewStatus: "簽核中",
    source: "證券開戶",
    enhancedReview: "否",
    applicationDate: "2025/08/01",
    lastModifiedTime: "2025/08/04 18:40",
    currentProcessor: "林玉山",
    amlStatus: "pending",
    b27Status: "pending",
    detailsLink: "./CustomerDueDiligence_Review_1.html",
    selectable: true,
    selected: false
  },
  {
    id: "C002",
    formId: "DD20250425013",
    branch: "經紀本部",
    customerAccount: "尚未開戶",
    idNumber: "A123456790",
    customerName: "張曉明",
    salesperson: "林大熊",
    reviewStatus: "簽核中",
    source: "證券開戶",
    enhancedReview: "否",
    applicationDate: "2025/08/01",
    lastModifiedTime: "2025/08/04 18:40",
    currentProcessor: "林玉山",
    amlStatus: "queried",
    b27Status: "queried",
    detailsLink: "./CustomerDueDiligence_Review_1.html",
    selectable: true,
    selected: false
  },
  {
    id: "C003",
    formId: "DD20250425012",
    branch: "經紀本部",
    customerAccount: "尚未開戶",
    idNumber: "A123456791",
    customerName: "李曉明",
    salesperson: "陳靜香",
    reviewStatus: "簽核中",
    source: "證券開戶",
    enhancedReview: "否",
    applicationDate: "2025/08/01",
    lastModifiedTime: "2025/08/04 18:40",
    currentProcessor: "林玉山",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: "./CustomerDueDiligence_Review_1.html",
    selectable: true,
    selected: false
  },
  {
    id: "C004",
    formId: null, // C# null
    branch: "經紀本部",
    customerAccount: "9801237",
    idNumber: "A123456792",
    customerName: "林曉明",
    salesperson: "林大熊",
    reviewStatus: "未處理",
    source: "期貨開戶",
    enhancedReview: "", // 空字串
    applicationDate: "2025/04/25",
    lastModifiedTime: "", // 空字串
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null, // C# null
    selectable: false,
    selected: null // C# null
  },
  {
    id: "C005",
    formId: null,
    branch: "經紀本部",
    customerAccount: "9801238",
    idNumber: "A123456793",
    customerName: "王大明",
    salesperson: "陳靜香",
    reviewStatus: "未處理",
    source: "期貨開戶",
    enhancedReview: "",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null,
    selectable: false,
    selected: null
  },
  {
    id: "C006",
    formId: null,
    branch: "經紀本部",
    customerAccount: "9801239",
    idNumber: "A123456794",
    customerName: "張大明",
    salesperson: "林大熊",
    reviewStatus: "未處理",
    source: "中風險定審",
    enhancedReview: "",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null,
    selectable: false,
    selected: null
  },
  {
    id: "C007",
    formId: null,
    branch: "經紀本部",
    customerAccount: "9801244",
    idNumber: "A123456795",
    customerName: "陳曉",
    salesperson: "陳靜香",
    reviewStatus: "未處理",
    source: "高風險定審",
    enhancedReview: "是",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null,
    selectable: false,
    selected: null
  },
  {
    id: "C008",
    formId: "DD20250425003",
    branch: "經紀本部",
    customerAccount: "9801245",
    idNumber: "A123456796",
    customerName: "王明月",
    salesperson: "林大熊",
    reviewStatus: "簽核中",
    source: "高風險定審",
    enhancedReview: "是",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: "./CustomerDueDiligence_Review_2.html",
    selectable: true,
    selected: false
  },
  {
    id: "C009",
    formId: null,
    branch: "經紀本部",
    customerAccount: "9801246",
    idNumber: "A123456797",
    customerName: "王曉花",
    salesperson: "陳靜香",
    reviewStatus: "未處理",
    source: "未成年轉正",
    enhancedReview: "",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null,
    selectable: false,
    selected: null
  },
  {
    id: "C010",
    formId: null,
    branch: "經紀本部",
    customerAccount: "9801247",
    idNumber: "A123456798",
    customerName: "王OO",
    salesperson: "林大熊",
    reviewStatus: "未處理",
    source: "期貨開戶",
    enhancedReview: "",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null,
    selectable: false,
    selected: null
  },
  {
    id: "C011",
    formId: null,
    branch: "經紀本部",
    customerAccount: "9801248",
    idNumber: "A123456799",
    customerName: "李OO",
    salesperson: "陳靜香",
    reviewStatus: "未處理",
    source: "信用開戶",
    enhancedReview: "",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null,
    selectable: false,
    selected: null
  },
  {
    id: "C012",
    formId: null,
    branch: "經紀本部",
    customerAccount: "9801249",
    idNumber: "A123456800",
    customerName: "陳OO",
    salesperson: "林大熊",
    reviewStatus: "未處理",
    source: "信用開戶",
    enhancedReview: "",
    applicationDate: "2025/08/01",
    lastModifiedTime: "",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: null,
    selectable: false,
    selected: null
  },
  {
    id: "C013",
    formId: "DD20250425002",
    branch: "經紀本部",
    customerAccount: "尚未開戶",
    idNumber: "A123456798",
    customerName: "張OO",
    salesperson: "陳靜香",
    reviewStatus: "完成",
    source: "期貨開戶",
    enhancedReview: "否",
    applicationDate: "2025/08/01",
    lastModifiedTime: "2025/08/01 19:45",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: "./CustomerDueDiligence_Review_1.html",
    selectable: false,
    selected: null
  },
  {
    id: "C014",
    formId: "DD20250425001",
    branch: "經紀本部",
    customerAccount: "尚未開戶",
    idNumber: "A123456798",
    customerName: "王OO",
    salesperson: "林大熊",
    reviewStatus: "完成",
    source: "期貨開戶",
    enhancedReview: "否",
    applicationDate: "2025/08/01",
    lastModifiedTime: "2025/08/01 19:45",
    currentProcessor: "-",
    amlStatus: "pending",
    b27Status: "queried",
    detailsLink: "./CustomerDueDiligence_Review_1.html",
    selectable: false,
    selected: null
  }
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