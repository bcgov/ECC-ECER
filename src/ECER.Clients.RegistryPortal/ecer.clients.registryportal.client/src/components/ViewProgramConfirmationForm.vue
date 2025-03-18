<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div class="d-flex flex-column ga-3 mb-6">
      <h2>Program Confirmation Form</h2>
      <p>You need to provide additional details about the program:</p>
      <ul class="ml-10">
        <li>
          <b>{{ transcript?.programName }}</b>
        </li>
      </ul>
      <p>You will need to:</p>
      <ol class="ml-10">
        <li>
          Download a
          <a href="https://www2.gov.bc.ca/assets/download/1DD5579B6A474ED2B095FD13B3268DA0 ">Program Confirmation Form (16KB, PDF).</a>
        </li>
        <li>Complete Section 1 of the form.</li>
        <li>
          Ask your educational institution to complete the rest of the form in English. If they cannot complete it in English, ask them to send the completed
          form directly to a professional translator.
        </li>
        <li>Upload the completed form after you get it back from your educational institution or translator.</li>
      </ol>
    </div>
    <h3>How will you provide your Program Confirmation Form?</h3>
    <v-form ref="updateProgramConfirmationOptionsAndDocuments" validate-on="input">
      <v-row class="mt-4">
        <v-radio-group id="programConfirmationRadio" v-model="programConfirmationOptions" :rules="[Rules.required()]" color="primary">
          <v-radio label="I have my Program Confirmation Form and will upload it now." value="UploadNow"></v-radio>
          <v-radio
            label="The ECE Registry already has my Program Confirmation Form on file for the program relevant to this application and certificate type."
            value="RegistryAlreadyHas"
          ></v-radio>
        </v-radio-group>
      </v-row>
      <v-row v-if="showFileInput">
        <v-col>
          <p class="mb-3"><b>Add your Program Confirmation Form</b></p>
          <FileUploader
            :user-files="generateUserPrimaryFileArray"
            :show-add-file-button="true"
            :max-number-of-files="3"
            :can-delete-permanent-files="false"
            @update:files="handleFileUpdate"
          />
        </v-col>
      </v-row>
    </v-form>
    <v-row class="mt-6">
      <v-btn :loading="loadingStore.isLoading('application_update_transcript_post')" @click="handleSubmit" size="large" color="primary">Save</v-btn>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAlertStore } from "@/store/alert";
import { getApplicationStatus, setTranscriptDocumentsAndOptions } from "@/api/application";
import { useLoadingStore } from "@/store/loading";
import Breadcrumb from "./Breadcrumb.vue";
import FileUploader from "@/components/FileUploader.vue";
import type { FileItem } from "./UploadFileItem.vue";
import * as Rules from "@/utils/formRules";
import type { ProgramConfirmationOptions } from "@/types/openapi";
import { parseHumanFileSize } from "@/utils/functions";
import type { VForm } from "vuetify/components";

export default defineComponent({
  name: "ViewProgramConfirmationForm",
  components: { Breadcrumb, FileUploader },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
    transcriptId: {
      type: String,
      required: true,
    },
  },
  setup: async (props) => {
    const alertStore = useAlertStore();

    const router = useRouter();
    const route = useRoute();
    const loadingStore = useLoadingStore();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    const transcript = applicationStatus?.transcriptsStatus?.find((transcript) => transcript.id === props.transcriptId);

    let programConfirmationOptions = ref<ProgramConfirmationOptions | undefined>(undefined);
    const areAttachedFilesValid = ref(true);
    const isFileUploadInProgress = ref(false);
    const newFiles = ref<string[]>([]);

    const items: { title: string; disabled: boolean; href: string }[] = [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Application",
        disabled: false,
        href: `/manage-application/${props.applicationId}`,
      },
      {
        title: "Program confirmation",
        disabled: true,
        href: `/manage-application/${props.applicationId}/transcript/${props.transcriptId}/program-confirmation`,
      },
    ];

    if (!transcript) {
      router.back();
    } else {
      // Set programConfirmationOptions based on a field from transcript
      programConfirmationOptions = ref(transcript.programConfirmationOptions || undefined);
    }

    return { router, transcript, alertStore, Rules, programConfirmationOptions, items, areAttachedFilesValid, isFileUploadInProgress, newFiles, loadingStore };
  },
  computed: {
    generateUserPrimaryFileArray() {
      const userFileList: FileItem[] = [];

      if (this.transcript?.programConfirmationFiles) {
        for (let file of this.transcript?.programConfirmationFiles) {
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

      return userFileList;
    },
    showFileInput(): boolean {
      return this.programConfirmationOptions === "UploadNow";
    },
  },
  methods: {
    async handleSubmit() {
      // Validate the form
      const { valid } = await (this.$refs.updateProgramConfirmationOptionsAndDocuments as VForm).validate();
      if (this.isFileUploadInProgress) {
        this.alertStore.setFailureAlert("Uploading files in progress. Please wait until files are uploaded and try again.");
      } else if (valid) {
        const { error } = await setTranscriptDocumentsAndOptions({
          programConfirmationOptions: this.programConfirmationOptions,
          newProgramConfirmationFiles: this.newFiles,
          applicationId: this.applicationId,
          transcriptId: this.transcriptId,
        });
        if (error) {
          this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
        } else {
          this.alertStore.setSuccessAlert("Your changes have been saved.");
          this.router.push({ name: "manageApplication", params: { applicationId: this.applicationId } });
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
    handleFileUpdate(filesArray: FileItem[]) {
      this.areAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      this.newFiles = []; // Reset attachments
      if (filesArray && filesArray.length > 0) {
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

          // Check if file exists in transcript.programConfirmationFiles
          if (this.transcript?.programConfirmationFiles?.find((f) => f.id === file.fileId)) {
            // If file exists in transcript.programConfirmationFiles, continue
            continue;
          }

          // Check for file errors
          if (file.fileErrors && file.fileErrors.length > 0) {
            this.areAttachedFilesValid = false;
          }

          // Check if file is still uploading
          else if (file.progress < 101) {
            this.isFileUploadInProgress = true;
          }

          // If file is valid and fully uploaded, add to attachments
          if (this.areAttachedFilesValid && !this.isFileUploadInProgress) {
            this.newFiles?.push(file.fileId);
          }
        }
      }
    },
  },
});
</script>
