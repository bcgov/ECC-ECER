<template>
  <v-container>
    <Breadcrumb :items="items" />
    <div class="d-flex flex-column ga-3 mb-6">
      <h2>Course outlines or syllabi</h2>
      <p>You need to provide detailed course outlines or syllabi for:</p>
      <ul class="ml-10">
        <li>
          <b>{{ transcript?.programName }}</b>
        </li>
      </ul>
      <p>Ask your educational institution for detailed course outlines or syllabi. You cannot create these yourself.</p>
      <p>The outlines must:</p>
      <ul class="ml-10">
        <li>Include detailed descriptions of course content, learning goals, outcomes and expectations</li>
        <li>Be for the year(s) you completed the course(s)</li>
        <li>Be in English</li>
      </ul>
    </div>
    <h3>How will you provide your course outlines or syllabi?</h3>
    <v-form ref="updateCourseOutlineOptionsAndDocuments" validate-on="input">
      <v-row class="mt-4">
        <v-radio-group id="courseOutlineRadio" v-model="courseOutlineOptions" :rules="[Rules.required()]" color="primary">
          <v-radio label="I have my course outlines or syllabi and will upload them now" value="UploadNow"></v-radio>
          <v-radio
            label="The ECE Registry already has my course outlines or syllabi on file for the course or program relevant to this application and certificate type"
            value="RegistryAlreadyHas"
          ></v-radio>
        </v-radio-group>
      </v-row>
      <v-row v-if="showFileInput">
        <v-col>
          <p class="mb-3"><b>Attach files</b></p>
          <p class="mb-3">
            Check that you have all the relevant course outlines or syllabi before you add them here. The ECE Registry will contact you if additional
            information is required.
          </p>
          <FileUploader
            :user-files="generateUserPrimaryFileArray"
            :show-add-file-button="true"
            :max-number-of-files="24"
            :can-delete-files="false"
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
import Breadcrumb from "./Breadcrumb.vue";
import FileUploader from "@/components/FileUploader.vue";
import type { FileItem } from "./UploadFileItem.vue";
import * as Rules from "@/utils/formRules";
import { parseHumanFileSize } from "@/utils/functions";

import type { CourseOutlineOptions } from "@/types/openapi";
import type { VForm } from "vuetify/components";
import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "ViewCourseOutline",
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
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const route = useRoute();

    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    const transcript = applicationStatus?.transcriptsStatus?.find((transcript) => transcript.id === props.transcriptId);

    let courseOutlineOptions = ref<CourseOutlineOptions | undefined>(undefined);
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
        title: "Course oultine",
        disabled: true,
        href: `/manage-application/${props.applicationId}/transcript/${props.transcriptId}/course-outline`,
      },
    ];

    if (!transcript) {
      router.back();
    } else {
      // Set courseOutlineOptions based on a field from transcript
      courseOutlineOptions = ref(transcript.courseOutlineOptions || undefined);
    }

    return { router, transcript, alertStore, Rules, courseOutlineOptions, items, areAttachedFilesValid, isFileUploadInProgress, newFiles, loadingStore };
  },
  computed: {
    generateUserPrimaryFileArray() {
      const userFileList: FileItem[] = [];

      if (this.transcript?.courseOutlineFiles) {
        for (let file of this.transcript?.courseOutlineFiles) {
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
      return this.courseOutlineOptions === "UploadNow";
    },
  },
  methods: {
    async handleSubmit() {
      // Validate the form
      const { valid } = await (this.$refs.updateCourseOutlineOptionsAndDocuments as VForm).validate();
      if (this.isFileUploadInProgress) {
        this.alertStore.setFailureAlert("Uploading files in progress. Please wait until files are uploaded and try again.");
      } else if (valid) {
        const { error } = await setTranscriptDocumentsAndOptions({
          courseOutlineOptions: this.courseOutlineOptions,
          programConfirmationOptions: this.transcript?.programConfirmationOptions,
          comprehensiveReportOptions: this.transcript?.comprehensiveReportOptions,
          newCourseOutlineFiles: this.newFiles,
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

          // Check if file exists in transcript.courseOutlineFiles
          if (this.transcript?.courseOutlineFiles?.find((f) => f.id === file.fileId)) {
            // If file exists in transcript.courseOutlineFiles, continue
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
