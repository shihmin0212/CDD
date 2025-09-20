<template>
  <div class="formContainer">
    <div class="formSectionTitle">
      <label>檔案項目</label>
      <button class="btn btn-sm btn-green" @click="customerReviewStore.addAttachmentRow">新增項目</button>
    </div>
    <div class="table-wrapper">
      <table class="data-table file-upload-table">
        <thead>
          <tr>
            <th style="width: 25%;">檔案項目</th>
            <th style="width: 25%;">檔案名稱</th>
            <th style="width: 15%;">上傳人員</th>
            <th style="width: 20%;">上傳時間</th>
            <th style="width: 15%;" class="text-center">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in customerReviewStore.caseData.attachments" :key="item.id">
            <td>
              <span v-if="item.uploader">{{ item.type }}</span>
              <select v-else v-model="item.type" class="ui-style">
                <option disabled>請選擇</option>
                <option>證交所聯合徵信</option>
                <option>集保洗錢發查結果</option>
                <option>其他項目</option>
              </select>
            </td>
            <td>
              <a v-if="item.fileName" href="#" @click.prevent>{{ item.fileName }}</a>
              <span v-else class="text-muted">尚未上傳檔案</span>
            </td>
            <td>{{ item.uploader || '-' }}</td>
            <td>{{ item.uploadTime || '-' }}</td>
            <td class="text-center">
              <div class="button-group-sm">
                <button class="btn-icon-upload" title="上傳檔案" @click="triggerFileUpload(item.id)"></button>
                <input 
                  type="file" 
                  :ref="el => (fileInputs[item.id] = el)" 
                  @change="onFileSelected($event, item.id)" 
                  style="display: none;"
                />
                <button class="btn-icon-delete" title="刪除" @click="customerReviewStore.removeAttachment(item.id)"></button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useCustomerReviewStore } from '../stores/customerReview';

const customerReviewStore = useCustomerReviewStore();
const fileInputs = ref<Record<number, HTMLInputElement | null>>({});

const triggerFileUpload = (id: number) => {
  fileInputs.value[id]?.click();
};

const onFileSelected = (event: Event, id: number) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];
  if (file) {
    customerReviewStore.handleFileUpload(id, file);
  }
};
</script>

<style scoped>
/* 沿用主元件的樣式，這裡只加微調 */
.file-upload-table th, .file-upload-table td {
  vertical-align: middle;
}
.text-muted {
  color: #6c757d;
  font-style: italic;
}
.button-group-sm {
  display: inline-flex;
  gap: 0.5rem;
  align-items: center;
}
.btn-icon-upload, .btn-icon-delete {
  width: 20px;
  height: 20px;
  border: none;
  background-color: transparent;
  background-size: contain;
  background-repeat: no-repeat;
  cursor: pointer;
}
.btn-icon-upload {
  background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" fill="%230d6efd"><path d="M288 109.3V352c0 17.7-14.3 32-32 32s-32-14.3-32-32V109.3l-73.4 73.4c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3l128-128c12.5-12.5 32.8-12.5 45.3 0l128 128c12.5 12.5 12.5 32.8 0 45.3s-32.8 12.5-45.3 0L288 109.3zM64 352H192c0 35.3 28.7 64 64 64s64-28.7 64-64H448c35.3 0 64 28.7 64 64v32c0 35.3-28.7 64-64 64H64c-35.3 0-64-28.7-64-64V416c0-35.3 28.7-64 64-64z"/></svg>');
}
.btn-icon-delete {
  background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="%23dc3545"><path d="M135.2 17.7L128 32H32C14.3 32 0 46.3 0 64S14.3 96 32 96H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H320l-7.2-14.3C307.4 6.8 296.3 0 284.2 0H163.8c-12.1 0-23.2 6.8-28.6 17.7zM416 128H32L53.2 467c1.6 25.3 22.6 45 47.9 45H346.9c25.3 0 46.3-19.7 47.9-45L416 128z"/></svg>');
}

.btn-green {
  background-color: #198754;
  color: white;
}
</style>