<!-- eslint-disable vue/no-v-html -->
<template>
  <v-row>
    <v-list lines="two">
      <MessageListItem v-for="(message, index) in messageStore.messages" :key="index" :message="message" />
    </v-list>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import MessageListItem from "@/components/MessageListItem.vue";
import { useMessageStore } from "@/store/message";

export default defineComponent({
  name: "MessageList",
  components: { MessageListItem },
  setup() {
    const messageStore = useMessageStore();
    return { messageStore };
  },
  created() {
    this.messageStore.fetchMessages();
  },
});
</script>
