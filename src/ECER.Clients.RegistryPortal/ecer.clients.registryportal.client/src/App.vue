<template>
  <main>
    <Suspense>
      <v-app>
        <NavigationBar />
        <v-main>
          <Snackbar />
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

import { getUserInfo } from "./api/user";
import EceFooter from "./components/Footer.vue";
import NavigationBar from "./components/NavigationBar.vue";
import Snackbar from "./components/Snackbar.vue";
import { useOidcStore } from "./store/oidc";
import type { Components } from "./types/openapi";

export default defineComponent({
  name: "App",
  components: {
    NavigationBar,
    EceFooter,
    Snackbar,
  },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();

    const router = useRouter();

    // Watch for changes to isAuthenticated flag
    watch(
      () => userStore.isAuthenticated,
      async (newValue: boolean) => {
        if (!newValue) {
          // If not authenticated, navigate to the login page
          router.push("/login");
        } else {
          // If authenticated, check if new user
          const userInfo: Components.Schemas.UserInfo | null = await getUserInfo();
          if (userInfo) {
            userStore.setUserInfo(userInfo);
            router.push("/");
          } else {
            router.push("/new-user");
          }
        }
      },
    );

    return { userStore, oidcStore };
  },
});
</script>

<style lang="scss">
@import "@/styles/main.scss";
</style>
