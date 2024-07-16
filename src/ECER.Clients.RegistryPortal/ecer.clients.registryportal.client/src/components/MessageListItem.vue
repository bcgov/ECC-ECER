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
        <svg v-if="!message.acknowledged" class="mr-3" xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15" fill="none" alt="unread">
          <circle cx="7.5" cy="7.5" r="7.5" fill="#1976D2" />
        </svg>
      </div>
    </template>
  </v-list-item>
  <v-divider></v-divider>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import { getChildMessages, markMessageAsRead } from "@/api/message";
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
  emits: ["message-read"],
  setup() {
    const messageStore = useMessageStore();

    return { messageStore };
  },
  computed: {
    messageDate(): string {
      return formatDate(String(this.message.notifiedOn), "LLL d, yyyy t");
    },
  },
  methods: {
    formatDate,
    async handleClick() {
      this.messageStore.currentMessage = this.message;
      this.messageStore.currentThread = (await getChildMessages({ parentId: this.message.id! })).data?.communications;
      if (!this.message.acknowledged) {
        await markMessageAsRead(this.message.id ?? "");
        this.$emit("message-read", this.message.id);
      }
    },
  },
});
</script>
