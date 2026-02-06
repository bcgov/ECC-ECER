<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Sorry, this link is no longer valid.</h1>
      <h2 class="mt-20">
        You may request a new invitation from another program representative at
        your educational institution who has access to the ECER Post-Secondary
        Programs portal.
      </h2>
      <div class="d-flex flex-column ga-3">
        <!-- prettier-ignore -->
        <p>
          If you need additional support, please contact the Registry at
          <a href="mailto:ECERegistry.Programs@gov.bc.ca">
            ECERegistry.Programs@gov.bc.ca
          </a>.
        </p>
      </div>
      <v-btn
        class="mt-8 align-self-start"
        @click="oidcStore.logout"
        :size="smAndDown ? 'default' : 'large'"
        color="primary"
      >
        Try again
      </v-btn>
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
