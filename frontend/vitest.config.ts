import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import AutoImport from 'unplugin-auto-import/vite'

export default defineConfig({
  plugins: [
    vue(),
    AutoImport({
      imports: ['vue'],
    }),
  ],
  test: {
    environment: 'jsdom',
    globals: true,
    css: false,
    include: ['app/tests/**/*.test.ts'],
    server: {
      deps: {
        inline: ['vuetify'],
      },
    },
  },
})
