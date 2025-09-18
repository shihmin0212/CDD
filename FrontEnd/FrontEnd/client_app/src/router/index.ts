import { createRouter, createWebHistory } from 'vue-router';
import CustomerReview from '../components/CustomerReview.vue';
import CaseManagement from '../components/CaseManagement.vue';
import ProxyQuery from '../components/ProxyQuery.vue';

const routes = [
  {
    path: '/',
    redirect: '/customer-review' // 預設導向顧客盡職審查頁面
  },
  {
    // 案件管理頁面
    path: '/case-management',
    name: 'CaseManagement',
    component: CaseManagement,
    meta: { title: '案件管理' }
  },
  {
    // 顧客盡職審查頁面
    path: '/customer-review',
    name: 'CustomerReview',
    component: CustomerReview, // 直接使用 CustomerReview 元件
    meta: { title: '顧客盡職審查' }
  },
  {
    path: '/proxy-query',
    name: 'ProxyQuery',
    component: ProxyQuery,
    meta: { title: '當日代理查詢' }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;