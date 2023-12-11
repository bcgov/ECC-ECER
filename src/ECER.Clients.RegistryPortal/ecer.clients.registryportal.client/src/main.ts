import { createPinia } from "pinia";
import piniaPluginPersistedstate from "pinia-plugin-persistedstate";
import { createApp } from "vue";

import App from "./App.vue";
import router from "./router";
import { useConfigStore } from "./store/config";

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);

const app = createApp(App);

app.use(pinia);
app.use(router);

const configStore = useConfigStore();

// Fetch OIDC configuration from the API and initialize the store before mounting the app
configStore.initialize().then(() => {
  app.mount("#app");
});
