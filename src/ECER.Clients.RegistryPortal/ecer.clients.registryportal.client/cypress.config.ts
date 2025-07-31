import { defineConfig } from "cypress";
import { env } from "process";

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
    specPattern: [
      "cypress/e2e/**/auth.cy.ts", // auth spec runs first
      "cypress/e2e/**/*.cy.ts", // other specs next
      "cypress/e2e/**/z_logout.cy.ts", // always run this last
    ],
  },
  defaultCommandTimeout: 20000,
  scrollBehavior: "center", // or 'nearest'
  retries: {
    runMode: 1, // retry once when we do `cypress run`
    openMode: 0, // no retries when we’re developing with `cypress open`
  },
  env: { BCSC_USERNAME: env.BCSC_USERNAME, BCSC_PASSWORD: env.BCSC_PASSWORD },
});
