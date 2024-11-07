<template>
  <v-sheet :rounded="0" flat color="primary" width="100%">
    <PageContainer :margin-top="false" class="py-13">
      <h1 class="text-white">Welcome to the Early Childhood Educator (ECE) Registry</h1>
      <p class="small text-white mt-4">Apply or manage your certification to work in a child care facility in British Columbia.</p>
    </PageContainer>
  </v-sheet>
  <PageContainer :margin-top="false">
    <div class="d-flex flex-column ga-3">
      <h2>Log in with BC Services Card</h2>
      <p>A BC Services Card account is:</p>
      <ul class="ml-10">
        <li>A secure login service provided by the government of British Columbia</li>
        <li>An account you can use to log in to My ECE Registry and many other government services</li>
        <li>Available to people with ID issued in Canada</li>
      </ul>
    </div>
    <v-btn @click="handleLogin('bcsc')" class="my-8" :size="smAndDown ? 'default' : 'large'" color="primary" append-icon="mdi-arrow-right">
      Log in with BC Services Card
    </v-btn>
    <p>
      <a href="https://id.gov.bc.ca/account/setup-instruction" target="_blank">How to set up a BC Services Card account</a>
    </p>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";

import LoginCard from "@/components/LoginCard.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useDisplay } from "vuetify";

export default defineComponent({
  name: "Login",
  components: { LoginCard, PageContainer },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const route = useRoute();
    const { smAndDown } = useDisplay();

    return { userStore, oidcStore, route, smAndDown };
  },
  methods: {
    async handleLogin(provider: string) {
      // check for redirect_to query param
      const redirectTo = this.route.query.redirect_to as string;

      await this.oidcStore.login(provider == "bceid" ? "bceidbasic" : "bcsc", redirectTo);
    },
  },
});
</script>
