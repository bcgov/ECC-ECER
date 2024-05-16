<template>
  <v-app-bar
    height="80"
    elevation="0"
    color="primary"
    :style="
      $vuetify.display.mobile
        ? {}
        : {
            'border-bottom': '2px solid #FFC72C',
          }
    "
  >
    <router-link to="/">
      <img src="../assets/bc-gov-logo.svg" width="155" class="logo ms-6" alt="B.C. Government Logo" />
    </router-link>
    <v-toolbar-title>My ECE Registry</v-toolbar-title>
    <v-btn v-if="userStore.hasUserInfo" color="white" @click="oidcStore.logout">Logout</v-btn>
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
});
</script>
