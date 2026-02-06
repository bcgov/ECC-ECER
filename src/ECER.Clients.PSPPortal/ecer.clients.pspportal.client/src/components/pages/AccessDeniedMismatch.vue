<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Access denied</h1>
      <h2 class="mt-20">
        {{ accessDeniedMessage }}
      </h2>
      <div class="d-flex flex-column ga-3">
        <p>You may have followed an invalid link.</p>
        <p>Contact the ECE Registry if you require help.</p>
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
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { getPortalInvitation } from "@/api/portal-invitation";

export default defineComponent({
  name: "AccessDenied",
  async setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const { smAndDown } = useDisplay();

    const user = await oidcStore.getUser();
    const bceidBusinessName = user?.profile?.bceid_business_name;

    const token = userStore.invitationToken;
    const { data } = await getPortalInvitation(token as string);
    const invitationInstitutionName = data?.portalInvitation?.bceidBusinessName;

    return {
      smAndDown,
      oidcStore,
      bceidBusinessName,
      invitationInstitutionName,
    };
  },
  computed: {
    accessDeniedMessage(): string {
      if (!this.invitationInstitutionName && !this.bceidBusinessName) {
        return "The Business BCeID account you are logged in with is not associated with this institution.";
      }
      if (!this.invitationInstitutionName) {
        return `The Business BCeID account you are logged in with (${this.bceidBusinessName}) is not associated with this institution.`;
      }
      if (!this.bceidBusinessName) {
        return `This portal invitation is for authentication with ${this.invitationInstitutionName}, but the Business BCeID account you are logged in with is not associated with that institution.`;
      }
      return `This portal invitation is for authentication with ${this.invitationInstitutionName}, but the Business BCeID account you logged in with is associated with ${this.bceidBusinessName}.`;
    },
  },
});
</script>
