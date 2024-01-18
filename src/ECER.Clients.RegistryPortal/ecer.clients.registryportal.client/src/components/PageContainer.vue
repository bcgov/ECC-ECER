<template>
  <v-container class="my-6">
    <slot></slot>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "PageContainer",
});
</script>
