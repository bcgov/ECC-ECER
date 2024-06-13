<template>
  <v-alert border="start" :title="title" variant="text" :border-color="type" class="border-opacity-100">
    <slot></slot>
  </v-alert>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

export default defineComponent({
  name: "Alert",
  props: {
    type: {
      type: String as PropType<"info" | "warning" | "error" | "success">,
      default: "info",
    },
    title: {
      type: String,
      required: true,
    },
  },
});
</script>
