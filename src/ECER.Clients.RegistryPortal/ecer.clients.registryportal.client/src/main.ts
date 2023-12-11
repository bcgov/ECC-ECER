// Vuetify
import "vuetify/styles";

import { createPinia } from "pinia";
import piniaPluginPersistedstate from "pinia-plugin-persistedstate";
import { createApp } from "vue";
import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";

import App from "./App.vue";
import router from "./router";
import { useConfigStore } from "./store/config";
import ecerTheme from "./styles/ecer-theme";

const vuetify = createVuetify({
  theme: {
    defaultTheme: "ecerTheme",
    themes: {
      ecerTheme,
    },
  },
  components,
  directives,
});

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);

const app = createApp(App);

app.use(pinia);
app.use(router);
app.use(vuetify);

const configStore = useConfigStore();

// Fetch OIDC configuration from the API and initialize the store before mounting the app
configStore.initialize().then(() => {
  app.mount("#app");
});
