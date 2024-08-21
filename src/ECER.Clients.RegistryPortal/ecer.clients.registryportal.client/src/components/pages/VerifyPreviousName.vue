<template>
  <PageContainer>
    <Breadcrumb :items="items" />
    <v-row class="mt-3">
      <v-col cols="12">
        <h1>Provide proof of previous name</h1>
        <div>
          <p class="mt-5">When your current legal name is different than your name on a supporting document, you need to provide proof of name change.</p>
          <p class="font-weight-bold mb-2 mt-5">Legal name on your account</p>
          <p>{{ userStore.legalName }}</p>
          <p class="font-weight-bold mb-2 mt-5">Previous name to add your account</p>
          <p>{{ previousName }}</p>
        </div>
      </v-col>
    </v-row>
    <v-row class="mt-5">
      <v-col>
        <ECEHeader title="Accepted ID" />
        <p class="mt-3">The following ID is accepted for proof of previous name.</p>
        <ul class="ml-10 mt-2">
          <li>Government-issued marriage certificate</li>
          <li>Divorce certificate or papers</li>
          <li>Government-issued change of name document</li>
        </ul>
        <p class="mt-3">The ID must:</p>
        <ul class="ml-10 mt-2">
          <li>Show both names above</li>
          <li>Be valid(not expired)</li>
          <li>Be Government-issued</li>
          <li>Be in English</li>
        </ul>
      </v-col>
    </v-row>
    <v-row class="mt-5">
      <v-col>
        <ECEHeader title="Upload a photo of the ID" />
        <FileUploader class="mt-1" @update:files="handleFileUpdate" />
      </v-col>
    </v-row>
    <v-row class="mt-6">
      <v-col class="d-flex flex-row ga-3 flex-wrap">
        <v-btn size="large" color="primary" @click="handleSendProofPreviousName">Send</v-btn>
        <v-btn size="large" variant="outlined" color="primary" @click="router.push('/profile')">Cancel</v-btn>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { useRoute, useRouter } from "vue-router";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import FileUploader from "@/components/FileUploader.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";

export default {
  name: "VerifyPreviousName",
  components: { PageContainer, Breadcrumb, ECEHeader, FileUploader },
  setup() {
    const loadingStore = useLoadingStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const router = useRouter();
    const route = useRoute();
    function fullName(name: Components.Schemas.PreviousName) {
      return name.middleName ? `${name.firstName} ${name.middleName} ${name.lastName}` : `${name.firstName} ${name.lastName}`;
    }
    const previousNameId = route.params.previousNameId.toString();
    const foundName = userStore.unverifiedPreviousNames.find((item) => item.id === previousNameId);
    const previousName = fullName(foundName!);

    return { loadingStore, alertStore, userStore, router, previousName };
  },
  data: () => ({
    items: [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Verify",
        disabled: true,
        href: "/profile/verify-previous-name",
      },
    ],
    areAttachedFilesValid: true,
    isFileUploadInProgress: false,
    isAtleastOneFileAdded: false,
  }),
  computed: {},
  methods: {
    async handleSendProofPreviousName() {
      if (!this.isAtleastOneFileAdded) {
        this.alertStore.setFailureAlert("You must add atleast one file.");
      } else if (this.isFileUploadInProgress) {
        this.alertStore.setFailureAlert("Uploading files. You need to wait until files are uploaded to send this message.");
      } else if (!this.areAttachedFilesValid) {
        this.alertStore.setFailureAlert("You must upload valid files.");
      }
    },
    handleFileUpdate(filesArray: any[]) {
      this.areAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      this.isAtleastOneFileAdded = false;
      if (filesArray && filesArray.length > 0) {
        this.isAtleastOneFileAdded = true;
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];
          if (!(file.fileErrors.length > 0) && file.progress < 101) {
            this.isFileUploadInProgress = true;
            break;
          }
        }
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];
          if (file.fileErrors && file.fileErrors.length > 0) {
            this.areAttachedFilesValid = false;
            break;
          }
        }
      }
    },
  },
};
</script>
