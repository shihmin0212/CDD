// vite.config.ts

import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
  ],
  resolve: {
    alias: {
      // 【新增】這行程式碼會告訴 Vite，'@' 這個別名代表的是 'src' 這個資料夾的路徑
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})