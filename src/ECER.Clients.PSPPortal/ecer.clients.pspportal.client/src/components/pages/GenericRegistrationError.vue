<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Sorry, an error occurred</h1>
      <h2 class="mt-20">
        An error occurred while registering your user. Please try again later.
      </h2>
      <v-btn
        class="mt-8 align-self-start"
        @click="oidcStore.logout"
        :size="smAndDown ? 'default' : 'large'"
        color="primary"
      >
        Logout
      </v-btn>
    </div>
  </v-container>
</template>

<script lang="ts">
import { useOidcStore } from "@/store/oidc";
import { useDisplay } from "vuetify";
export default {
  name: "GenericRegistrationError",
  setup() {
    const oidcStore = useOidcStore();
    const { smAndDown } = useDisplay();
    return { smAndDown, oidcStore };
  },
};
</script>
