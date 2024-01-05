<template>
  <v-app-bar
    height="80"
    elevation="0"
    color="primary"
    :style="{
      'border-bottom': '2px solid #FCBA19',
    }"
  >
    <router-link to="/">
      <img src="../assets/bc-gov-logo.svg" width="155" class="logo ms-6" alt="B.C. Government Logo" />
    </router-link>
    <v-toolbar-title>My ECE Registry</v-toolbar-title>
    <v-btn v-if="userStore.isAuthenticated" class="align-self-center" color="white" @click="logout">Logout</v-btn>
  </v-app-bar>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "NavigationBar",
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();

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
