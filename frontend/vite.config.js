import path from 'node:path'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: { '@': path.resolve(__dirname, './src') }, // import '@/components/...'
  },
  server: {
    port: 5173,
    // En desarrollo, /api se redirige al backend (evita problemas de CORS)
    proxy: { '/api': 'http://localhost:5000' },
  },
})
