<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <v-btn prepend-icon="mdi-close" variant="text" text="Close" @click="showCloseDialog = true"></v-btn>
        <hr class="w-full" />
        <h1 class="mt-5">{{ messageStore.currentMessage?.subject }}</h1>
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
            <input ref="fileInput" type="file" style="display: none" accept="application/pdf" @change="handleFileUpload" />

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
    :cancel-button-text="'Cancel'"
    :accept-button-text="'Delete message'"
    :title="'Delete Message?'"
    :show="showCloseDialog"
    @cancel="showCloseDialog = false"
    @accept="router.push('/messages')"
  >
    <template #confirmation-text>
      <p>Your message will be deleted. It will not be sent.</p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { useRouter } from "vue-router";
import { uploadFile } from "@/api/file";
import { v4 as uuidv4 } from 'uuid';
import { sendMessage } from "@/api/message";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import PageContainer from "@/components/PageContainer.vue";
import UploadFileItem from "@/components/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useMessageStore } from "@/store/message";
import * as Rules from "@/utils/formRules";
import type { AxiosProgressEvent } from "axios";

export default defineComponent({
  name: "ReplyToMessage",
  components: { PageContainer, ConfirmationDialog, UploadFileItem },
  setup() {
    const messageStore = useMessageStore();
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();
    const router = useRouter();
    const fileInput = ref<HTMLInputElement | null>(null);
    const selectedFiles = ref<{ file: File; progress: number }[]>([]);
    const formatFileTags = (file: File): string => {
    const fileName = file.name;
    const fileSize = (file.size / 1024).toFixed(2) + ' KB'; // Convert size to KB
    const fileFormat = file.name.split('.').pop(); // Get file extension
    
  const tags = {
    Name: fileName,
    Size: fileSize,
    Format: fileFormat,
  };

  return Object.entries(tags).map(([key, value]) => `${key}=${value}`).join(',');
    };
    const triggerFileInput = () => {
      fileInput.value?.click();
    };

    const handleFileUpload = (event: Event) => {
      const target = event.target as HTMLInputElement;
      const file = target.files?.[0];
      if (file) {
        if (file.size > 10 * 1024 * 1024) {
          alertStore.setFailureAlert("File size exceeds the 10MB limit.");
          return;
        }

        selectedFiles.value.push({ file, progress: 0 });
        uploadFileWithProgress(file);
      }
    };

    const uploadFileWithProgress = async (file: File) => {
      const fileId = uuidv4(); // Generate a unique file ID using uuid
      const fileClassification = "document";
      const fileTags = formatFileTags(file);
      try {
      const response = await uploadFile(fileId, file, fileClassification, fileTags, (progressEvent: AxiosProgressEvent) => {
        const total = progressEvent.total?progressEvent.total:10485760;
        const progress = Math.round((progressEvent.loaded * 100) / total);
        const fileIndex = selectedFiles.value.findIndex(f => f.file === file);
        if (fileIndex > -1) {
          selectedFiles.value[fileIndex].progress = progress;
        }
      });

      if (response.data) {
        
      } else {
        alertStore.setFailureAlert("An error occurred during file upload");
      }
    } catch (error) {
       alertStore.setFailureAlert("An error occurred during file upload");
    }
    };

    const removeFile = (file: File) => {
      selectedFiles.value = selectedFiles.value.filter(f => f.file !== file);
    };

    return {
      messageStore,
      loadingStore,
      alertStore,
      router,
      fileInput,
      triggerFileInput,
      handleFileUpload,
      selectedFiles,
      removeFile,
    };
  },
  data() {
    return {
      text: " ",
      Rules,
      showCloseDialog: false,
    };
  },
  methods: {
    async handleReplyToMessage() {
      if (this.text.trim().length > 0) {
        const { error } = await sendMessage({ communication: { id: this.messageStore.currentMessage?.id, text: this.text } });
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
  },
});
</script>
