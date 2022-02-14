import { defineConfig } from "vite";
import vuePlugin from "@vitejs/plugin-vue";
import viteEslintPlugin from "vite-plugin-eslint";
import { readFileSync } from "fs";
import { certFilePath, keyFilePath } from "./https.dev.config.js";

const defaultDevApiEndpointConf = {
  target: "https://localhost:5001/",
  secure: false,
};

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vuePlugin(), viteEslintPlugin],
  define: {
    __VUE_OPTIONS_API__: false,
    __VUE_PROD_DEVTOOLS__: false,
  },
  server: {
    host: "localhost",
    port: 5002,
    strictPort: true,
    https: {
      key: readFileSync(keyFilePath),
      cert: readFileSync(certFilePath),
    },
    proxy: {
      "/api": defaultDevApiEndpointConf,
      "/swagger": defaultDevApiEndpointConf,
      "/weatherforecast": defaultDevApiEndpointConf,
    },
    cors: {
      origin: "*",
    },
    hmr: true,
  },
});
