// src/stores/types.ts

/**
 * 代理人查詢列表的單一項目資料結構
 */
export interface ProxyItem {
  branch: string;
  employeeId: string;
  name: string;
  agentId: string | null;
  applicationDate: string | null;
  lastModifiedUser: string | null;
  lastModifiedTime: string | null;
}

/**
 * 代理人選項的資料結構
 */
export interface AgentOption {
  id: string;
  name: string;
}

// 未來可以繼續增加其他 store 會用到的共用類型
// export interface CustomerProfile { ... }