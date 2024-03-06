<!-- eslint-disable vue/no-v-html -->
<template>
  <v-row class="ma-4">
    <v-list lines="two">
      <v-list-item
        v-for="(message, index) in messageStore.messages"
        :key="message.id ? `id-${message.id}` : `index-${index}`"
        prepend-icon="mdi-message-alert-outline"
      >
        <div v-html="message.text"></div>
      </v-list-item>
    </v-list>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useMessageStore } from "@/store/message";

export default defineComponent({
  name: "MessageList",
  setup() {
    const messageStore = useMessageStore();
    return { messageStore };
  },
  created() {
    this.messageStore.fetchMessages();
  },
});
</script>
