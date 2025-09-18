<template>
  <div id="pageContentContainer">
    <div v-if="caseStore.isLoading" class="loading-overlay show">
      <div class="loading-spinner"></div>
      <div class="loading-text">正在查詢中...</div>
    </div>
    
    <div class="breadcrumbContainer">
      <ul>
        <li>首頁</li>
        <li>客戶盡職審查</li>
        <li class="now">案件管理</li>
      </ul>
    </div>
    <header>案件管理</header>

    <div class="formContainer">
      <div class="search-form-grid">
        <div class="form-row">
          <div class="form-title">分公司</div>
          <div class="form-value">
            <select v-model="caseStore.filters.branch" class="ui-style">
              <option>全部</option> <option>經紀本部</option> <option>數位分公司</option>
            </select>
          </div>
          <div class="form-title">投專</div>
          <div class="form-value">
            <select v-model="caseStore.filters.specialist" class="ui-style">
              <option>全部</option> <option>陳靜香</option> <option>林大熊</option>
            </select>
          </div>
          <div class="form-title">來源</div>
          <div class="form-value">
            <select v-model="caseStore.filters.source" class="ui-style">
              <option>全部</option> <option>證券開戶</option> <option>期貨開戶</option>
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-title">帳號</div>
          <div class="form-value">
            <input type="text" v-model="caseStore.filters.accountNumber" class="ui-style" placeholder="請輸入帳號">
          </div>
          <div class="form-title">身分證字號</div>
          <div class="form-value">
            <input type="text" v-model="caseStore.filters.idNumber" class="ui-style" placeholder="請輸入身分證字號">
          </div>
          <div class="form-title">日期區間</div>
          <div class="form-value date-range">
            <input type="date" v-model="caseStore.filters.dateFrom" class="ui-style">
            <span>—</span>
            <input type="date" v-model="caseStore.filters.dateTo" class="ui-style">
          </div>
        </div>
        <div class="form-row">
           <div class="form-title">審查進度</div>
           <div class="form-value">
            <select v-model="caseStore.filters.reviewStatus" class="ui-style">
              <option>全部</option> <option>簽核中</option> <option>未處理</option> <option>完成</option>
            </select>
           </div>
           <div class="form-title">處理人員</div>
           <div class="form-value">
            <select v-model="caseStore.filters.processor" class="ui-style">
              <option>全部</option> <option>林玉山</option>
            </select>
           </div>
           <div class="form-title">表單編號</div>
           <div class="form-value">
            <input type="text" v-model="caseStore.filters.formId" class="ui-style" placeholder="請輸入表單編號">
           </div>
        </div>
        <div class="form-row">
          <div class="form-title">加強審查</div>
          <div class="form-value radio-group">
              <label><input type="radio" value="all" v-model="caseStore.filters.enhancedReview"> 全部</label>
              <label><input type="radio" value="yes" v-model="caseStore.filters.enhancedReview"> 是</label>
              <label><input type="radio" value="no" v-model="caseStore.filters.enhancedReview"> 否</label>
          </div>
        </div>
      </div>
       <div class="action-buttons">
          <div class="search-summary">
            {{ caseStore.searchSummary }}
          </div>
          <div class="button-group">
            <button class="btn btn-grey" @click="openManualCaseModal">人工起單</button>
            <button class="btn btn-grey">個人待辦</button>
            <button class="btn btn-blue" @click="performSearch" :disabled="caseStore.isLoading">
              {{ caseStore.isLoading ? '查詢中...' : '查詢' }}
            </button>
          </div>
      </div>
    </div>

    <div class="formContainer">
        <div class="formSectionTitle">
            <label>案件列表</label>
            <button class="btn btn-sm btn-blue-outline">多筆傳送</button>
        </div>
        <div class="table-wrapper">
          <table class="data-table">
              <thead>
                  <tr>
                      <th><input type="checkbox" v-model="selectAll"></th>
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
              <tbody>
                  <tr v-if="caseStore.caseList.length === 0">
                    <td colspan="12" class="text-center p-4 text-muted">
                      {{ caseStore.searchSummary ? '沒有符合條件的資料' : '請點擊查詢按鈕以搜尋案件' }}
                    </td>
                  </tr>
                  <tr v-for="item in caseStore.caseList" :key="item.id">
                      <td><input type="checkbox" v-model="item.selected" v-if="item.selectable"></td>
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
                        <router-link v-if="item.detailsLink" :to="item.detailsLink">
                          <button class="btn-icon-edit" title="編輯"></button>
                        </router-link>
                        <button v-else class="btn-icon-edit-disabled" disabled title="不可編輯"></button>
                      </td>
                  </tr>
              </tbody>
          </table>
        </div>
        <div class="pager-container">
            <ul>
                <li class="prevArrow disabled"></li><li class="now">1</li><li class="nextArrow"></li>
            </ul>
        </div>
    </div>
    
    <div class="popup-overlay" v-if="isModalVisible">
      </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useCaseManagementStore } from '../stores/caseManagement';

const caseStore = useCaseManagementStore();

// (Modal 相關的 script 維持不變)
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
/* 【新增】發查狀態 Icon 樣式 */
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
  background-color: #ccc; /* 預設灰色 (pending) */
  user-select: none;
}
.status-icon.queried {
  background-color: #198754; /* 綠色 (queried) */
}

/* (其他原有樣式維持不變) */
.loading-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255, 255, 255, 0.8); display: flex; flex-direction: column; justify-content: center; align-items: center; z-index: 9999; opacity: 0; visibility: hidden; transition: opacity 0.3s, visibility 0.3s; }
.loading-overlay.show { opacity: 1; visibility: visible; }
.loading-spinner { border: 4px solid #f3f3f3; border-top: 4px solid #003366; border-radius: 50%; width: 40px; height: 40px; animation: spin 1s linear infinite; }
.loading-text { margin-top: 1rem; font-weight: 500; color: #003366; }
@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
#pageContentContainer { padding: 1rem; background-color: #f8f9fa; }
header { font-size: 24px; font-weight: 600; margin-bottom: 1rem; padding-bottom: 1rem; border-bottom: 1px solid #dee2e6; }
.breadcrumbContainer ul { list-style: none; padding: 0; margin: 0 0 1rem 0; display: flex; font-size: 14px; }
.breadcrumbContainer li { margin-right: 8px; }
.breadcrumbContainer li::after { content: '>'; margin-left: 8px; color: #6c757d; }
.breadcrumbContainer li:last-child::after { content: ''; }
.breadcrumbContainer li.now { color: #0d6efd; font-weight: 500; }
.formContainer { background-color: #fff; border: 1px solid #ddd; border-radius: 4px; margin-bottom: 1.5rem; padding: 1.5rem; }
.formSectionTitle { display: flex; justify-content: space-between; align-items: center; padding-bottom: 1rem; margin-bottom: 1rem; border-bottom: 1px solid #eee; }
.formSectionTitle label { font-size: 18px; font-weight: 600; }
.form-row { display: grid; grid-template-columns: 100px 1fr 100px 1fr 100px 1fr; gap: 1rem; align-items: center; padding: 0.5rem 0; }
.form-title { font-weight: bold; color: #555; text-align: right; }
.form-value { text-align: left; }
.search-form-grid { padding-bottom: 1rem; margin-bottom: 1rem; border-bottom: 1px dashed #ccc; }
.search-form-grid .form-row { border: none; }
.date-range { display: flex; align-items: center; gap: 0.5rem; }
.ui-style { width: 100%; padding: 8px 12px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; }
.radio-group label { margin-right: 15px; display: inline-flex; align-items: center; gap: 4px; font-weight: normal; }
.text-danger { color: #dc3545; font-weight: bold; }
.search-summary { font-size: 14px; color: red; font-weight: 500; min-height: 21px; }
.action-buttons { display: flex; justify-content: space-between; align-items: center; margin-top: 1rem; }
.button-group { display: flex; gap: 0.5rem; }
.btn { padding: 8px 16px; border: none; border-radius: 4px; cursor: pointer; font-weight: 500; transition: background-color 0.2s; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-blue { background-color: #003366; color: white; }
.btn-green { background-color: #198754; color: white; }
.btn-blue-outline { background-color: transparent; color: #003366; border: 1px solid #003366; }
.btn-grey { background-color: #6c757d; color: white; }
.table-wrapper { overflow-x: auto; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th, .data-table td { border: 1px solid #ddd; padding: 10px 12px; text-align: center; font-size: 14px; white-space: nowrap; vertical-align: middle; }
.data-table th { background-color: #f2f2f2; font-weight: bold; }
.btn-icon-edit, .btn-icon-edit-disabled { width: 20px; height: 20px; border: none; background-color: transparent; background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" fill="%23003366"><path d="M471.6 21.7c-21.9-21.9-57.3-21.9-79.2 0L362.3 51.7l97.9 97.9 30.1-30.1c21.9-21.9 21.9-57.3 0-79.2L471.6 21.7zm-299.2 220c-6.1 6.1-10.8 13.6-13.5 21.9l-29.6 88.8c-2.9 8.6-.6 18.1 5.8 24.6s15.9 8.7 24.6 5.8l88.8-29.6c8.2-2.7 15.7-7.4 21.9-13.5L437.7 172.3 339.7 74.3 172.4 241.7zM96 64C43 64 0 107 0 160V416c0 53 43 96 96 96H352c53 0 96-43 96-96V320c0-17.7-14.3-32-32-32s-32 14.3-32 32v96c0 17.7-14.3 32-32 32H96c-17.7 0-32-14.3-32-32V160c0-17.7 14.3-32 32-32h96c17.7 0 32-14.3 32-32s-14.3-32-32-32H96z"/></svg>'); background-size: contain; background-repeat: no-repeat; cursor: pointer; }
.btn-icon-edit-disabled { filter: grayscale(100%); opacity: 0.5; cursor: not-allowed; }
.pager-container { display: flex; justify-content: center; margin-top: 1.5rem; }
.pager-container ul { display: flex; list-style: none; padding: 0; }
.pager-container li { padding: 5px 12px; border: 1px solid #ddd; margin: 0 4px; cursor: pointer; border-radius: 4px; }
.pager-container li.now { background-color: #003366; color: white; border-color: #003366; }
.pager-container li.disabled { opacity: 0.5; cursor: not-allowed; }
.popup-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.popup-container { background: #fff; border-radius: 8px; box-shadow: 0 4px 15px rgba(0,0,0,0.2); width: 500px; max-width: 90%; }
.popup-header { display: flex; justify-content: space-between; align-items: center; padding: 1rem 1.5rem; border-bottom: 1px solid #eee; }
.popup-header h3 { margin: 0; font-size: 18px; }
.btn-close { border: none; background: none; font-size: 20px; cursor: pointer; }
.popup-body { padding: 1.5rem; }
.form-row-vertical { margin-bottom: 1rem; }
.form-row-vertical .form-title { text-align: left; margin-bottom: 0.5rem; display: block; }
.popup-footer { display: flex; justify-content: space-between; align-items: center; padding: 1rem 1.5rem; border-top: 1px solid #eee; }
.query-result-text { color: red; font-weight: 500; min-height: 21px; }
.mt-2 { margin-top: 0.5rem; }
</style>