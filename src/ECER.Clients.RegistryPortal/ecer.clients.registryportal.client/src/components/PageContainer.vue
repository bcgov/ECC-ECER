<template>
  <div class="h-screen bg-white">
    <v-container class="py-12">
      <slot></slot>
    </v-container>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "PageContainer",
});
</script>
