<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <v-btn prepend-icon="mdi-close" variant="text" text="Close" @click="showCloseDialog = true"></v-btn>
        <hr class="w-full" />
        <h1 class="mt-5">Re: {{ messageThreadSubject }}</h1>
        <v-form ref="replyForm" v-model="formValid">
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
          <FileUploader @update:files="handleFileUpdate" />
          <v-row class="mt-10">
            <v-col>
              <v-btn size="large" color="primary" :loading="loadingStore.isLoading('message_post')" @click="handleReplyToMessage">Send</v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-col>
    </v-row>
  </PageContainer>
  <ConfirmationDialog
    :cancel-button-text="'Continue editing'"
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
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";

import { getChildMessages, sendMessage } from "@/api/message";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import FileUploader from "@/components/FileUploader.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import { useMessageStore } from "@/store/message";
import * as Rules from "@/utils/formRules";

interface ReplyToMessageData {
  text: string;
  Rules: any;
  showCloseDialog: boolean;
  areAttachedFilesValid: boolean;
  isFileUploadInProgress: boolean;
  formValid: boolean;
}

export default defineComponent({
  name: "ReplyToMessage",
  components: { PageContainer, ConfirmationDialog, FileUploader },
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
      text: "",
      Rules,
      showCloseDialog: false,
      areAttachedFilesValid: true,
      isFileUploadInProgress: false,
      formValid: false,
    };
  },
  methods: {
    async handleReplyToMessage() {
      const { valid } = await (this.$refs.replyForm as any).validate();
      if (this.isFileUploadInProgress) {
        this.alertStore.setFailureAlert("Uploading files. You need to wait until files are uploaded to send this message.");
      } else if (!this.areAttachedFilesValid) {
        this.alertStore.setFailureAlert("You must upload valid files.");
      } else if (valid) {
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

    handleFileUpdate(filesArray: any[]) {
      this.areAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      if (filesArray && filesArray.length > 0) {
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
});
</script>
