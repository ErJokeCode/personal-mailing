import tailwindcss from "@tailwindcss/vite";
import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import path from "path";

export default defineConfig({
    plugins: [svelte(), tailwindcss()],

    server: {
        host: "localhost",
        port: 5020,
    },

    resolve: {
        alias: {
            src: path.resolve('src/'),
        },
    }
})
