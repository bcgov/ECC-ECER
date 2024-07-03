<!-- eslint-disable vue/no-v-html -->
<template>
  <v-list lines="two" class="flex-grow-1 message-list">
    <MessageListItem v-for="(message, index) in Messages" :key="index" :message="message" />
    <v-pagination v-if="MessageCount > 1" v-model="currentPage" size="small" class="mt-4" elevation="2" :length="totalPages"></v-pagination>
  </v-list>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getMessages } from "@/api/message";
import MessageListItem from "@/components/MessageListItem.vue";

const PAGE_SIZE = 10;

export default defineComponent({
  name: "MessageList",
  components: { MessageListItem },
  setup: async () => {
    const params = {
      page: 1,
      pageSize: PAGE_SIZE,
    };
    const Messages = (await getMessages(params)).data?.communications;
    const MessageCount = (await getMessages(params)).data?.messageCount!;

    return { Messages, MessageCount };
  },
  data() {
    return {
      page: 0,
    };
  },
  computed: {
    totalPages(): number {
      return Math.ceil((this.MessageCount ?? 0) / PAGE_SIZE);
    },
    currentPage: {
      get() {
        return this.page;
      },
      set(newValue: number) {
        this.page = newValue;
        this.onPageChange(newValue);
      },
    },
  },

  methods: {
    async onPageChange(newPage: number) {
      this.Messages = (await getMessages({ page: newPage, pageSize: PAGE_SIZE })).data?.communications;
    },
  },
});
</script>
