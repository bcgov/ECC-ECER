<template>
  <v-app-bar height="80" elevation="0" border>
    <v-container>
      <v-row align="center">
        <!-- <router-link to="/"> -->
        <img src="../assets/bc-gov-logo.png" width="155" class="logo" alt="B.C. Government Logo" />
        <!-- </router-link> -->
        <v-toolbar-title>ECE Educational Institution Portal</v-toolbar-title>
        <p class="small text-decoration-underline text-links" @click="oidcStore.logout">
          Log out</p>
      </v-row>
    </v-container>
  </v-app-bar>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { cleanPreferredName } from "@/utils/functions";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "NavigationBar",
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const router = useRouter();
    return { userStore, oidcStore, router, cleanPreferredName };
  },
});
</script>
