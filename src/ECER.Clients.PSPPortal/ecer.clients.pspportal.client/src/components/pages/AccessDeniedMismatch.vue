<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Access denied.</h1>
      <h2 class="mt-20">
        {{ accessDeniedMessage }}
      </h2>
      <p>{{ accessDeniedSubMessage }}</p>
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
    const postSecondaryInstitutionName =
      data?.portalInvitation?.postSecondaryInstitutionName;
    const dynamicsBceidBusinessName = data?.portalInvitation?.bceidBusinessName;

    return {
      smAndDown,
      oidcStore,
      bceidBusinessName,
      postSecondaryInstitutionName,
      dynamicsBceidBusinessName,
    };
  },
  computed: {
    accessDeniedMessage(): string {
      if (!this.postSecondaryInstitutionName && !this.bceidBusinessName) {
        return "The Business BCeID account you are logged in with is not associated with this institution.";
      }
      if (!this.postSecondaryInstitutionName) {
        return `The Business BCeID account you are logged in with (${this.bceidBusinessName}) is not associated with this institution.`;
      }
      if (!this.bceidBusinessName) {
        return `This portal invitation is for authentication with ${this.postSecondaryInstitutionName}, but the Business BCeID account you are logged in with is not associated with that institution.`;
      }
      return `This portal invitation is for authentication with ${this.postSecondaryInstitutionName}, but the Business BCeID account you logged in with is associated with ${this.bceidBusinessName}.`;
    },
    accessDeniedSubMessage(): string {
      return this.dynamicsBceidBusinessName
        ? `The Business BCeID account listed in our records is ${this.dynamicsBceidBusinessName}. Please contact the ECE Registry if this is incorrect.`
        : "You may have followed an invalid link. Contact the ECE Registry if you require help.";
    },
  },
});
</script>
