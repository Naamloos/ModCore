import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { env } from 'process';
import tailwindcss from '@tailwindcss/vite';

// Fallback to ASPNETCORE_URLS, or an HTTP environment variable if available, else default HTTP port
const target = env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(';')[0]
    : 'http://localhost:5162';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin(), tailwindcss()],
    resolve: {
        alias: {
            // FIX: Added the trailing slash to the target path string so folder mapping evaluates cleanly
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/api/token': {
                target,
                secure: false // Keeps proxying simple over HTTP
            }
        },
        port: parseInt(env.DEV_SERVER_PORT || '53552'),
        allowedHosts: [
            'modcoredev.nudes.zip'
        ]
    }
});