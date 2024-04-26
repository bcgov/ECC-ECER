<!-- eslint-disable vue/no-v-html -->
<template>
  <h3>{{ messageStore.messageById(id)?.subject }}</h3>
  <p class="small mt-2">{{ messageDate }}</p>
  <p class="small mt-6">{{ messageStore.messageById(id)?.text }}</p>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useMessageStore } from "@/store/message";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "Message",
  props: {
    id: {
      type: String,
      required: true,
    },
  },
  setup() {
    const messageStore = useMessageStore();

    return { messageStore };
  },
  computed: {
    messageDate(): string {
      let message = this.messageStore.messageById(this.id);
      return message ? formatDate(String(message.notifiedOn), "LLL dd, yyyy t") : "";
    },
  },
  methods: {
    formatDate,
  },
});
</script>
