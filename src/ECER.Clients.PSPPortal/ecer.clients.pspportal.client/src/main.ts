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
    VBtn: {
      rounded: "lg",
      elevation: 0,
      size: "large",
    },
  },
  directives,
});

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);

const app = createApp(App);
app.use(pinia);
app.use(router);      
app.use(vuetify);    
app.mount("#app");   