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
    <v-form ref="verifyIdentificationForm">
      <div class="mt-12">
        <ECEHeader title="Important information" />
      </div>
      <div class="d-flex flex-column ga-4">
        <p>The ID you provide must:</p>

        <p>To verify your identity:</p>

        <ul class="ml-10">
          <li>
            Show the exact names as your account: {{ userStore.userProfile?.firstName }} {{ userStore.userProfile?.middleName }}
            {{ userStore.userProfile?.lastName }}
            <v-tooltip text="If this name does not match your ID, you will need to contact us to have it updated." location="top">
              <template #activator="{ props }">
                <v-icon v-bind="props" icon="mdi-help-circle" variant="plain" />
              </template>
            </v-tooltip>
          </li>
          <li>Be valid (not expired)</li>
          <li>
            Be in English
            <v-tooltip
              text="If you do not have ID in English, you need to have it professionally translated. Please provide both the original and translated copies."
              location="top"
            >
              <template #activator="{ props }">
                <v-icon v-bind="props" icon="mdi-help-circle" variant="plain" />
              </template>
            </v-tooltip>
          </li>
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
          <v-select
            class="pt-2"
            :items="configStore.primaryIdentificationType"
            item-title="name"
            item-value="id"
            variant="outlined"
            label=""
            v-model="primaryIdType"
            :rules="[Rules.required('Select your primary ID type')]"
          ></v-select>
        </label>
        <label>Upload file</label>
        <FileUploader
          :allow-multiple-files="true"
          :max-number-of-files="3"
          :user-files="generateUserPrimaryFileArray"
          class="mt-1"
          @update:files="handlePrimaryFileUpload"
          :rules="[Rules.atLeastOneOptionRequired('You must add at least one file')]"
        />
      </div>
      <div class="mt-12">
        <ECEHeader title="Then, choose a secondary ID" />
        <label>
          What type of ID are you providing?
          <v-select
            class="pt-2"
            :items="configStore.secondaryIdentificationType"
            item-title="name"
            item-value="id"
            variant="outlined"
            label=""
            v-model="secondaryIdType"
            :rules="[Rules.required('Select your secondary ID type'), Rules.notSameAs(primaryIdType, 'Select a different type of ID than your primary ID')]"
          ></v-select>
        </label>
        <label>Upload file</label>
        <FileUploader
          :allow-multiple-files="true"
          :max-number-of-files="3"
          :user-files="generateUserSecondaryFileArray"
          class="mt-1"
          @update:files="handleSecondaryFileUpload"
          :rules="[Rules.atLeastOneOptionRequired('You must add at least one file')]"
        />
      </div>
      <v-btn
        @click="handleSubmit()"
        :loading="loadingStore.isLoading('profileVerification_post')"
        class="my-8"
        :size="smAndDown ? 'default' : 'large'"
        color="primary"
        append-icon="mdi-arrow-right"
      >
        Send for verification
      </v-btn>
    </v-form>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";
import * as Rules from "@/utils/formRules";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import IconCard from "@/components/IconCard.vue";
import FileUploader from "@/components/FileUploader.vue";
import Callout from "@/components/Callout.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useDisplay } from "vuetify";
import type { FileItem } from "@/components/UploadFileItem.vue";
import ECEHeader from "../ECEHeader.vue";
import { useConfigStore } from "@/store/config";
import type { Components, IdentificationType, ProfileIdentification } from "@/types/openapi";
import { parseHumanFileSize, removeElementByIndex, replaceElementByIndex } from "@/utils/functions";
import { useLoadingStore } from "@/store/loading";
import * as Functions from "@/utils/functions";
import { useAlertStore } from "@/store/alert";
import { postProfileVerification } from "@/api/profile";
import type { VForm } from "vuetify/components/VForm";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "VerifyIdentification",
  components: { PageContainer, Breadcrumb, IconCard, ECEHeader, Callout, FileUploader },
  setup() {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const configStore = useConfigStore();
    const loadingStore = useLoadingStore();
    const route = useRoute();
    const router = useRouter();
    const { smAndDown } = useDisplay();
    const alertStore = useAlertStore();
    return { userStore, oidcStore, configStore, route, router, smAndDown, Rules, alertStore, loadingStore };
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

    return {
      items,
      primaryIdType: "",
      primaryIdFiles: [] as Components.Schemas.IdentityDocument[],
      newPrimaryIdFiles: [],
      secondaryIdType: "",
      secondaryIdFiles: [] as Components.Schemas.IdentityDocument[],
      newSecondaryIdFiles: [],
      arePrimaryAttachedFilesValid: true,
      areSecondaryAttachedFilesValid: true,
      isFileUploadInProgress: false,
      isAtleastOnePrimaryFileAdded: false,
      isAtleastOneSecondaryFileAdded: false,
    };
  },
  methods: {
    async handleSubmit() {
      const { valid } = await (this.$refs.verifyIdentificationForm as VForm).validate();

      if (valid) {
        const identification: ProfileIdentification = {
          primaryIdTypeObjectId: this.primaryIdType,
          secondaryIdTypeObjectId: this.secondaryIdType,
          primaryIds: this.primaryIdFiles,
          secondaryIds: this.secondaryIdFiles,
        };
        const { error } = await postProfileVerification(identification);
        if (!error) {
          this.alertStore.setSuccessAlert("ID received");
          this.router.push("/");
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      }
    },
    async handlePrimaryFileUpload(filesArray: any[]) {
      this.arePrimaryAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      this.isAtleastOnePrimaryFileAdded = false;

      // Reset attachments
      this.primaryIdFiles = [];

      if (filesArray && filesArray.length > 0) {
        this.isAtleastOnePrimaryFileAdded = true;
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

          // Check for file errors
          if (file.fileErrors && file.fileErrors.length > 0) {
            this.arePrimaryAttachedFilesValid = false;
          }

          // Check if file is still uploading
          else if (file.progress < 101) {
            this.isFileUploadInProgress = true;
          }

          // If file is valid and fully uploaded, add to attachments
          if (this.arePrimaryAttachedFilesValid && !this.isFileUploadInProgress) {
            this.primaryIdFiles.push({
              id: file.fileId,
              name: file.file.name,
              size: Functions.humanFileSize(file.file.size),
              extention: file.file.name.split(".").pop(),
            });
          }
        }
      }
    },
    async handleSecondaryFileUpload(filesArray: any[]) {
      this.areSecondaryAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      this.isAtleastOneSecondaryFileAdded = false;

      // Reset attachments
      this.secondaryIdFiles = [];

      if (filesArray && filesArray.length > 0) {
        this.isAtleastOneSecondaryFileAdded = true;
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

          // Check for file errors
          if (file.fileErrors && file.fileErrors.length > 0) {
            this.areSecondaryAttachedFilesValid = false;
          }

          // Check if file is still uploading
          else if (file.progress < 101) {
            this.isFileUploadInProgress = true;
          }

          // If file is valid and fully uploaded, add to attachments
          if (this.areSecondaryAttachedFilesValid && !this.isFileUploadInProgress) {
            this.secondaryIdFiles.push({
              id: file.fileId,
              name: file.file.name,
              size: Functions.humanFileSize(file.file.size),
              extention: file.file.name.split(".").pop(),
            });
          }
        }
      }
    },
  },
  computed: {
    generateUserPrimaryFileArray() {
      const userFileList: FileItem[] = [];

      if (this.primaryIdFiles) {
        for (let file of this.primaryIdFiles) {
          const newFileItem: FileItem = {
            fileId: file.id!,
            fileErrors: [],
            fileSize: parseHumanFileSize(file.size!),
            fileName: file.name!,
            progress: 101,
            file: new File([], file.name!),
            storageFolder: "permanent",
          };

          userFileList.push(newFileItem);
        }
      }

      if (this.newPrimaryIdFiles) {
        for (let each of this.newPrimaryIdFiles) {
          userFileList.push(each);
        }
      }

      return userFileList;
    },
    generateUserSecondaryFileArray() {
      const userFileList: FileItem[] = [];

      if (this.secondaryIdFiles) {
        for (let file of this.secondaryIdFiles) {
          const newFileItem: FileItem = {
            fileId: file.id!,
            fileErrors: [],
            fileSize: parseHumanFileSize(file.size!),
            fileName: file.name!,
            progress: 101,
            file: new File([], file.name!),
            storageFolder: "permanent",
          };

          userFileList.push(newFileItem);
        }
      }

      if (this.newSecondaryIdFiles) {
        for (let each of this.newSecondaryIdFiles) {
          userFileList.push(each);
        }
      }

      return userFileList;
    },
  },
});
</script>
