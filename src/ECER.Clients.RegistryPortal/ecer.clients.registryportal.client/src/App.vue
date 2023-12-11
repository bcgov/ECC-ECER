<template>
  <main>
    <p>
      <router-link to="/">Go to Home</router-link>
    </p>

    <div v-if="userStore.isAuthenticated">
      <button type="button" @click="logout">Logout</button>
    </div>
    <div v-else>
      <router-link to="/login">Login</router-link>
    </div>
    <hr />

    <router-view></router-view>
  </main>
</template>

<script lang="ts">
import { defineComponent, watch } from "vue";
import { useRouter } from "vue-router";

import { useUserStore } from "@/store/user";

import { useOidcStore } from "./store/oidc";

export default defineComponent({
  name: "App",
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
    logout() {
      if (this.userStore.isAuthenticated && this.userStore.authority) {
        if (this.userStore.authority == "bceid") {
          this.oidcStore.logout(this.userStore.authority);
        }
        if (this.userStore.authority == "bcsc") {
          // BCSC does not support a session logout callback endpoint so just remove session data from client
          this.oidcStore.removeUser(this.userStore.authority);
          this.userStore.$reset();
        }
      }
    },
  },
});
</script>
