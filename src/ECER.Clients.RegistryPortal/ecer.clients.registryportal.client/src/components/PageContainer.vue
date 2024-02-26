<template>
  <v-container class="my-6 px-0">
    <slot></slot>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "PageContainer",
});
</script>
