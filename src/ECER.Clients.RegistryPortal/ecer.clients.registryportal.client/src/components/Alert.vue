<template>
  <v-alert
    :type="type"
    :color="type"
    closable
    variant="outlined"
    rounded="lg"
    :class="{
      error: type === 'error',
      warning: type === 'warning',
      success: type === 'success',
      info: type === 'info',
    }"
  >
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
  },
});
</script>
