<template>
  <v-list lines="two" class="flex-grow-1 message-list">
    <MessageListItem v-for="(message, index) in messages" :key="index" :message="message" @message-read="fetchMessages(page)" />
    <v-pagination v-if="messageCount > 1" v-model="currentPage" size="small" class="mt-4" elevation="2" :length="totalPages"></v-pagination>
  </v-list>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getMessages } from "@/api/message";
import MessageListItem from "@/components/MessageListItem.vue";
import { useMessageStore } from "@/store/message";
import type { Components } from "@/types/openapi";

const PAGE_SIZE = 10;

export default defineComponent({
  name: "MessageList",
  components: { MessageListItem },
  setup() {
    const messageStore = useMessageStore();

    return { messageStore };
  },
  data() {
    return {
      messages: [] as Components.Schemas.Communication[],
      messageCount: 0,
      page: 1,
    };
  },
  computed: {
    totalPages() {
      return Math.ceil(this.messageCount / PAGE_SIZE);
    },
    currentPage: {
      get() {
        return this.page;
      },
      set(newValue: number) {
        this.page = newValue;
        this.messageStore.currentMessage = null;
        this.messageStore.currentThread = null;
        this.fetchMessages(newValue);
        window.scrollTo({
          top: 0,
          behavior: "smooth",
        });
      },
    },
  },
  mounted() {
    this.fetchMessages(this.page);
  },
  methods: {
    async fetchMessages(page: number) {
      const params = {
        page,
        pageSize: PAGE_SIZE,
      };
      const response = await getMessages(params);
      this.messages = response.data?.communications || [];
      this.messageCount = response.data?.totalMessagesCount || 0;
    },
  },
});
</script>
