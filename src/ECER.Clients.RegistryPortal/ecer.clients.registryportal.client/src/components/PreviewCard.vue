<template>
  <v-card variant="outlined" color="grey-lightest" rounded="lg">
    <slot></slot>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "PreviewCard",
});
</script>
