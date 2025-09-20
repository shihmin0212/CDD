<template>
  <div>
    <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="#">首頁</a></li>
        <li class="breadcrumb-item"><a href="#">客戶盡職審查</a></li>
        <li class="breadcrumb-item active" aria-current="page">當日代理維護管理</li>
      </ol>
    </nav>

    <header class="h4 fw-bold mb-3 esun-header">當日代理查詢</header>

    <div class="card p-3 bg-light border-0 shadow-sm mb-3">
      <div class="row g-3 align-items-center">
        <div class="col-md-5">
          <div class="input-group">
            <span class="input-group-text">分公司</span>
            <select class="form-select" v-model="proxyQueryStore.searchForm.branch">
              <option v-for="branch in proxyQueryStore.branchOptions" :key="branch" :value="branch">
                {{ branch || '全部' }}
              </option>
            </select>
          </div>
        </div>
        <div class="col-md-5">
           <div class="input-group">
            <span class="input-group-text">人員</span>
            <select class="form-select" v-model="proxyQueryStore.searchForm.personnel">
               <option v-for="person in proxyQueryStore.personnelOptions" :key="person" :value="person">
                {{ person || '全部' }}
              </option>
            </select>
          </div>
        </div>
        <div class="col-md-2">
          <button class="btn btn-primary w-100" @click="handleSearch">
            <i class="bi bi-search me-1"></i>查詢
          </button>
        </div>
      </div>
    </div>
    
    <p class="text-danger small">* 若未指定代理則已銀行HR差勤系統為主</p>

    <div class="card shadow-sm">
      <div class="card-header h6">
        當日代理列表
      </div>
      <div class="table-responsive">
        <table class="table table-striped table-hover mb-0" style="min-width: 1200px;">
          <thead class="table-light">
            <tr>
              <th scope="col" class="text-center">序號</th>
              <th scope="col">分公司</th>
              <th scope="col">員編</th>
              <th scope="col">姓名</th>
              <th scope="col">指定代理人員編</th>
              <th scope="col">指定代理人姓名</th>
              <th scope="col">申請日期</th>
              <th scope="col">最後異動人員</th>
              <th scope="col">最後異動時間</th>
              <th scope="col" class="text-center">編輯</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in proxyQueryStore.paginatedList" :key="item.employeeId">
              <td class="text-center">{{ (proxyQueryStore.currentPage - 1) * proxyQueryStore.itemsPerPage + index + 1 }}</td>
              <td>{{ item.branch }}</td>
              <td>{{ item.employeeId }}</td>
              <td>{{ item.name }}</td>
              
              <template v-if="!proxyQueryStore.editingRows[item.employeeId]">
                <td>{{ item.agentId || '-' }}</td>
                <td>{{ proxyQueryStore.getAgentName(item.agentId) || '-' }}</td>
                <td>{{ item.applicationDate || '-' }}</td>
              </template>
              <template v-else>
                <td>
                  <select class="form-select form-select-sm" v-model="item.agentId">
                    <option :value="null">- 請選擇 -</option>
                    <option v-for="agent in proxyQueryStore.agentOptions" :key="agent.id" :value="agent.id">
                      {{ agent.name }}({{ agent.id }})
                    </option>
                  </select>
                </td>
                <td>{{ proxyQueryStore.getAgentName(item.agentId) || '-' }}</td>
                <td><input type="date" class="form-control form-control-sm" v-model="item.applicationDate"></td>
              </template>

              <td>{{ item.lastModifiedUser || '-' }}</td>
              <td>{{ item.lastModifiedTime || '-' }}</td>
              <td class="text-center">
                <div class="btn-group btn-group-sm">
                  <button v-if="!proxyQueryStore.editingRows[item.employeeId]" class="btn btn-outline-secondary" @click="editRow(item.employeeId)">
                    <i class="bi bi-pencil-square"></i>
                  </button>
                  <template v-else>
                    <button class="btn btn-outline-success" @click="saveRow(item)">
                      <i class="bi bi-check-lg"></i>
                    </button>
                    <button class="btn btn-outline-danger" @click="cancelEdit(item.employeeId)">
                      <i class="bi bi-x-lg"></i>
                    </button>
                  </template>
                </div>
              </td>
            </tr>
            <tr v-if="proxyQueryStore.paginatedList.length === 0">
              <td colspan="10" class="text-center text-muted py-5">查無資料</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    
    <nav v-if="proxyQueryStore.totalPages > 1" class="d-flex justify-content-center mt-4">
      <ul class="pagination">
        <li class="page-item" :class="{ disabled: proxyQueryStore.currentPage === 1 }">
          <a class="page-link" href="#" @click.prevent="goToPage(proxyQueryStore.currentPage - 1)">&laquo;</a>
        </li>
        <li v-for="page in proxyQueryStore.totalPages" :key="page" class="page-item" :class="{ active: proxyQueryStore.currentPage === page }">
          <a class="page-link" href="#" @click.prevent="goToPage(page)">{{ page }}</a>
        </li>
        <li class="page-item" :class="{ disabled: proxyQueryStore.currentPage === proxyQueryStore.totalPages }">
          <a class="page-link" href="#" @click.prevent="goToPage(proxyQueryStore.currentPage + 1)">&raquo;</a>
        </li>
      </ul>
    </nav>

    <MessageBox :show="showMessage" :message="message" @close="closeMessage" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import MessageBox from './MessageBox.vue';
import { useProxyQueryStore, type ProxyItem } from '../stores/proxyQueryStore';

const proxyQueryStore = useProxyQueryStore();

// 彈窗狀態
const message = ref('');
const showMessage = ref(false);

// 方法
const handleSearch = () => proxyQueryStore.handleSearch();
const editRow = (employeeId: string) => proxyQueryStore.editRow(employeeId);
const saveRow = (item: ProxyItem) => proxyQueryStore.saveRow(item).then((res) => showMessageBox(res));
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

<style scoped>
/* 模擬玉山樣式的 header 左側綠色 border */
.esun-header {
  border-left: 5px solid #006030;
  padding-left: 1rem;
}

/* 調整 input group 文字寬度，使其對齊 */
.input-group-text {
  min-width: 80px;
  justify-content: center;
}
</style>