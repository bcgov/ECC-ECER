// .storybook/preview.ts

import type { Preview } from "@storybook/vue3";
import { setup } from "@storybook/vue3"; // ✅ Use the setup function from Storybook
import { createVuetify } from "vuetify";
import "vuetify/styles"; // Import Vuetify base styles
import "@mdi/font/css/materialdesignicons.css"; // Optional: MDI icons

import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import { aliases, mdi } from "vuetify/iconsets/mdi";
import { VDateInput } from "vuetify/labs/VDateInput";

import ecerTheme from "../src/styles/ecer-theme"; // Your custom theme file
import { createPinia } from "pinia";

// ✅ Global setup: Register Vuetify and Pinia
setup((app) => {
  const vuetify = createVuetify({
    theme: {
      defaultTheme: "ecerTheme",
      themes: { ecerTheme },
    },
    icons: {
      defaultSet: "mdi",
      aliases,
      sets: { mdi },
    },
    display: {
      mobileBreakpoint: "sm",
    },
    components: {
      VDateInput,
      ...components,
    },
    directives,
    defaults: {
      VTextField: {
        variant: "outlined",
        color: "primary",
        hideDetails: "auto",
      },
      VDateInput: {
        variant: "outlined",
        color: "primary",
        hideDetails: "auto",
      },
      VCheckbox: {
        color: "primary",
      },
      VBtn: {
        rounded: "lg",
        elevation: 0,
        size: "large",
      },
    },
  });

  app.use(vuetify);
  app.use(createPinia());
});

// ✅ Storybook parameters (optional: customize as needed)
const preview: Preview = {
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/,
      },
    },
  },
};

export default preview;
