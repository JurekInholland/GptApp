import { fileURLToPath, URL } from "node:url";

import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
// import AutoImport from "unplugin-auto-import/vite";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    // AutoImport({
    //   include: [
    //     /\.[tj]sx?$/, // .ts, .tsx, .js, .jsx
    //     /\.vue$/,
    //     /\.vue\?vue/, // .vue
    //     /\.md$/, // .md
    //   ],
    //   imports: [
    //     // presets
    //     "vue",
    //     "vue-router",
    //   ],
    // }),
    vue(),
  ],
  publicDir: "public",
  server: {
    proxy: {
      "/api": {
        target: "http://localhost:5154",
        changeOrigin: false,
        secure: false,
        ws: true,
      },
    },
  },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
});
