import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import path from "path";

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [svelte()],
    server: {
        host: "0.0.0.0",
        port: 5010
    },
    resolve: {
        alias: {
            src: path.resolve('src/'),
        },
    }
})
