<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="smAndDown">
    <v-dialog :model-value="smAndDown && messageStore.currentMessage != null" :transition="false" fullscreen
      @update:model-value="messageStore.currentMessage = null">
      <v-card>
        <v-toolbar color="white">
          <v-btn prepend-icon="mdi-close" text="Close" @click="messageStore.currentMessage = null"></v-btn>
        </v-toolbar>
        <v-card-text>
          <v-sheet v-if="messageStore.currentMessage?.doNotReply == false" class="message-reply mb-6">
            <v-btn prepend-icon="mdi-reply" variant="text" color="primary" text="Reply"
              @click="handleMessageReply"></v-btn>
          </v-sheet>
          <h2>{{ messageStore.currentMessage?.subject }}</h2>

          <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
            <span>{{ messageFromString(message) }}</span>
            <div class="mt-3"
              v-html="`${formatDate(String(message.notifiedOn), 'LLL d, yyyy')} &nbsp; ${formatDate(String(message.notifiedOn), 't')}`">
            </div>
            <div class="mt-6" v-html="message.text"></div>
            <div v-if="message.documents!.length > 0" class="mt-6">
              <p>
                <v-icon class="ml-n2">mdi-paperclip</v-icon>
                Attachments
              </p>
              <!-- <div v-for="(file, fileIndex) in message.documents" :key="fileIndex" class="mt-3">
                <DownloadFileLink :name="file.name" :get-file-function="() => getCommunicationFile(message.id || '', file.id || '')">
                  <div>{{ `${file.name} (${file.size!.replace(/\s+/g, "")})` }}</div>
                </DownloadFileLink>
              </div> -->
            </div>
            <v-divider v-if="index < messageStore.currentThread!.length - 1" color="ash-grey"
              class="mt-10 border-opacity-100"></v-divider>
          </div>
          <div v-if="messageStore.currentMessage?.doNotReply">
            <v-divider color="ash-grey" class="mt-12 border-opacity-100"></v-divider>
            <div class="mt-2">No reply option available for this message.</div>
          </div>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
  <div v-if="mdAndUp && messageStore.currentMessage !== null">
    <v-sheet v-if="messageStore.currentMessage?.doNotReply === false || messageStore.currentMessage?.applicationId"
      class="message-reply mb-6">
      <v-btn v-if="messageStore.currentMessage?.doNotReply === false" prepend-icon="mdi-reply" variant="text"
        color="primary" text="Reply" @click="handleMessageReply"></v-btn>
      <span
        v-if="messageStore.currentMessage?.doNotReply === false && messageStore.currentMessage?.applicationId">|</span>
      <v-btn v-if="messageStore.currentMessage?.applicationId" prepend-icon="mdi-list-box" variant="text"
        color="primary" text="Go to application summary" @click="handleApplicationSummary"></v-btn>
    </v-sheet>
    <h2>{{ messageStore.currentMessage?.subject }}</h2>
    <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
      <span>{{ messageFromString(message) }}</span>
      <div class="mt-3"
        v-html="`${formatDate(String(message.notifiedOn), 'LLL d, yyyy')} &nbsp; ${formatDate(String(message.notifiedOn), 't')}`">
      </div>
      <div class="mt-6" v-html="message.text"></div>
      <div v-if="message.documents!.length > 0" class="mt-6">
        <p>
          <v-icon class="ml-n2">mdi-paperclip</v-icon>
          Attachments
        </p>
        <!-- <div v-for="(file, fileIndex) in message.documents" :key="fileIndex" class="mt-3">
          <DownloadFileLink :name="file.name" :get-file-function="() => getCommunicationFile(message.id || '', file.id || '')">
            <div>{{ `${file.name} (${file.size!.replace(/\s+/g, "")})` }}</div>
          </DownloadFileLink>
        </div> -->
      </div>

      <v-divider v-if="index < messageStore.currentThread!.length - 1" color="ash-grey"
        class="mt-10 border-opacity-100"></v-divider>
    </div>
    <div v-if="messageStore.currentMessage?.doNotReply">
      <v-divider color="ash-grey" class="mt-12 border-opacity-100"></v-divider>
      <div class="mt-2">No reply option available for this message.</div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
// import { getCommunicationFile } from "@/api/message";
import { useMessageStore } from "@/store/message";
import { formatDate } from "@/utils/format";
import { useLoadingStore } from "@/store/loading";
import DownloadFileLink from "./DownloadFileLink.vue";
import type { Communication } from "@/types/openapi";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "Message",
  components: { DownloadFileLink },
  setup() {
    const messageStore = useMessageStore();
    const loadingStore = useLoadingStore();
    const { smAndDown, mdAndUp } = useDisplay();
    const router = useRouter();

    return {
      messageStore,
      loadingStore,
      smAndDown,
      mdAndUp,
      // getCommunicationFile,
      router,
    };
  },
  computed: {
    messageDate(): string {
      let message = this.messageStore.currentMessage;
      return message ? formatDate(String(message.notifiedOn), "LLL d, yyyy t") : "";
    },
  },
  methods: {
    formatDate,
    handleMessageReply() {
      this.router.push({
        name: "replyToMessage",
        params: { messageId: this.messageStore.currentMessage?.id },
      });
      this.messageStore.currentMessage = null; // Putting this in to make router redirect correctly for mobile devices
    },
    handleApplicationSummary() {
      this.router.push({
        name: "manageApplication",
        params: { applicationId: this.messageStore.currentMessage?.applicationId },
      });
      this.messageStore.currentMessage = null;
    },
    messageFromString(message: Communication): string {
      switch (message.from) {
        case "Registry":
          return "From ECE Registry";
        case "PortalUser":
          return "You Replied";
        case "Investigation":
          return "From ECE Investigations";
        default:
          return "";
      }
    },
  },
});
</script>
<style scoped>
.message-reply {
  background-color: #e4e4e4;
  height: 48px;
  display: flex;
  align-items: center;
}
</style>
