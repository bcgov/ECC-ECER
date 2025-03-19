<template>
  <v-list lines="two" class="flex-grow-1 message-list" style="padding: 0px">
    <<<<<<< HEAD
    <MessageListItem v-for="(message, index) in messages" :key="index" :message="message" @update:message-is-read="message.isRead = $event" />
    <v-pagination v-if="messageCount > 1" v-model="currentPage" size="small" class="mt-4" elevation="2" :length="totalPages"></v-pagination>
    =======
    <MessageListItem
      v-for="(message, index) in messages"
      :key="index"
      :message="message"
      ref="messageListItems"
      @update:message-is-read="message.isRead = $event"
    />
    <v-pagination v-if="messageCount > 1" v-model="currentPage" size="small" class="mt-4" elevation="2" :length="totalPages"></v-pagination>
  </v-list>
</template>

<script lang="ts">
import { defineComponent, nextTick } from "vue";
import { getMessages } from "@/api/message";
import { useDisplay } from "vuetify";
import MessageListItem from "@/components/MessageListItem.vue";
import { useMessageStore } from "@/store/message";
import type { Components } from "@/types/openapi";

const PAGE_SIZE = 10;

export default defineComponent({
  name: "MessageList",
  components: { MessageListItem },
  setup() {
    const messageStore = useMessageStore();
    const { mdAndUp } = useDisplay();
    return { messageStore, mdAndUp };
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
        this.messageStore.$reset();
        this.fetchMessages(newValue);
      },
    },
  },
  async mounted() {
    this.messageStore.$reset();
    await this.fetchMessages(this.page);
  },
  methods: {
    async fetchMessages(page: number) {
      const params = { page, pageSize: PAGE_SIZE };
      const response = await getMessages(params);
      this.messages = response.data?.communications || [];
      this.messageCount = response.data?.totalMessagesCount || 0;

      window.scrollTo({ top: 0, behavior: "smooth" });

      // After the list is rendered, select/load the first message if available
      await nextTick();
      const messageListItems = this.$refs.messageListItems as Array<{ loadChildMessages: (id: string) => void }>;
      if (this.messages.length > 0 && messageListItems && this.mdAndUp) {
        const firstItem = messageListItems[0];
        if (firstItem && typeof firstItem.loadChildMessages === "function" && this.messages[0]?.id) {
          firstItem.loadChildMessages(this.messages[0].id);
        }
      }
    },
  },
});
</script>
