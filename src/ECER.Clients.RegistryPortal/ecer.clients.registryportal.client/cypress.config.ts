import { defineConfig } from "cypress";
import { env } from "process";

// Define default devices if none are provided via environment variables.
// can add multiple devices for testing
const defaultDevices = [{ device: "Desktop", width: 1280, height: 720 }];

export default defineConfig({
  e2e: {
    baseUrl: env.BASE_URL || "http://localhost:5121",
    setupNodeEvents(on, config) {
      // If a BASE_URL is provided via the environment, use it.
      if (config.env.BASE_URL) {
        config.baseUrl = config.env.BASE_URL;
      }
      return config;
    },
  },
  env: {
    BCSC_USERNAME: env.BCSC_USERNAME,
    BCSC_PASSWORD: env.BCSC_PASSWORD,
    DEVICES: env.DEVICES || JSON.stringify(defaultDevices),
  },
});
