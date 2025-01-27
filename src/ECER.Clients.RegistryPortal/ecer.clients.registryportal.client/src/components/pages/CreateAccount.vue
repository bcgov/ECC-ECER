<template>
  <PageContainer :margin-top="false">
    <Breadcrumb :items="items" />
    <h1>Create a My ECE Registry account</h1>
    <v-row class="mt-8">
      <v-col cols="12" md="4">
        <IconCard title="Step 1" icon="mdi-lock-open" text="Log in securely with BC Services Card or another login option." />
      </v-col>
      <v-col cols="12" md="4">
        <IconCard title="Step 2" icon="mdi-account-edit" text="Add your contact information to complete your My ECE Registry account." />
      </v-col>
      <v-col cols="12" md="4">
        <IconCard title="Step 3" icon="mdi-check-decagram" text="Submit a new application or access your existing certifications." />
      </v-col>
    </v-row>
    <div class="mt-12">
      <ECEHeader title="Get started" />
    </div>
    <p class="my-6">You can create your My ECE Registry account using:</p>
    <v-expansion-panels v-model="panel" multiple elevation="0">
      <v-expansion-panel collapse-icon="" expand-icon="" readonly :style="{ border: '1px solid #606367' }">
        <v-expansion-panel-title><h2>BC Services Card</h2></v-expansion-panel-title>
        <v-expansion-panel-text>
          <p>It is easy, secure, and the fastest way to set up your account.</p>
          <v-btn @click="handleLogin('bcsc')" class="my-8" :size="smAndDown ? 'default' : 'large'" color="primary" append-icon="mdi-arrow-right">
            Log in with BC Services Card
          </v-btn>
          <div class="d-flex flex-column ga-4">
            <p>Logging in with BC Services Card</p>
            <ul class="ml-10">
              <li>Is a secure login service provided by the Government of British Columbia</li>
              <li>Allows you to log in to My ECE Registry and many other government services</li>
              <li>Is available to individuals with ID issued in Canada</li>
            </ul>
            <p>
              <a href="https://id.gov.bc.ca/account/setup-instruction" target="_blank">How to set up a BC Services Card account</a>
            </p>
          </div>
        </v-expansion-panel-text>
      </v-expansion-panel>
      <v-expansion-panel :style="{ border: '1px solid #606367' }" class="mt-10">
        <v-expansion-panel-title><h2>Other login options</h2></v-expansion-panel-title>
        <v-expansion-panel-text>
          <div class="d-flex flex-column ga-4">
            <p>
              You can use
              <b>Basic BCeID</b>
              to log in when you cannot use BC Services Card (for example, if you only have ID that was issued outside of Canada).
            </p>
            <p>This login option:</p>
            <Callout type="warning">
              <div class="d-flex flex-column ga-3 mt-3">
                <v-icon>mdi-card-account-details</v-icon>
                <h3>Requires government-issued ID</h3>
                <p>
                  You will be asked to provide 2 pieces of
                  <a href="https://www2.gov.bc.ca/gov/content/governments/government-id/bcservicescardapp/id" target="_blank">accepted government-issued ID</a>
                  after you create a My ECE Registry account with your Basic BCeID.
                </p>
              </div>
            </Callout>
            <Callout type="warning">
              <div class="d-flex flex-column ga-3 mt-3">
                <v-icon>mdi-clock-outline</v-icon>
                <h3>Takes longer to set up</h3>
                <p>Processing time is approximately 3-5 business days as we review the ID to verify your identity.</p>
              </div>
            </Callout>
          </div>
          <v-btn @click="handleLogin('bceid')" class="my-8" :size="smAndDown ? 'default' : 'large'" color="primary" append-icon="mdi-arrow-right">
            Log in with Basic BCeID
          </v-btn>
          <p>
            <a href="https://www.bceid.ca/register/basic/account_details.aspx?type=regular&eServiceType=basic" target="_blank">Register for a Basic BCeID</a>
          </p>
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";

import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import IconCard from "@/components/IconCard.vue";
import Callout from "@/components/Callout.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useDisplay } from "vuetify";
import ECEHeader from "../ECEHeader.vue";

export default defineComponent({
  name: "CreateAccount",
  components: { PageContainer, Breadcrumb, IconCard, ECEHeader, Callout },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const route = useRoute();
    const { smAndDown } = useDisplay();

    return { userStore, oidcStore, route, smAndDown };
  },
  data() {
    const panel = [0];
    const items = [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Create Account",
        disabled: true,
        href: "/create-account",
      },
    ];

    return { items, panel };
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
