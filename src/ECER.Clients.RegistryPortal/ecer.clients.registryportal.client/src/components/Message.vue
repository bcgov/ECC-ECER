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
          <h2>{{ messageStore.currentMessage?.subject }}</h2>

          <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
            <span v-html="message.from == 'Registry' ? 'From ECE Registry' : 'PortalUser' ? 'You Replied' : ''"></span>
            <br />
            <br />
            <span v-html="formatDate(String(message.notifiedOn), 'LLL d, yyyy t')"></span>
            <br />
            <br />
            <br />
            <span v-html="message.text"></span>
            <hr class="grey-line" />
          </div>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
  <div v-if="mdAndUp">
    <h2>{{ messageStore.currentMessage?.subject }}</h2>
    <div v-for="(message, index) in messageStore.currentThread" :key="index" class="small mt-6">
      <span v-html="message.from == 'Registry' ? 'From ECE Registry' : 'PortalUser' ? 'You Replied' : ''"></span>
      <br />
      <br />
      <span v-html="formatDate(String(message.notifiedOn), 'LLL d, yyyy t')"></span>
      <br />
      <br />
      <br />
      <span v-html="message.text"></span>
      <v-divider class="mt-5"></v-divider>
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
  },
});
</script>
<style>
.grey-line {
  border: 0;
  border-top: 1px solid grey;
  margin: 10px 0; /* Adjust margin as needed */
}
</style>
