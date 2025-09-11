import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tsconfigPaths from "vite-tsconfig-paths";
import tailwindcss from '@tailwindcss/vite';
import fs from 'fs';
import path from 'path';

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tsconfigPaths(),
    tailwindcss()
  ],
  server: {
   // https: {
  //    key: fs.readFileSync(path.resolve(__dirname, 'src/services/certs/localhost-key.pem')),
   //   cert: fs.readFileSync(path.resolve(__dirname, 'src/services/certs/localhost.pem')),
   // },
    allowedHosts: ['nicesait71front.serveo.net'],
    port: 5173,
  //  host: '0.0.0.0',
  },
  define: {
    //API_BASE_URL: JSON.stringify("https://localhost:5001"), // адрес твоего бэкенда
    API_BASE_URL: JSON.stringify("https://nicesait71.serveo.net"),
    BG: JSON.stringify("black"),
    COLOR: JSON.stringify("white")
  }
});