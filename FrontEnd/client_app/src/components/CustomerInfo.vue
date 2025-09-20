<template>
  <div class="formContainer">
    <div class="formSectionTitle collapsible" @click="$emit('toggle')">
      <label>顧客資訊</label>
      <span class="collapse-icon" :class="{ 'expanded': isExpanded }"></span>
    </div>
    <transition name="collapse">
      <div v-show="isExpanded" class="form-content customer-info-grid">
        <div class="form-row"><div class="form-title">顧客帳號</div><div class="form-value">{{ customerInfo.accountNumber }}</div><div class="form-title">身分證字號</div><div class="form-value">{{ customerInfo.idNumber }}</div></div>
        <div class="form-row"><div class="form-title">顧客姓名</div><div class="form-value">{{ customerInfo.name }}</div><div class="form-title">國籍</div><div class="form-value">{{ customerInfo.nationality }}</div></div>
        <div class="form-row"><div class="form-title">連絡電話</div><div class="form-value">{{ customerInfo.contactPhone }}</div><div class="form-title">行動電話</div><div class="form-value">{{ customerInfo.mobilePhone }}</div></div>
        <div class="form-row"><div class="form-title">出生年月日</div><div class="form-value">{{ customerInfo.birthDate }}</div><div class="form-title">年齡</div><div class="form-value">{{ customerInfo.age }}</div></div>
        <div class="form-row"><div class="form-title">身分類別</div><div class="form-value">{{ customerInfo.identityType }}</div><div class="form-title">職業類別</div><div class="form-value">{{ customerInfo.occupationCategory }}</div></div>
        <div class="form-row"><div class="form-title">公司名稱</div><div class="form-value">{{ customerInfo.companyName }}</div><div class="form-title">公司電話</div><div class="form-value">{{ customerInfo.companyPhone }}</div></div>
        <div class="form-row full-width"><div class="form-title">戶籍地址</div><div class="form-value">{{ customerInfo.householdAddress }}</div></div>
        <div class="form-row full-width"><div class="form-title">公司地址</div><div class="form-value">{{ customerInfo.companyAddress }}</div></div>
        <div class="form-row full-width"><div class="form-title">通訊地址</div><div class="form-value">{{ customerInfo.mailingAddress }}</div></div>
        <div class="form-row"><div class="form-title">銀行帳號</div><div class="form-value">{{ customerInfo.bankAccount }}</div><div class="form-title">證券投資上限</div><div class="form-value">{{ customerInfo.investmentLimit }}</div></div>
        <div class="form-row"><div class="form-title">緊急聯絡人</div><div class="form-value">{{ customerInfo.emergencyContact }}</div><div class="form-title">緊急連絡人電話</div><div class="form-value">{{ customerInfo.emergencyContactPhone }}</div></div>
        <div class="form-row"><div class="form-title">買賣代理人</div><div class="form-value">{{ customerInfo.tradingAgent }}</div><div class="form-title">法定代理人</div><div class="form-value">{{ customerInfo.legalRepresentative }}</div></div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  customerInfo: Record<string, any>;
  isExpanded: boolean;
}>();

defineEmits(['toggle']);
</script>

<style scoped>
/* 通用容器與標題樣式 */
.formContainer { margin-bottom: 1rem; background-color: #fff; border: 1px solid #ddd; border-radius: 8px; overflow: hidden; }
.formSectionTitle { padding: 1rem; background-color: #00a19b; font-size: 1.1rem; font-weight: 600; border-bottom: 1px solid #ddd; }.formSectionTitle.collapsible { cursor: pointer; display: flex; justify-content: space-between; align-items: center; user-select: none; }

/* 摺疊圖示與動畫 */
.collapse-icon { width: 16px; height: 16px; transition: transform 0.3s ease; background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="%23555"><path d="M201.4 342.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 274.7 86.6 137.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"/></svg>'); background-size: contain; background-repeat: no-repeat; background-position: center; }
.collapse-icon.expanded { transform: rotate(180deg); }
.collapse-enter-active, .collapse-leave-active { transition: all 0.3s ease-in-out; overflow: hidden; }
.collapse-enter-from, .collapse-leave-to { max-height: 0; opacity: 0; margin-top: -1rem; padding-top: 0; padding-bottom: 0; }
.collapse-enter-to, .collapse-leave-from { max-height: 1500px; opacity: 1; }

/* 內容樣式 */
.form-content { padding: 10px; margin-top: 1rem; }
.customer-info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0 20px; }
.form-row { display: grid; grid-template-columns: 120px 1fr; align-items: center; border-bottom: 1px solid #f0f0f0; padding: 8px 0; }
.form-row.full-width { grid-column: 1 / -1; }
.form-title { font-weight: bold; color: #555; text-align: right; padding-right: 15px; }
.form-value { text-align: left; }
</style>