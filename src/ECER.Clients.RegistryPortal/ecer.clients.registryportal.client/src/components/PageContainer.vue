<template>
  <div class="bg-white fill-height">
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
