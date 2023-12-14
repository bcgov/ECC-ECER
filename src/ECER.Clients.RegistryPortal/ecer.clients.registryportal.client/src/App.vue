<template>
  <main>
    <v-app>
      <NavigationBar />
      <v-main>
        <v-container class="my-6">
          <router-view></router-view>
        </v-container>
      </v-main>
      <EceFooter />
    </v-app>
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
          // If authenticated, navigate to the home page
          router.push("/"); // Adjust the route name based on your routes
        }
      },
    );

    return { userStore, oidcStore };
  },
  methods: {
    async logout() {
      if (this.userStore.isAuthenticated && this.userStore.authority) {
        if (this.userStore.authority == "bceid") {
          await this.oidcStore.logout(this.userStore.authority);
        }
        if (this.userStore.authority == "bcsc") {
          // BCSC does not support a session logout callback endpoint so just remove session data from client
          await this.oidcStore.removeUser(this.userStore.authority);
          this.userStore.$reset();
        }
      }
    },
  },
});
</script>

<style lang="scss">
@import "@/styles/typography.scss";
</style>
