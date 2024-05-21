<template>
  <PageContainer>
    <div>
      <h1>Welcome to my ECE Registry</h1>
      <p class="mt-6">
        To work as an Early Childhood Educator (ECE) or ECE Assistant in a licensed child care facility in B.C. you must be certified by the ECE Registry. Log
        in to manage your certifications, review your eligibility, and access your messages.
      </p>

      <div class="mt-16">
        <div class="d-flex flex-row flex-wrap justify-center ga-16">
          <LoginCard is-preferred>
            <div class="d-flex flex-column ga-8 align-center justify-center fill-height">
              <img src="@/assets/BCSC.svg" class="logo ms-6" width="62" alt="BCSC Logo" />
              <h2>Login with BC Services Card</h2>
              <div class="d-flex flex-column ga-2" :style="{ height: '140px' }">
                <div class="d-flex flex-row align-center">
                  <v-icon size="small" color="grey-dark" icon="mdi-check" class="mr-2"></v-icon>
                  <p class="small">Safe and secure method for Canadian residents</p>
                </div>
                <div class="d-flex flex-row align-center">
                  <v-icon size="small" color="grey-dark" icon="mdi-check" class="mr-2"></v-icon>
                  <p class="small">Easy to set up with Canadian government-issued ID</p>
                </div>
                <div class="d-flex flex-row align-center">
                  <v-icon size="small" color="grey-dark" icon="mdi-check" class="mr-2"></v-icon>
                  <p class="small">Your identity is verified upon login</p>
                </div>
                <div class="d-flex flex-row align-center">
                  <v-icon size="small" color="grey-dark" icon="mdi-check" class="mr-2"></v-icon>
                  <p class="small">Save time later in the application process</p>
                </div>
              </div>
              <v-btn rounded="lg" class="font-weight-bold" color="primary" @click="handleLogin('bcsc')">LOG IN WITH BC SERVICES CARD</v-btn>
              <div class="text-center">
                <p class="small">Don't have an account?</p>
                <a href="https://id.gov.bc.ca/account/setup-instruction">
                  <p class="small">Get set up with BC Services Card</p>
                </a>
              </div>
            </div>
          </LoginCard>
          <LoginCard>
            <div class="d-flex flex-column ga-8 align-center justify-center fill-height">
              <img src="@/assets/BCeID.png" alt="BCeID Logo" />
              <h2>Login with Basic BCeID</h2>
              <div class="mx-10" :style="{ height: '140px' }">
                <p class="small">
                  If you do not have ID issued in Canada you can use a BCeID for secure login. Identity verification will be required later in the application
                  process.
                </p>
              </div>
              <v-btn rounded="lg" class="font-weight-bold" color="primary" @click="handleLogin('bceid')">LOG IN WITH BASIC BCeID</v-btn>
              <div class="text-center">
                <p class="small">Don't have an account?</p>
                <a href="https://www.bceid.ca/register/">
                  <p class="small">Register for Basic BCeID</p>
                </a>
              </div>
            </div>
          </LoginCard>
        </div>
      </div>
    </div>
  </PageContainer>
  <!-- <div class="bg-black text-center border-t-lg border-b-lg border-warning border-opacity-100">
    <p class="small text-white py-4 px-10">
      The B.C. Public Service acknowledges the territories of First Nations around B.C. and is grateful to carry out our work on these lands. We acknowledge the
      rights, interests, priorities, and concerns of all Indigenous Peoples - First Nations, Métis, and Inuit - respecting and acknowledging their distinct
      cultures, histories, rights, laws, and governments.
    </p>
  </div> -->
</template>

<script lang="ts">
import { defineComponent } from "vue";

import LoginCard from "@/components/LoginCard.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";

export default defineComponent({
  name: "Login",
  components: { LoginCard, PageContainer },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    return { userStore, oidcStore };
  },
  methods: {
    async handleLogin(provider: string) {
      await this.oidcStore.login(provider == "bceid" ? "bceidbasic" : "bcsc");
    },
  },
});
</script>
