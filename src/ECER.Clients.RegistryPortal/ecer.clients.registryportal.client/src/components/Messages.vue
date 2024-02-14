<template>
  <v-row class="ma-4">
    <v-col cols="12">
      <h1>
        Messages
        <span v-if="messageStore.messageCount > 0">({{ messageStore.messageCount }})</span>
      </h1>
    </v-col>
  </v-row>
  <MessageList />
</template>

<script lang="ts">
import { defineComponent } from "vue";

import MessageList from "@/components/MessageList.vue";
import { useMessageStore } from "@/store/message";

export default defineComponent({
  name: "Messages",
  components: { MessageList },
  setup() {
    const messageStore = useMessageStore();

    return { messageStore };
  },
});
</script>
