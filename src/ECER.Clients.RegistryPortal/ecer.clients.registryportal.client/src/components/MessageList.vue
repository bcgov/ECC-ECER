<template>
  <v-row class="ma-4">
    <v-list lines="two">
      <v-list-item v-for="message in messageStore.messages" :key="message.name" prepend-icon="mdi-message-alert-outline">
        {{ message.value }}
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
});
</script>
