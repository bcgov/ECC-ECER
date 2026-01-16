<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Sorry, this link is no longer valid</h1>
      <h2 class="mt-20">This link is no longer valid. Please request a new invitation from your institution administrator.</h2>
      <v-btn class="mt-8 align-self-start" @click="oidcStore.logout" :size="smAndDown ? 'default' : 'large'" color="primary">Try again</v-btn>
    </div>
  </v-container>
</template>

<script lang="ts">
import { useOidcStore } from "@/store/oidc";
import { useDisplay } from "vuetify";
export default {
  name: "InvalidInvitation",
  setup() {
    const oidcStore = useOidcStore();
    const { smAndDown } = useDisplay();
    return { smAndDown, oidcStore };
  },
};
</script>
