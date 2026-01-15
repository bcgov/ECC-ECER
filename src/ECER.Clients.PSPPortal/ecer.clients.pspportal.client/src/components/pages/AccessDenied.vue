<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Access denied</h1>
      <h2 class="mt-20">Your business BCeid account is not associated with this institution.</h2>
      <div class="d-flex flex-column ga-3">
        <p>You may have followed an invalid link.</p>
        <p>Contact the ECE Registry if you require help.</p>
      </div>
      <v-btn class="mt-8 align-self-start" @click="oidcStore.logout" :size="smAndDown ? 'default' : 'large'" color="primary">Try again</v-btn>
    </div>
  </v-container>
</template>

<script lang="ts">
import { useDisplay } from "vuetify";
import { useOidcStore } from "@/store/oidc";
export default {
  name: "AccessDenied",
  setup() {
    const oidcStore = useOidcStore();
    const { smAndDown } = useDisplay();
    return { smAndDown, oidcStore };
  },
};
</script>
