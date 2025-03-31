import tailwindcss from "@tailwindcss/vite";
import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import path from "path";

export default defineConfig({
    plugins: [svelte(), tailwindcss()],

    server: {
        host: "0.0.0.0",
        port: 4000,
    },

    preview: {
        host: "0.0.0.0",
        port: 4000,
    },

    resolve: {
        alias: {
            src: path.resolve('src/'),
        },
    }
})
