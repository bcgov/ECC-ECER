<template>
  <main>
    <Suspense>
      <v-app>
        <NavigationBar />
        <v-main>
          <router-view></router-view>
        </v-main>
        <EceFooter />
      </v-app>
    </Suspense>
  </main>
</template>

<script lang="ts">
import { defineComponent, watch } from "vue";
import { useRouter } from "vue-router";

import { useUserStore } from "@/store/user";

import EceFooter from "./components/Footer.vue";
import NavigationBar from "./components/NavigationBar.vue";
import { useOidcStore } from "./store/oidc";

export default defineComponent({
  name: "App",
  components: {
    NavigationBar,
    EceFooter,
  },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const router = useRouter();

    // Watch for changes to isAuthenticated flag
    watch(
      () => userStore.isAuthenticated,
      (newValue: boolean) => {
        if (!newValue) {
          // If not authenticated, navigate to the login page
          router.push("/login");
        } else {
          // If authenticated, navigate to the dashboard page
          // TODO: Once ECER-494 is complete, this should be changed to direct first time users to "/new-user" page and returning users to the dashboard "/"
          router.push("/new-user");
        }
      },
    );

    return { userStore, oidcStore };
  },
});
</script>

<style lang="scss">
@import "@/styles/typography.scss";
@import "@/styles/button.scss";
</style>
