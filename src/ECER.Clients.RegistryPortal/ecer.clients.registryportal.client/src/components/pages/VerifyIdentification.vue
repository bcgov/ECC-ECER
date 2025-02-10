<template>
  <PageContainer :margin-top="false">
    <Breadcrumb :items="items" />
    <h1>Verify your identity</h1>
    <div class="d-flex flex-column ga-4">
      <p>Before you can access your full My ECE Registry account, we need to verify your identity.</p>

      <p>To verify your identity:</p>
      <ol class="ml-10">
        <li>Provide photos of 2 government-issued IDs</li>
        <li>We will review your ID</li>
        <li>We will send you a message when your account is ready in 2-3 business days</li>
      </ol>
    </div>
    <div class="mt-12">
      <ECEHeader title="Important information" />
    </div>
    <div class="d-flex flex-column ga-4">
      <p>The ID you provide must:</p>

      <p>To verify your identity:</p>
      <ul class="ml-10">
        <li>Show the exact names as your account: JANET MARIE SMITH</li>
        <li>Be valid (not expired)</li>
        <li>Be in English</li>
      </ul>
      <p>Make sure the photo of your ID:</p>
      <ul class="ml-10">
        <li>Is clear and not blurry</li>
        <li>Shows your full ID and does not cut off any part of the document</li>
      </ul>
    </div>
    <div class="mt-12">
      <ECEHeader title="First, choose a primary ID" />
      <label>
        What type of ID are you providing?
        <v-select class="pt-2" :items="configStore.primaryIdentificationType" variant="outlined" label=""></v-select>
      </label>
      <label>
        Upload file
        <FileUploader :allow-multiple-files="false" :max-number-of-files="1" class="mt-1" @update:files="" />
      </label>
    </div>
    <div class="mt-12">
      <ECEHeader title="Then, choose a secondary ID" />
      <label>
        What type of ID are you providing?
        <v-select class="pt-2" :items="configStore.secondaryIdentificationType" variant="outlined" label=""></v-select>
      </label>
      <label>
        Upload file
        <FileUploader :allow-multiple-files="false" :max-number-of-files="1" class="mt-1" @update:files="" />
      </label>
    </div>
    <v-btn @click="handleSubmit()" class="my-8" :size="smAndDown ? 'default' : 'large'" color="primary" append-icon="mdi-arrow-right">
      Send for verification
    </v-btn>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";

import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import IconCard from "@/components/IconCard.vue";
import FileUploader from "@/components/FileUploader.vue";
import Callout from "@/components/Callout.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useDisplay } from "vuetify";
import ECEHeader from "../ECEHeader.vue";
import { useConfigStore } from "@/store/config";

export default defineComponent({
  name: "VerifyIdentification",
  components: { PageContainer, Breadcrumb, IconCard, ECEHeader, Callout, FileUploader },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const configStore = useConfigStore();
    const route = useRoute();
    const { smAndDown } = useDisplay();

    return { userStore, oidcStore, configStore, route, smAndDown };
  },
  data() {
    const items = [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Verify",
        disabled: true,
        href: "/verify-identification",
      },
    ];

    return { items };
  },
  methods: {
    async handleSubmit() {},
  },
});
</script>
