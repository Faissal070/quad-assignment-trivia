import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
    plugins: [
        tailwindcss(),
        plugin()
    ],
    server: {
        proxy: {
            '/api': {
                target: 'https://trivia-api-oex0.onrender.com',
                changeOrigin: true,
                rewrite: p => p.replace(/^\/api/, '')
            }
        }
    }
})