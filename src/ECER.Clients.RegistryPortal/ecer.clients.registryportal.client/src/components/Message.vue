<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="smAndDown">
    <v-dialog
      :model-value="smAndDown && messageStore.currentMessage != null"
      :transition="false"
      fullscreen
      @update:model-value="messageStore.currentMessage = null"
    >
      <v-card>
        <v-toolbar color="white">
          <v-btn prepend-icon="mdi-close" text="Close" @click="messageStore.currentMessage = null"></v-btn>
        </v-toolbar>
        <v-card-text>
          <v-btn prepend-icon="mdi-reply" variant="text" color="primary" text="Reply" @click="handleMessageReply">Reply</v-btn>
          <h2>{{ messageStore.currentMessage?.subject }}</h2>

          <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
            <span v-html="message.from == 'Registry' ? 'From ECE Registry' : 'PortalUser' ? 'You Replied' : ''"></span>
            <div class="mt-3" v-html="formatDate(String(message.notifiedOn), 'LLL d, yyyy t')"></div>
            <div class="mt-6" v-html="message.text"></div>
            <v-divider v-if="index < messageStore.currentThread!.length - 1" class="mt-6"></v-divider>
          </div>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
  <div v-if="mdAndUp && messageStore.currentMessage != null">
    <v-btn prepend-icon="mdi-reply" variant="text" color="primary" text="Reply" @click="handleMessageReply">Reply</v-btn>
    <h2>{{ messageStore.currentMessage?.subject }}</h2>
    <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
      <span v-html="message.from == 'Registry' ? 'From ECE Registry' : 'PortalUser' ? 'You Replied' : ''"></span>
      <div class="mt-3" v-html="formatDate(String(message.notifiedOn), 'LLL d, yyyy t')"></div>
      <div class="mt-6" v-html="message.text"></div>
      <v-divider v-if="index < messageStore.currentThread!.length - 1" class="mt-6"></v-divider>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import { useMessageStore } from "@/store/message";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "Message",
  setup() {
    const messageStore = useMessageStore();
    const { smAndDown, mdAndUp } = useDisplay();

    return { messageStore, smAndDown, mdAndUp };
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
      this.$router.push({ name: "replyToMessage", params: { messageId: this.messageStore.currentMessage?.id } });
    },
  },
});
</script>
