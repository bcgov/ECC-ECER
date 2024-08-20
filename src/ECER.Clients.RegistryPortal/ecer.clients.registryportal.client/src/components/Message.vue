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
          <v-btn
            v-if="messageStore.currentMessage?.doNotReply == false"
            prepend-icon="mdi-reply"
            variant="text"
            color="primary"
            text="Reply"
            @click="handleMessageReply"
          >
            Reply
          </v-btn>
          <h2 class="mt-6">{{ messageStore.currentMessage?.subject }}</h2>

          <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
            <span v-html="message.from == 'Registry' ? 'From ECE Registry' : 'PortalUser' ? 'You Replied' : ''"></span>
            <div class="mt-3" v-html="formatDate(String(message.notifiedOn), 'LLL d, yyyy t')"></div>
            <div class="mt-6" v-html="message.text"></div>
            <v-divider v-if="index < messageStore.currentThread!.length - 1" color="ash-grey" class="mt-10 border-opacity-100"></v-divider>
          </div>
          <div v-if="messageStore.currentMessage?.doNotReply">
            <v-divider color="ash-grey" class="mt-12 border-opacity-100"></v-divider>
            <div class="mt-2">No reply option available for this message.</div>
          </div>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
  <div v-if="mdAndUp && messageStore.currentMessage != null">
    <v-btn
      v-if="messageStore.currentMessage?.doNotReply == false"
      prepend-icon="mdi-reply"
      variant="text"
      color="primary"
      text="Reply"
      @click="handleMessageReply"
    >
      Reply
    </v-btn>
    <h2 class="mt-6">{{ messageStore.currentMessage?.subject }}</h2>
    <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
      <span v-html="message.from == 'Registry' ? 'From ECE Registry' : 'PortalUser' ? 'You Replied' : ''"></span>
      <div class="mt-3" v-html="formatDate(String(message.notifiedOn), 'LLL d, yyyy t')"></div>
      <div class="mt-6" v-html="message.text"></div>
      <div v-if="message.documents!.length > 0" class="mt-6">
      <p><v-icon>mdi-paperclip</v-icon> Attachments</p>
      <div class="mt-3" v-for="file in message.documents">
      <router-link class="small" to="" @click.native="OpenFile(file)">
        {{ file.name }} ({{ file.size!.replace(/\s+/g, '') }})
      </router-link>
      </div>
      </div>

      <v-divider v-if="index < messageStore.currentThread!.length - 1" color="ash-grey" class="mt-10 border-opacity-100"></v-divider>
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
    OpenFile(file) {
      console.log(file);
      // Your function logic here
    }
  },
});
</script>
