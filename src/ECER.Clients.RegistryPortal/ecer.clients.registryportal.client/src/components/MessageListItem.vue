<!-- eslint-disable vue/no-v-html -->
<template>
  <v-list-item :key="String(message.id)" :title="String(message.subject)" :subtitle="messageDate" :value="String(message.id)">
    <template #prepend>
      <v-badge color="alternate" inline></v-badge>
    </template>
  </v-list-item>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

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
  computed: {
    messageDate(): string {
      return formatDate(String(this.message.notifiedOn), "LLL dd, yyyy t");
    },
  },
  methods: {
    formatDate,
  },
});
</script>
