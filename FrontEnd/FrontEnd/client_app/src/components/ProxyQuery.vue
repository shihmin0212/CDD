<template>
  <div id="pageContentContainer">
    <div class="breadcrumbContainer">
      <ul>
        <li>首頁</li>
        <li>客戶盡職審查</li>
        <li class="now">當日代理維護管理</li>
      </ul>
    </div>
    <header>當日代理查詢</header>
    <div id="problemCaseManager_UnsolvedForm">
      <!-- Search From -->
      <div id="unsolvedSearchFormContainer" class="formContainer fillForm more">
        <div class="formRow set4">
          <div class="formCell formTitle">分公司</div>
          <div class="formCell">
            <div class="selectField">
              <select v-model="proxyQueryStore.searchForm.branch">
                <option value="">請選擇</option>
                <option v-for="branch in proxyQueryStore.branchOptions" :key="branch" :value="branch">{{ branch }}</option>
              </select>
            </div>
          </div>
          <div class="formCell formTitle">人員</div>
          <div class="formCell">
            <div class="selectField">
              <select v-model="proxyQueryStore.searchForm.personnel">
                <option value="">請選擇</option>
                <option v-for="person in proxyQueryStore.personnelOptions" :key="person" :value="person">{{ person }}</option>
              </select>
            </div>
          </div>
        </div>
        <!-- /formRow -->
        <div id="unsolvedSearchBtnContainer">
          <button class="uiStyle sizeM btnDarkBlue withIcon" id="btnUnsolvedSearch" @click="handleSearch">
            <img src="data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='white'><path d='M15.5 14h-.79l-.28-.27A6.471 6.471 0 0 0 16 9.5 6.5 6.5 0 1 0 9.5 16a6.471 6.471 0 0 0 3.73-1.28l.27.28v.79l5 4.99L20.49 19.5l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z'></path></svg>" title="查詢" />查詢
          </button>
        </div>
      </div>
      <div class="search-note">
        * 若未指定代理則已銀行HR差勤系統為主
      </div>
      <div id="applicationInfo_PersonalInfo" class="formContainer fillForm more">
        <!-- 當日代理列表 From -->
        <div class="formContainer listForm" style="margin-bottom: 0px;">
          <div class="formSectionTitle">
            <label>當日代理列表</label>
          </div>
        </div>
        <!-- airportInquiryResult -->
        <div id="airportInquiryResult_UsedList">
          <div>
            <div id="unsolved_ListForm" class="formContainer listForm">
              <!-- Form Top -->
              <div class="formRow formTitle">
                <div class="formCell">序號</div>
                <div class="formCell">分公司</div>
                <div class="formCell">員編</div>
                <div class="formCell">姓名</div>
                <div class="formCell">指定代理人員編</div>
                <div class="formCell">指定代理人姓名</div>
                <div class="formCell">申請日期</div>
                <div class="formCell">最後異動人員</div>
                <div class="formCell">最後異動時間</div>
                <div class="formCell">編輯</div>
              </div>
              <div v-for="(item, index) in proxyQueryStore.paginatedList" :key="item.employeeId" class="formRow" :id="'application_ApplicationInfoForm' + (index + 1)">
                <div class="formCell">{{ (proxyQueryStore.currentPage - 1) * proxyQueryStore.itemsPerPage + index + 1 }}</div>
                <div class="formCell">{{ item.branch }}</div>
                <div class="formCell">{{ item.employeeId }}</div>
                <div class="formCell">{{ item.name }}</div>
                <div class="formCell">
                  <div class="dropdownBtnS" :class="{ 'disabled': !proxyQueryStore.editingRows[item.employeeId] }">
                    <span v-if="!proxyQueryStore.editingRows[item.employeeId]">{{ item.agentId || '-' }}</span>
                    <select v-else class="dropdownBoxS SaveSet" v-model="item.agentId">
                      <option value="">-</option>
                      <option v-for="agent in proxyQueryStore.agentOptions" :key="agent.id" :value="agent.id">{{ agent.id }}</option>
                    </select>
                  </div>
                </div>
                <div class="formCell">{{ proxyQueryStore.getAgentName(item.agentId) || '-' }}</div>
                <div class="formCell">{{ item.applicationDate }}</div>
                <div class="formCell">{{ item.lastModifiedUser || '-' }}</div>
                <div class="formCell">{{ item.lastModifiedTime || '-' }}</div>
                <div class="formCell">
                  <div v-if="!proxyQueryStore.editingRows[item.employeeId]">
                    <button class="btnListEdit" :id="'btnApplicationFormEdit' + (index + 1)" @click="editRow(item.employeeId)"></button>
                  </div>
                  <div v-else>
                    <button class="btnListSave SaveSet" :id="'btnApplicationFormESave' + (index + 1)" @click="saveRow(item)"></button>
                    <button class="btnListCancel SaveSet" :id="'btnApplicationFormECancel' + (index + 1)" @click="cancelEdit(item.employeeId)"></button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- Pager -->
    <div class="pagerContainer totalPageMore">
      <ul class="show">
        <li v-for="page in proxyQueryStore.totalPages" :key="page" :class="{ 'now': proxyQueryStore.currentPage === page }" @click="goToPage(page)">
          {{ page }}
        </li>
      </ul>
    </div>
    <!-- 彈窗元件 -->
    <MessageBox :show="showMessage" :message="message" @close="closeMessage" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import MessageBox from './MessageBox.vue';
import { useProxyQueryStore } from '../stores/proxyQueryStore';

const proxyQueryStore = useProxyQueryStore();

// 彈窗狀態
const message = ref('');
const showMessage = ref(false);

// 方法
const handleSearch = () => proxyQueryStore.handleSearch();
const editRow = (employeeId: string) => proxyQueryStore.editRow(employeeId);
const saveRow = (item: any) => proxyQueryStore.saveRow(item).then((res) => showMessageBox(res));
const cancelEdit = (employeeId: string) => proxyQueryStore.cancelEdit(employeeId).then((res) => showMessageBox(res));
const goToPage = (page: number) => proxyQueryStore.goToPage(page);

const showMessageBox = (msg: string) => {
  message.value = msg;
  showMessage.value = true;
};

const closeMessage = () => {
  showMessage.value = false;
};

onMounted(() => {
  proxyQueryStore.mockFetchData();
});
</script>
