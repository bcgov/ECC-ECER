<template>
  <h3>{{ messageStore.currentMessage?.subject }}</h3>
  <p class="small mt-2">{{ messageDate }}</p>
  <p class="small mt-6">{{ messageStore.currentMessage?.text }}</p>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useMessageStore } from "@/store/message";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "Message",
  setup() {
    const messageStore = useMessageStore();

    return { messageStore };
  },
  computed: {
    messageDate(): string {
      let message = this.messageStore.currentMessage;
      return message ? formatDate(String(message.notifiedOn), "LLL dd, yyyy t") : "";
    },
  },
  methods: {
    formatDate,
  },
});
</script>
