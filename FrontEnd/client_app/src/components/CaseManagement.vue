<template>
  <div>
    <div v-if="caseStore.isLoading" class="loading-overlay show">
      <div class="loading-spinner"></div>
      <div class="loading-text">正在查詢中...</div>
    </div>

    <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="#">首頁</a></li>
        <li class="breadcrumb-item"><a href="#">客戶盡職審查</a></li>
        <li class="breadcrumb-item active" aria-current="page">案件管理</li>
      </ol>
    </nav>

    <header class="h4 fw-bold mb-3 esun-header">案件管理</header>

    <div class="card shadow-sm mb-4">
      <div class="card-body p-4">
        <div class="row g-3">
          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text">分公司</span>
              <select v-model="caseStore.filters.branch" class="form-select">
                <option>全部</option> <option>經紀本部</option> <option>數位分公司</option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text">投專</span>
              <select v-model="caseStore.filters.specialist" class="form-select">
                <option>全部</option> <option>陳靜香</option> <option>林大熊</option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text">來源</span>
              <select v-model="caseStore.filters.source" class="form-select">
                <option>全部</option> <option>證券開戶</option> <option>期貨開戶</option>
              </select>
            </div>
          </div>

          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text">帳號</span>
              <input type="text" v-model="caseStore.filters.accountNumber" class="form-control" placeholder="請輸入帳號">
            </div>
          </div>
          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text">身分證字號</span>
              <input type="text" v-model="caseStore.filters.idNumber" class="form-control" placeholder="請輸入身分證字號">
            </div>
          </div>
          <div class="col-md-4">
             <div class="input-group">
              <span class="input-group-text">日期區間</span>
              <input type="date" v-model="caseStore.filters.dateFrom" class="form-control">
              <input type="date" v-model="caseStore.filters.dateTo" class="form-control">
            </div>
          </div>
          
          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text">審查進度</span>
              <select v-model="caseStore.filters.reviewStatus" class="form-select">
                <option>全部</option> <option>簽核中</option> <option>未處理</option> <option>完成</option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text">處理人員</span>
               <select v-model="caseStore.filters.processor" class="form-select">
                <option>全部</option> <option>林玉山</option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
             <div class="input-group">
              <span class="input-group-text">表單編號</span>
              <input type="text" v-model="caseStore.filters.formId" class="form-control" placeholder="請輸入表單編號">
            </div>
          </div>

          <div class="col-12">
            <div class="input-group">
                <span class="input-group-text">加強審查</span>
                <div class="form-control d-flex align-items-center gap-3">
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" id="reviewAll" value="all" v-model="caseStore.filters.enhancedReview">
                        <label class="form-check-label" for="reviewAll">全部</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" id="reviewYes" value="yes" v-model="caseStore.filters.enhancedReview">
                        <label class="form-check-label" for="reviewYes">是</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" id="reviewNo" value="no" v-model="caseStore.filters.enhancedReview">
                        <label class="form-check-label" for="reviewNo">否</label>
                    </div>
                </div>
            </div>
          </div>
        </div>

        <hr class="my-4">

        <div class="d-flex justify-content-between align-items-center">
          <div class="text-danger fw-bold" style="min-height: 24px;">{{ caseStore.searchSummary }}</div>
          <div class="d-flex gap-2">
            <button class="btn btn-outline-secondary" @click="openManualCaseModal">人工起單</button>
            <button class="btn btn-outline-secondary">個人待辦</button>
            <button class="btn btn-primary" @click="performSearch" :disabled="caseStore.isLoading">
              <span v-if="caseStore.isLoading" class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
              {{ caseStore.isLoading ? '查詢中...' : '查詢' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h6 class="mb-0">案件列表</h6>
        <button class="btn btn-sm btn-outline-primary">多筆傳送</button>
      </div>
      <div class="table-responsive">
        <table class="table table-striped table-hover mb-0 align-middle" style="min-width: 1200px;">
          <thead class="table-light text-center">
            <tr>
              <th><input class="form-check-input" type="checkbox" v-model="selectAll"></th>
              <th>表單編號</th>
              <th>分公司</th>
              <th>顧客帳號</th>
              <th>顧客姓名</th>
              <th>營業員</th>
              <th>審查進度</th>
              <th>發查狀態</th>
              <th>申請日期</th>
              <th>目前處理人</th>
              <th>最後異動時間</th>
              <th>明細</th>
            </tr>
          </thead>
          <tbody class="text-center">
            <tr v-if="caseStore.caseList.length === 0">
              <td colspan="12" class="py-5 text-muted">
                {{ caseStore.searchSummary ? '沒有符合條件的資料' : '請點擊查詢按鈕以搜尋案件' }}
              </td>
            </tr>
            <tr v-for="item in caseStore.caseList" :key="item.id">
              <td><input class="form-check-input" type="checkbox" v-model="item.selected" v-if="item.selectable"></td>
              <td>
                <router-link v-if="item.formId" :to="item.detailsLink || ''">{{ item.formId }}</router-link>
                <span v-else>-</span>
              </td>
              <td>{{ item.branch }}</td>
              <td>{{ item.customerAccount }}</td>
              <td>{{ item.customerName }}</td>
              <td>{{ item.salesperson }}</td>
              <td>{{ item.reviewStatus }}</td>
              <td>
                <div class="status-icons">
                  <span class="status-icon" :class="item.amlStatus" title="AML集保洗錢">A</span>
                  <span class="status-icon" :class="item.b27Status" title="交易所B27">B</span>
                </div>
              </td>
              <td>{{ item.applicationDate }}</td>
              <td>{{ item.currentProcessor }}</td>
              <td>{{ item.lastModifiedTime }}</td>
              <td>
                <router-link v-if="item.detailsLink" :to="item.detailsLink" class="btn btn-sm btn-outline-secondary" title="編輯">
                   <i class="bi bi-pencil-square"></i>
                </router-link>
                 <button v-else class="btn btn-sm btn-outline-secondary" disabled title="不可編輯">
                   <i class="bi bi-pencil-square"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
       <div v-if="caseStore.totalPages > 1" class="card-footer d-flex justify-content-center">
         <nav aria-label="Page navigation">
            <ul class="pagination mb-0">
                <li class="page-item" :class="{ disabled: caseStore.currentPage === 1 }">
                    <a class="page-link" href="#" @click.prevent="caseStore.goToPage(caseStore.currentPage - 1)">&laquo;</a>
                </li>
                <li v-for="page in caseStore.totalPages" :key="page" class="page-item" :class="{ active: caseStore.currentPage === page }">
                    <a class="page-link" href="#" @click.prevent="caseStore.goToPage(page)">{{ page }}</a>
                </li>
                <li class="page-item" :class="{ disabled: caseStore.currentPage === caseStore.totalPages }">
                    <a class="page-link" href="#" @click.prevent="caseStore.goToPage(caseStore.currentPage + 1)">&raquo;</a>
                </li>
            </ul>
        </nav>
      </div>
    </div>

    <div class="popup-overlay" v-if="isModalVisible">
      </div>
  </div>
</template>

<script setup lang="ts">
// Script 邏輯幾乎不用變，因為我們只改了 Template
import { ref, reactive, computed } from 'vue';
import { useCaseManagementStore } from '../stores/caseManagement';

const caseStore = useCaseManagementStore();

const isModalVisible = ref(false);
const queryCompleted = ref(false);
const isQuerying = ref(false);
const queryResultText = ref('');
const manualCase = reactive({ branch: '經紀本部', account: '9805892', businessType: '證券', reviewReason: '定期檢視', otherReason: '' });
const resetModalState = () => { /* ... */ };
const openManualCaseModal = () => { resetModalState(); isModalVisible.value = true; };
const performModalQuery = async () => { /* ... */ };
const createCase = () => { /* ... */ };

const selectAll = computed({
  get: () => {
    const selectableItems = caseStore.caseList.filter(item => item.selectable);
    return selectableItems.length > 0 && selectableItems.every(item => item.selected);
  },
  set: (value) => {
    caseStore.caseList.forEach(item => {
      if (item.selectable) { item.selected = value; }
    });
  }
});

const performSearch = () => {
  caseStore.searchCases();
}
</script>

<style scoped>
/* 8. [簡化] 大幅減少自訂 CSS，只保留必要的 */

/* 通用樣式 */
.esun-header {
  border-left: 5px solid #006030;
  padding-left: 1rem;
}

.input-group-text {
  min-width: 100px; /* 統一標籤寬度 */
  justify-content: center;
}

/* 發查狀態 Icon 樣式 (保留) */
.status-icons {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 6px;
}
.status-icon {
  display: inline-flex;
  justify-content: center;
  align-items: center;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  font-size: 12px;
  font-weight: bold;
  color: white;
  background-color: #ccc; /* pending */
  user-select: none;
}
.status-icon.queried {
  background-color: #198754; /* green (queried) */
}

/* Loading Overlay 樣式 (保留) */
.loading-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255, 255, 255, 0.8); display: flex; flex-direction: column; justify-content: center; align-items: center; z-index: 9999; opacity: 0; visibility: hidden; transition: opacity 0.3s, visibility 0.3s; }
.loading-overlay.show { opacity: 1; visibility: visible; }
.loading-spinner { border: 4px solid #f3f3f3; border-top: 4px solid #003366; border-radius: 50%; width: 40px; height: 40px; animation: spin 1s linear infinite; }
.loading-text { margin-top: 1rem; font-weight: 500; color: #003366; }
@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }

/* Modal 相關樣式 (保留) */
.popup-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000; }
/* ... 您原有的 Modal CSS ... */
</style>