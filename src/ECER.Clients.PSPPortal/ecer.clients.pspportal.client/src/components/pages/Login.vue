<template>
  <v-sheet :rounded="0" flat color="primary" width="100%">
    <PageContainer :margin-top="false" class="py-13">
      <h1 class="text-white">Welcome to the ECE Post-Secondary Programs Portal</h1>
      <p class="small text-white mt-4">For educational institutions to manage their ECE programs with the B.C. ECE Registry</p>
    </PageContainer>
  </v-sheet>
  <PageContainer :margin-top="false">
    <v-row>
      <v-col cols="12" md="6" class="mb-12">
        <div class="d-flex flex-column ga-4">
          <h2>Login is required</h2>
          <p>In order to login, you will need:</p>
          <ul class="ml-10">
            <li>an emailed invitation from the B.C. ECE Registry</li>
            <li>an active Business BCeID user account (set up by your institution)</li>
          </ul>
          <p>If you have these, please continue to login.</p>
        </div>
        <v-btn class="mt-8" @click="handleLogin()" :size="smAndDown ? 'default' : 'large'" color="primary">Login with Business BCeID</v-btn>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="6" class="mb-12">
        <div class="d-flex flex-column ga-4">
          <p class="font-weight-bold">Collection Notice</p>
          <p>
            Your personal information will be collected for the purposes of communicating with you, authenticating users, and/or determining or auditing the
            eligibility of the ECE programs offered at the educational institution which you represent. If you have any questions about the collection of this
            personal information, please contact:
          </p>
          <p><a href="mailto:ECERegistry.Programs@gov.bc.ca">ECERegistry.Programs@gov.bc.ca</a></p>
          <p>
            This information is being collected by The Ministry of Education and Child Care under Section 26(a) and Section 26(c) of
            <a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/96165_00">the Freedom of Information and Protection of Privacy Act</a>
            .
          </p>
        </div>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";

import LoginCard from "@/components/LoginCard.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useConfigStore } from "@/store/config";
import { useDisplay } from "vuetify";
import Alert from "../Alert.vue";

export default defineComponent({
  name: "Login",
  components: { LoginCard, PageContainer, Alert },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const configStore = useConfigStore();
    const router = useRouter();

    const route = useRoute();
    const { smAndDown } = useDisplay();

    return { userStore, oidcStore, configStore, route, smAndDown, router };
  },
  methods: {
    async handleLogin() {
      // check for redirect_to query param
      const redirectTo = this.route.query.redirect_to as string;

      await this.oidcStore.login("bceidbusiness", redirectTo);
    },
  },
});
</script>
