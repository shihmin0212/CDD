import axios from 'axios';
import MockAdapter from 'axios-mock-adapter';
import { workflowMockHandlers } from './workflowMock';
import { caseManagementMockHandlers } from './caseManagementMock';


export const initializeMocks = () => {
  // 建立一個共用的 Mock Adapter 實例
  const mock = new MockAdapter(axios, { delayResponse: 800 });
  
  console.log(' initializing all API mocks...');

  // 註冊來自不同模組的 mock 處理器
  workflowMockHandlers(mock);
  caseManagementMockHandlers(mock);


  // 您也可以在這裡加入一個"通配符"規則來處理所有未匹配的請求
  mock.onAny().passThrough(); // 對於沒有 mock 的請求，讓它正常發送 (在真實開發中很有用)
};