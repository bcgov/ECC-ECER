<template>
  <v-sheet :rounded="0" flat color="primary" width="100%">
    <PageContainer :margin-top="false" class="py-13">
      <h1 class="text-white">Welcome to the Early Childhood Educator (ECE) Registry</h1>
      <p class="small text-white mt-4">Manage your certification to work in a licensed child care facility in B.C.</p>
    </PageContainer>
  </v-sheet>
  <PageContainer :margin-top="false">
    <div v-for="(systemMessage, index) in configStore.systemMessages" :index="index">
      <div v-if="systemMessage.portalTags && systemMessage.portalTags.includes('LOGIN')" class="d-flex flex-column ga-3 mb-10">
        <Banner type="info" :title="systemMessage.message ? systemMessage.message : ''" />
      </div>
    </div>
    <v-row>
      <v-col cols="12" md="6" class="mb-12">
        <div class="d-flex flex-column ga-4">
          <h2>First time using My ECE Registry online?</h2>
          <p>You need to create an account to submit a new application or access your existing certifications.</p>
        </div>
        <v-btn class="mt-8" @click="handleCreateAccount()" :size="smAndDown ? 'default' : 'large'" color="primary">Create account</v-btn>
      </v-col>
      <v-col cols="12" md="6">
        <h2>Already have an account?</h2>
        <div class="d-inline-flex flex-column ga-8 mt-6">
          <v-btn @click="handleLogin('bcsc')" :size="smAndDown ? 'default' : 'large'" color="primary" variant="outlined">Log in with BC Services Card</v-btn>
          <v-btn @click="handleLogin('bceid')" :size="smAndDown ? 'default' : 'large'" color="primary" variant="outlined">Log in with Basic BCeID</v-btn>
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
import Banner from "../Banner.vue";

export default defineComponent({
  name: "Login",
  components: { LoginCard, PageContainer, Banner },
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
    async handleLogin(provider: string) {
      // check for redirect_to query param
      const redirectTo = this.route.query.redirect_to as string;

      await this.oidcStore.login(provider == "bceid" ? "bceidbasic" : "bcsc", redirectTo);
    },
    handleCreateAccount() {
      this.router.push({ name: "createAccount" });
    },
  },
});
</script>
