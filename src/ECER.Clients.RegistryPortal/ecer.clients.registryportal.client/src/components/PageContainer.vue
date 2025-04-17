<template>
  <v-container :class="{ 'mb-12': marginBottom, 'pt-12': marginTop }">
    <slot></slot>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "PageContainer",
  props: {
    marginTop: {
      type: Boolean,
      default: true,
    },
    marginBottom: {
      type: Boolean,
      default: true,
    },
  },
});
</script>
