<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <v-btn
          prepend-icon="mdi-close"
          variant="text"
          text="Close"
          @click="showCloseDialog = true"
        ></v-btn>
        <v-divider class="mb-5" :style="{ opacity: 1 }" />
        <v-row>
          <v-col><h2>New message</h2></v-col>
        </v-row>
        <v-row>
          <v-col>
            Send a message to the ECE Registry.
            <span class="font-weight-bold">
              Please note that all users that have access to the PSP Portal at
              your institution will be able to read and reply to this thread.
            </span>
          </v-col>
        </v-row>
        <v-form ref="messageForm" v-model="formValid">
          <v-row class="mt-5">
            <v-col cols="6">
              <div>Subject</div>
              <v-text-field
                ref="subjectField"
                v-model="subject"
                class="mt-2"
                variant="outlined"
                :rules="[Rules.required('Required')]"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row class="mt-5">
            <v-col cols="6">
              <div>Category</div>
              <v-select
                ref="categorySelect"
                v-model="category"
                class="mt-2"
                variant="outlined"
                :items="initiableCommunicationCategoryOptions"
                item-title="label"
                item-value="value"
                :rules="[Rules.required('Required')]"
                hide-details="auto"
              ></v-select>
            </v-col>
          </v-row>
          <CommunicationCategoryTemplate :category="selectedCategory" />
          <v-row class="mt-5">
            <v-col>
              <div>Message</div>
              <v-textarea
                ref="textarea"
                v-model="text"
                class="mt-2"
                auto-grow
                counter="1000"
                maxlength="1000"
                color="primary"
                variant="outlined"
                hide-details="auto"
                :rules="[
                  Rules.required(
                    'Enter a message no longer than 1000 characters',
                  ),
                ]"
              ></v-textarea>
            </v-col>
          </v-row>
          <FileUploader
            ref="FileUploader"
            :max-number-of-files="maxNumberOfFiles"
            @update:files="handleFileUpdate"
          />
          <v-row class="mt-10">
            <v-col>
              <v-btn
                size="large"
                color="primary"
                :loading="loadingStore.isLoading('message_post')"
                @click="send"
              >
                Send
              </v-btn>
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
import type { ComponentPublicInstance, PropType } from "vue";
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import type { VForm } from "vuetify/components";
import FileUploader from "@/components/common/FileUploader.vue";
import { sendMessage } from "@/api/message";
import CommunicationCategoryTemplate from "@/components/communication/CommunicationCategoryTemplate.vue";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import PageContainer from "@/components/PageContainer.vue";
import { useAlertStore } from "@/store/alert";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";
import {
  initiableCommunicationCategoryOptions,
  isInitiableCommunicationCategory,
  type InitiableCommunicationCategory,
} from "@/utils/communicationCategory";
import * as Rules from "@/utils/formRules";
import * as Functions from "@/utils/functions";

interface NewMessage {
  text: string;
  subject: string;
  category: Components.Schemas.CommunicationCategory | null;
  Rules: any;
  showCloseDialog: boolean;
  areAttachedFilesValid: boolean;
  isFileUploadInProgress: boolean;
  formValid: boolean;
  attachments: Components.Schemas.CommunicationDocument[];
}

export default defineComponent({
  name: "NewMessage",
  components: {
    CommunicationCategoryTemplate,
    PageContainer,
    ConfirmationDialog,
    FileUploader,
  },
  props: {
    initialCategory: {
      type: String as PropType<Components.Schemas.CommunicationCategory | null>,
      default: null,
    },
  },
  setup() {
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();
    const router = useRouter();
    const maxNumberOfFiles = 5;

    return {
      loadingStore,
      alertStore,
      initiableCommunicationCategoryOptions,
      maxNumberOfFiles,
      router,
    };
  },
  data(): NewMessage {
    return {
      text: "",
      Rules,
      showCloseDialog: false,
      areAttachedFilesValid: true,
      isFileUploadInProgress: false,
      formValid: false,
      attachments: [],
      subject: "",
      category: isInitiableCommunicationCategory(this.initialCategory)
        ? this.initialCategory
        : null,
    };
  },
  computed: {
    selectedCategory(): InitiableCommunicationCategory | null {
      return isInitiableCommunicationCategory(this.category)
        ? this.category
        : null;
    },
  },
  methods: {
    scrollToComponent(component: ComponentPublicInstance | undefined) {
      if (component?.$el) {
        component.$el.scrollIntoView({ behavior: "smooth" });
      }
    },
    async send() {
      const { valid } = await (this.$refs.messageForm as VForm).validate();
      if (this.isFileUploadInProgress) {
        this.alertStore.setFailureAlert(
          "Uploading files in progress. Please wait until files are uploaded and try again.",
        );
      } else if (valid && this.selectedCategory) {
        const { error } = await sendMessage({
          communication: {
            subject: this.subject,
            text: this.text,
            category: this.selectedCategory,
            documents: this.attachments,
          },
        });
        if (error) {
          this.alertStore.setFailureAlert(
            "Sorry, something went wrong and your changes could not be saved. Try again later.",
          );
        } else {
          this.alertStore.setSuccessAlert("Message sent successfully.");
          this.router.push("/messages");
        }
      } else {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format to continue.",
        );

        let component = this.$refs.subjectField as
          | ComponentPublicInstance<{ $el: HTMLElement }>
          | undefined;

        if (this.subject.trim()) {
          if (!this.selectedCategory) {
            component = this.$refs.categorySelect as
              | ComponentPublicInstance<{ $el: HTMLElement }>
              | undefined;
          } else if (!this.text.trim()) {
            component = this.$refs.textarea as
              | ComponentPublicInstance<{ $el: HTMLElement }>
              | undefined;
          } else {
            component = this.$refs.FileUploader as
              | ComponentPublicInstance<{ $el: HTMLElement }>
              | undefined;
          }
        }

        this.scrollToComponent(component);
      }
    },
    handleFileUpdate(filesArray: any[]) {
      this.areAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      this.attachments = [];
      if (filesArray && filesArray.length > 0) {
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

          if (file.fileErrors && file.fileErrors.length > 0) {
            this.areAttachedFilesValid = false;
          } else if (file.progress < 101) {
            this.isFileUploadInProgress = true;
          }

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
