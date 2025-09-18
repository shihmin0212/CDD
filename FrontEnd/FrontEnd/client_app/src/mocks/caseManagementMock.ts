// src/mocks/caseManagementMock.ts

import type MockAdapter from 'axios-mock-adapter';
import type { SearchFilters, CaseItem } from '../stores/caseManagement';

// 【修改後】擴充 mockDatabase 的欄位
const mockDatabase: CaseItem[] = [
    { 
        id: '1', formId: 'DD20250425014', branch: '經紀本部', customerAccount: '尚未開戶', idNumber: 'A123456789', 
        customerName: '王曉明', salesperson: '陳靜香', reviewStatus: '簽核中', source: '證券開戶', enhancedReview: '是', 
        applicationDate: '2025/08/01', lastModifiedTime: '2025/08/04 18:40', currentProcessor: '林玉山',
        amlStatus: 'queried', b27Status: 'queried',
        detailsLink: '/customer-review', selectable: true 
    },
    { 
        id: '2', formId: 'DD20250425013', branch: '經紀本部', customerAccount: '尚未開戶', idNumber: 'A123456790', 
        customerName: '張曉明', salesperson: '林大熊', reviewStatus: '簽核中', source: '證券開戶', enhancedReview: '否', 
        applicationDate: '2025/08/01', lastModifiedTime: '2025/08/04 18:40', currentProcessor: '林玉山',
        amlStatus: 'queried', b27Status: 'pending',
        detailsLink: '/customer-review', selectable: true 
    },
    { 
        id: '3', formId: '', branch: '數位分公司', customerAccount: '9801237', idNumber: 'B223456792', 
        customerName: '林曉明', salesperson: '林大熊', reviewStatus: '未處理', source: '期貨開戶', enhancedReview: '否', 
        applicationDate: '2025/04/25', lastModifiedTime: '', currentProcessor: '-',
        amlStatus: 'pending', b27Status: 'pending',
        detailsLink: null, selectable: false 
    },
    { 
        id: '4', formId: 'DD20250425011', branch: '南東分公司', customerAccount: '9801238', idNumber: 'F123456793', 
        customerName: '王大明', salesperson: '陳靜香', reviewStatus: '完成', source: '信用開戶', enhancedReview: '否', 
        applicationDate: '2025/08/01', lastModifiedTime: '2025/08/02 10:00', currentProcessor: '-',
        amlStatus: 'queried', b27Status: 'queried',
        detailsLink: '/customer-review', selectable: false 
    },
];

export const caseManagementMockHandlers = (mock: MockAdapter) => {
  mock.onPost('/api/v1/cases/search').reply(config => {
    const filters = JSON.parse(config.data) as SearchFilters;
    console.log('📠 Mock API (Case) 收到查詢請求:', filters);

    // 模擬篩選邏輯 (這裡只示範簡單篩選)
    const results = mockDatabase.filter(item => {
      const branchMatch = filters.branch === '全部' || item.branch === filters.branch;
      const statusMatch = filters.reviewStatus === '全部' || item.reviewStatus === filters.reviewStatus;
      return branchMatch && statusMatch;
    });
    
    const responseData = results.map(item => ({...item, selected: false }));
    return [200, responseData];
  });
};