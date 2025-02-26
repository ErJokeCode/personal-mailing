import tailwindcss from "@tailwindcss/vite";
import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import path from "path";

export default defineConfig({
  plugins: [svelte(), tailwindcss()],

  resolve: {
    alias: {
      src: path.resolve('src/'),
    },
  }
})
