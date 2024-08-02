<!-- eslint-disable vue/no-v-html -->
<template>
  <v-list-item
    :key="String(message.id)"
    :title="String(message.subject)"
    :subtitle="messageDate"
    :value="String(message.id)"
    :active="message.id == messageStore.currentMessage?.id"
    @click="handleClick"
  >
    <template #prepend>
      <div class="d-inline-flex flex-nowrap">
        <svg v-if="!message.isRead" class="mr-3" xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15" fill="none" alt="unread">
          <circle cx="7" cy="7" r="7" fill="#1976D2" />
        </svg>
      </div>
    </template>
  </v-list-item>
  <v-divider></v-divider>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import { getChildMessages, markMessageAsRead } from "@/api/message";
import { useAlertStore } from "@/store/alert";
import { useMessageStore } from "@/store/message";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "MessageListItem",
  props: {
    message: {
      type: Object as PropType<Components.Schemas.Communication>,
      required: true,
    },
  },
  setup() {
    const messageStore = useMessageStore();
    const alertStore = useAlertStore();

    return { messageStore, alertStore };
  },
  computed: {
    messageDate(): string {
      return formatDate(String(this.message.latestMessageNotifiedOn), "LLL d, yyyy t");
    },
  },

  methods: {
    formatDate,
    async handleClick() {
      this.messageStore.currentMessage = this.message;
      this.$props.message.isRead = true;
      this.loadChildMessages(this.message.id!);
    },
    async loadChildMessages(messageId: string) {
      this.messageStore.currentThread = (await getChildMessages({ parentId: messageId })).data?.communications;
      // mark unread messages from Registry as read
      if (this.messageStore.currentThread != null) {
        const unreadMessage = this.messageStore.currentThread?.filter((message) => !message.isRead && message.from == "Registry");
        if (unreadMessage) {
          for (const message of unreadMessage) {
            const { error } = await markMessageAsRead(message?.id ?? "");
            if (error) {
              this.alertStore.setFailureAlert("Failed to mark message as read");
            }
          }
        }
      }
    },
  },
});
</script>
