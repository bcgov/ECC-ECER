<template>
  <v-list lines="two" class="flex-grow-1 message-list" style="padding: 0px">
    <MessageListItem
      v-for="(message, index) in messages"
      :key="index"
      :message="message"
      ref="messageListItems"
      @update:message-is-read="message.isRead = $event"
    />
    <v-pagination v-if="messageCount > 1" v-model="currentPage" size="small" class="mt-4" elevation="2" :length="totalPages" />
  </v-list>
</template>

<script lang="ts">
import { ref, computed, onMounted, nextTick } from "vue";
import { getMessages } from "@/api/message";
import MessageListItem from "@/components/MessageListItem.vue";
import { useMessageStore } from "@/store/message";
import type { Components } from "@/types/openapi";
import { useDisplay } from "vuetify";
const PAGE_SIZE = 10;

export default {
  name: "MessageList",
  components: { MessageListItem },
  setup() {
    const messageStore = useMessageStore();
    const messages = ref<Components.Schemas.Communication[]>([]);
    const messageCount = ref(0);
    const { mdAndUp } = useDisplay();
    const page = ref(1);
    // This ref will automatically become an array of MessageListItem component instances.
    const messageListItems = ref<InstanceType<typeof MessageListItem>[]>([]);

    const totalPages = computed(() => Math.ceil(messageCount.value / PAGE_SIZE));

    const currentPage = computed({
      get() {
        return page.value;
      },
      set(newPage: number) {
        page.value = newPage;
        messageStore.$reset();
        fetchMessages(newPage);
      },
    });

    const fetchMessages = async (pageNumber: number) => {
      const params = { page: pageNumber, pageSize: PAGE_SIZE };
      const response = await getMessages(params);
      messages.value = response.data?.communications || [];
      messageCount.value = response.data?.totalMessagesCount || 0;

      window.scrollTo({ top: 0, behavior: "smooth" });
      console.log(mdAndUp.value);
      // After the list is rendered, select/load the first message if available
      await nextTick();
      if (messages.value.length > 0 && messageListItems.value.length > 0 && mdAndUp.value) {
        const firstItem = messageListItems.value[0];
        if (firstItem && typeof firstItem.loadChildMessages === "function" && messages.value[0]?.id) {
          firstItem.loadChildMessages(messages.value[0].id);
        }
      }
    };

    onMounted(async () => {
      messageStore.$reset();
      await fetchMessages(page.value);
    });

    return {
      messages,
      messageCount,
      totalPages,
      currentPage,
      messageListItems,
      fetchMessages,
    };
  },
};
</script>
