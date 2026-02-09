<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Access denied.</h1>
      <h2 class="mt-20">
        Access to the ECER Post-Secondary Programs portal is limited to program
        representatives for recognized educational institutions.
      </h2>
      <div class="d-flex flex-column ga-3">
        <p>
          If you are a program representative for a recognized institution,
          another program representative with access to the ECER Post-Secondary
          Programs portal can send you an invitation from their dashboard.
        </p>
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
  name: "RequiredInvitation",
  setup() {
    const oidcStore = useOidcStore();
    const { smAndDown } = useDisplay();
    return { smAndDown, oidcStore };
  },
};
</script>
