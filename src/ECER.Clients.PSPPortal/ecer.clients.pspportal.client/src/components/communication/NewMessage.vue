<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <v-btn prepend-icon="mdi-close" variant="text" text="Close" @click="showCloseDialog = true"></v-btn>
        <v-divider :style="{ opacity: 1 }" />
        <v-row>
          <v-col>
            New message
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            Send a message to the ECE Registry. 
            <span class="font-weight-bold">Please note that all program representatives at your institution will be able to read and reply to this thread.</span>
          </v-col>
        </v-row>
        <v-form ref="messageForm" v-model="formValid">
          <v-row class="mt-5">
            <v-col cols="6">
              <div>Subject</div>
              <v-text-field v-model="subject" class="mt-2" variant="outlined"
                :rules="[Rules.required('Required')]"></v-text-field>
            </v-col>
          </v-row>
          <v-row class="mt-5">
            <v-col>
              <div>Message</div>
              <v-textarea ref="textarea" v-model="text" class="mt-2" auto-grow counter="1000" maxlength="1000"
                color="primary" variant="outlined" hide-details="auto"
                :rules="[Rules.required('Enter a message no longer than 1000 characters')]"></v-textarea>
            </v-col>
          </v-row>
          <FileUploader ref="FileUploader" :max-number-of-files="maxNumberOfFiles" @update:files="handleFileUpdate" />
          <v-row class="mt-10">
            <v-col>
              <v-btn size="large" color="primary" :loading="loadingStore.isLoading('message_post')"
                @click="send">Send</v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-col>
    </v-row>
  </PageContainer>
  <ConfirmationDialog :cancel-button-text="'Continue editing'" :accept-button-text="'Delete message'"
    :title="'Delete Message?'" :show="showCloseDialog" @cancel="showCloseDialog = false"
    @accept="router.push('/messages')">
    <template #confirmation-text>
      <p>Your message will be deleted. It will not be sent.</p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import type { ComponentPublicInstance } from "vue";
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";
import type { VForm } from "vuetify/components";
import FileUploader from "@/components/common/FileUploader.vue";
import { getChildMessages, sendMessage } from "@/api/message";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useMessageStore } from "@/store/message";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
import * as Functions from "@/utils/functions";
interface NewMessage {
  text: string;
  subject: string;
  Rules: any;
  showCloseDialog: boolean;
  areAttachedFilesValid: boolean;
  isFileUploadInProgress: boolean;
  formValid: boolean;
  attachments: Components.Schemas.CommunicationDocument[];
}

export default defineComponent({
  name: "NewMessage",
  components: { PageContainer, ConfirmationDialog, FileUploader },
  async setup() {
    const messageStore = useMessageStore();
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();
    const router = useRouter();
    const route = useRoute();
    const maxNumberOfFiles = 5;

    return {
      messageStore,
      loadingStore,
      alertStore,
      maxNumberOfFiles,
      router
    };
  },
  data(): NewMessage {
    return {
      text: '',
      Rules,
      showCloseDialog: false,
      areAttachedFilesValid: true,
      isFileUploadInProgress: false,
      formValid: false,
      attachments: [],
      subject: ''
    };
  },
  methods: {
    scrollToComponent(component: ComponentPublicInstance) {
      if (component?.$el) {
        component.$el.scrollIntoView({ behavior: "smooth" });
      }
    },
    async send() {
      const { valid } = await (this.$refs.messageForm as VForm).validate();
      if (this.isFileUploadInProgress) {
        this.alertStore.setFailureAlert("Uploading files in progress. Please wait until files are uploaded and try again.");
      } else if (valid) {
        const { error } = await sendMessage({ communication: { subject: this.subject, text: this.text, documents: this.attachments } });
        if (error) {
          this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
        } else {
          this.alertStore.setSuccessAlert("Message sent successfully.");
          this.router.push("/messages");
        }
      } else {
        let component;
        if (!this.text.trim()) {
          this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
          component = this.$refs.textarea as ComponentPublicInstance<{ $el: HTMLElement }>;
        } else {
          component = this.$refs.FileUploader as ComponentPublicInstance<{ $el: HTMLElement }>;
        }
        this.scrollToComponent(component);
      }
    },
    handleFileUpdate(filesArray: any[]) {
      this.areAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      this.attachments = []; // Reset attachments
      if (filesArray && filesArray.length > 0) {
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

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
            this.attachments.push({
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
});
</script>
