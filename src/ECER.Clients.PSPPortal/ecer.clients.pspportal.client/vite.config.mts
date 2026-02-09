import { fileURLToPath, URL } from "node:url";

import plugin from "@vitejs/plugin-vue";
import { env } from "process";
import { defineConfig } from "vite";

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(";")[0]
    : "http://localhost:5140";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    proxy: {
      "^/api/*": {
        target: target,
        secure: false,
      },
      "^/swagger/*": {
        target: target,
        secure: false,
      },
    },
    port: 5130,
  },
});
