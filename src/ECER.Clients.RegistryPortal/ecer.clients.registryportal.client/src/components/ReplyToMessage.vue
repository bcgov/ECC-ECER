<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <v-btn prepend-icon="mdi-close" variant="text" text="Close" @click="showCloseDialog = true"></v-btn>
        <hr class="w-full" />
        <h1 class="mt-5">Re: {{ messageThreadSubject }}</h1>
        <v-row class="mt-5">
          <v-col>
            <div>Message</div>
            <v-textarea
              v-model="text"
              class="mt-2"
              auto-grow
              counter="1000"
              maxlength="1000"
              color="primary"
              variant="outlined"
              hide-details="auto"
              :rules="[Rules.required('Enter a message no longer than 1000 characters')]"
            ></v-textarea>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <p>
              <v-icon size="large" icon="mdi-attachment" />
              Attachments
            </p>
            <p>You can only upload PDF files up to 10MB.</p>
            <v-btn prepend-icon="mdi-plus" variant="text" color="primary" class="mt-3" @click="triggerFileInput">Add file</v-btn>
            <v-file-input ref="fileInput" style="display: none" multiple accept="application/pdf" @change="handleFileUpload"></v-file-input>

            <v-list lines="two" class="flex-grow-1 message-list">
              <UploadFileItem
                v-for="(file, index) in selectedFiles"
                :key="index"
                :file="file.file"
                :upload-progress="file.progress"
                @delete-file="removeFile"
              />
            </v-list>
          </v-col>
        </v-row>
        <v-row class="mt-10">
          <v-col>
            <v-btn size="large" color="primary" :loading="loadingStore.isLoading('message_post')" @click="handleReplyToMessage">Send</v-btn>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
  <ConfirmationDialog
    :cancel-button-text="'Continue editing'"
    :accept-button-text="'Delete message'"
    :title="'Delete Message?'"
    :show="showCloseDialog"
    :is-cancel-button-first="true"
    @cancel="showCloseDialog = false"
    @accept="router.push('/messages')"
  >
    <template #confirmation-text>
      <p>Your message will be deleted. It will not be sent.</p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import type { AxiosProgressEvent } from "axios";
import { v4 as uuidv4 } from "uuid";
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";

import { uploadFile } from "@/api/file";
import { getChildMessages, sendMessage } from "@/api/message";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import PageContainer from "@/components/PageContainer.vue";
import UploadFileItem from "@/components/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useMessageStore } from "@/store/message";
import * as Rules from "@/utils/formRules";
import * as Functions from "@/utils/functions";

interface FileProgress {
  file: File;
  progress: number;
}

interface ReplyToMessageData {
  selectedFiles: FileProgress[];
  text: string;
  Rules: any;
  showCloseDialog: boolean;
}

export default defineComponent({
  name: "ReplyToMessage",
  components: { PageContainer, ConfirmationDialog, UploadFileItem },
  async setup() {
    const messageStore = useMessageStore();
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();
    const router = useRouter();
    const route = useRoute();
    const messageId = route.params.messageId.toString();

    const messageThread = (await getChildMessages({ parentId: messageId })).data?.communications;
    let messageThreadSubject = "";
    if (messageThread.length > 0) {
      messageThreadSubject = messageThread[0].subject;
    }

    return {
      messageStore,
      loadingStore,
      alertStore,
      router,
      messageId,
      messageThreadSubject,
    };
  },
  data(): ReplyToMessageData {
    return {
      selectedFiles: [],
      text: " ",
      Rules,
      showCloseDialog: false,
    };
  },
  methods: {
    async handleReplyToMessage() {
      if (this.text.trim().length > 0) {
        const { error } = await sendMessage({ communication: { id: this.messageId, text: this.text } });
        if (error) {
          this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
        } else {
          this.alertStore.setSuccessAlert("Message sent successfully.");
          this.router.push("/messages");
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
    handleFileUpload(event: Event) {
      const target = event.target as HTMLInputElement;
      const files = target.files;
      if (files) {
        for (let i = 0; i < files.length; i++) {
          const file = files[i];
          if (file.size > 10 * 1024 * 1024) {
            this.alertStore.setFailureAlert("File size exceeds the 10MB limit.");
            continue; // Skip this file and continue with the next one
          }
          if (file.type !== "application/pdf") {
            this.alertStore.setFailureAlert("File type must be PDF.");
            continue; // Skip this file and continue with the next one
          }
          if (this.selectedFiles.some((f: FileProgress) => f.file.name === file.name)) {
            this.alertStore.setFailureAlert("File with the same name already exists.");
            continue; // Skip this file and continue with the next one
          }
          this.selectedFiles.push({ file, progress: 0 });
          this.uploadFileWithProgress(file);
        }
      }
    },
    async uploadFileWithProgress(file: File) {
      const fileId = uuidv4(); // Generate a unique file ID using uuid
      const fileClassification = "document";
      const fileTags = this.formatFileTags(file);
      try {
        const response = await uploadFile(fileId, file, fileClassification, fileTags, (progressEvent: AxiosProgressEvent) => {
          const total = progressEvent.total ? progressEvent.total : 10485760;
          const progress = Math.round((progressEvent.loaded * 100) / total);
          const fileIndex = this.selectedFiles.findIndex((f: FileProgress) => f.file === file);
          if (fileIndex > -1) {
            this.selectedFiles[fileIndex].progress = progress;
          }
        });

        if (!response.data) {
          this.removeFile(file);
          this.alertStore.setFailureAlert("An error occurred during file upload");
        }
      } catch (error) {
        this.removeFile(file);
        this.alertStore.setFailureAlert("An error occurred during file upload");
        console.log(error);
      }
    },
    formatFileTags(file: File): string {
      const fileName = file.name;
      const fileSize = Functions.humanFileSize(file.size); // Convert size to KB
      const fileFormat = file.name.split(".").pop(); // Get file extension

      const tags = {
        Name: fileName,
        Size: fileSize,
        Format: fileFormat,
      };

      return Object.entries(tags)
        .map(([key, value]) => `${key}=${value}`)
        .join(",");
    },
    removeFile(file: File) {
      this.selectedFiles = this.selectedFiles.filter((f: FileProgress) => f.file !== file);
    },
    triggerFileInput() {
      (this.$refs.fileInput as any).click();
    },
  },
});
</script>
