<template>
  <v-container :class="{ 'mb-12': true, 'pt-12': marginTop }">
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
  },
});
</script>
