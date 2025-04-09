import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tsconfigPaths from "vite-tsconfig-paths"
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tsconfigPaths(),
    tailwindcss()],
    server: {
      allowedHosts: ['nicesait71front.serveo.net']
    },
    define: {
      API_BASE_URL: JSON.stringify("https://nicesait71.serveo.net"),
      BG: JSON.stringify("black"),
      COLOR: JSON.stringify("white")
    }
})
