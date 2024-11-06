// Vuetify
import "vuetify/styles";
import "@mdi/font/css/materialdesignicons.css";

import { createPinia } from "pinia";
import piniaPluginPersistedstate from "pinia-plugin-persistedstate";
import { createApp } from "vue";
import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import { aliases, mdi } from "vuetify/iconsets/mdi";

import App from "./App.vue";
import router from "./router";
import { useConfigStore } from "./store/config";
import ecerTheme from "./styles/ecer-theme";
import { VDateInput } from "vuetify/labs/VDateInput";

const vuetify = createVuetify({
  theme: {
    defaultTheme: "ecerTheme",
    themes: {
      ecerTheme,
    },
  },
  icons: {
    defaultSet: "mdi",
    aliases,
    sets: {
      mdi,
    },
  },
  display: {
    mobileBreakpoint: "sm",
  },
  components: {
    VDateInput,
    ...components,
  },
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
  },
  directives,
  defaults: {
    VBtn: {
      rounded: "lg",
      elevation: 0,
      size: "large",
    },
  },
});

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);

const app = createApp(App);

app.use(pinia);

const configStore = useConfigStore();

// Fetch OIDC configuration from the API and initialize the store before mounting the app
configStore.initialize().then(() => {
  // Ensure the OIDC configuration is loaded before, instantiating the router (which will make a getUser() call) and mounting the app
  app.use(router);
  app.use(vuetify);
  app.mount("#app");
});
