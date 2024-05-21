<template>
  <main>
    <Suspense>
      <v-app>
        <NavigationBar />
        <v-main>
          <InactiveSessionTimeout />
          <Snackbar />
          <router-view></router-view>
        </v-main>
        <!-- <EceFooter /> -->
      </v-app>
    </Suspense>
  </main>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { watch } from "vue";
import { useRouter } from "vue-router";

import { useUserStore } from "@/store/user";

import { getProfile } from "./api/profile";
import { getUserInfo } from "./api/user";
import EceFooter from "./components/Footer.vue";
import InactiveSessionTimeout from "./components/InactiveSessionTimeout.vue";
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
    InactiveSessionTimeout,
  },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const router = useRouter();

    oidcStore.userManager.events.addUserLoaded(async () => {
      const userInfo: Components.Schemas.UserInfo | null = await getUserInfo();
      if (userInfo) {
        // Maybe user has a profile already
        const profileInfo: Components.Schemas.UserProfile | null = await getProfile();

        // Set user info and profile info in the store
        userStore.setUserInfo(userInfo);
        userStore.setUserProfile(profileInfo);
      } else {
        // Push user to the new user page if they don't have user info
        router.push("/new-user");
      }
    });

    watch(
      () => userStore.hasUserInfo,
      async (user) => {
        if (user) {
          // Push user to the home page after login
          router.push("/");
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
