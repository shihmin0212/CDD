import { createApp } from 'vue';
import { createPinia } from 'pinia';
import { createRouter, createWebHistory } from 'vue-router'; // 引入 Vue Router 的核心方法
import App from './App.vue';
import router from './router'; // 導入你的路由設定檔
import './style.css'; // 導入全域樣式
// 1. 引入 Mock 初始化函式
import { initializeMocks } from './mocks/apiMock';

// 2. 在建立 App 之前執行它
initializeMocks();

// 加入這兩行來引入 Bootstrap 的 CSS 和 JS
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css';


const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router); // 告訴 Vue 應用程式要使用這個路由實例
app.mount('#app');