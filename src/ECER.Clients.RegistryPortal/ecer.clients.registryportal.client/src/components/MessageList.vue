<template>
  <v-list lines="two" class="flex-grow-1 message-list">
    <MessageListItem v-for="(message, index) in messages" :key="index" :message="message" />
    <v-pagination v-if="messageCount > 1" v-model="currentPage" size="small" class="mt-4" elevation="2" :length="totalPages"></v-pagination>
  </v-list>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, ref } from "vue";

import { getMessages } from "@/api/message";
import MessageListItem from "@/components/MessageListItem.vue";
import type { Components } from "@/types/openapi";

const PAGE_SIZE = 10;

export default defineComponent({
  name: "MessageList",
  components: { MessageListItem },
  setup() {
    const messages = ref<Components.Schemas.Communication[]>([]);
    const messageCount = ref(0);
    const page = ref(1);

    const fetchMessages = async (page: number) => {
      const params = {
        page,
        pageSize: PAGE_SIZE,
      };
      const response = await getMessages(params);
      messages.value = response.data?.communications || [];
      messageCount.value = response.data?.messageCount || 0;
    };

    onMounted(() => {
      fetchMessages(page.value);
    });

    const totalPages = computed(() => Math.ceil(messageCount.value / PAGE_SIZE));

    const currentPage = computed({
      get() {
        return page.value;
      },
      set(newValue: number) {
        page.value = newValue;
        fetchMessages(newValue);
      },
    });

    return {
      messages,
      messageCount,
      currentPage,
      totalPages,
    };
  },
});
</script>
