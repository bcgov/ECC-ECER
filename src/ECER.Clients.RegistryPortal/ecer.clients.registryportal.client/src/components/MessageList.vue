<!-- eslint-disable vue/no-v-html -->
<template>
  <v-row>
    <v-col cols="12" md="6" lg="4">
      <v-list lines="two" class="flex-grow-1 message-list">
        <MessageListItem v-for="(message, index) in messageStore.paginatedMessages" :key="index" :message="message" />
        <v-pagination
          v-if="messageStore.totalPages > 1"
          v-model="currentPage"
          size="small"
          class="mt-4"
          elevation="2"
          :length="messageStore.totalPages"
        ></v-pagination>
      </v-list>
    </v-col>
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
  computed: {
    currentPage: {
      get() {
        return this.messageStore.currentPage;
      },
      set(value: number) {
        this.messageStore.setPage(value);
      },
    },
  },
  created() {
    this.messageStore.fetchMessages();
  },
});
</script>
