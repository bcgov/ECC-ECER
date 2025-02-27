import { defineConfig } from "cypress";
import { env } from "process";

// Define default devices if none are provided via environment variables.
const defaultDevices = [
  { device: "Desktop", width: 1280, height: 720 },
  { device: "Mobile", width: 375, height: 667 }, // Example: iPhone 6 dimensions
  { device: "Tablet", width: 768, height: 1024 }, // Example tablet dimensions
];

export default defineConfig({
  e2e: {
    baseUrl: "http://localhost:5121",
    setupNodeEvents(on, config) {
      // Implement any node event listeners here if needed.
      return config;
    },
  },
  env: {
    BCSC_USERNAME: env.BCSC_USERNAME || "bcsc_username",
    BCSC_PASSWORD: env.BCSC_PASSWORD || "bcsc_password",
    DEVICES: env.DEVICES || JSON.stringify(defaultDevices),
  },
});
