<template>
  <v-alert
    :type="type"
    :color="type"
    :icon="icon"
    closable
    variant="outlined"
    :rounded="rounded ? 'lg' : 0"
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
    icon: {
      type: String,
      default: "mdi-alert-circle",
    },
    rounded: {
      type: Boolean,
      default: true,
    },
  },
});
</script>
