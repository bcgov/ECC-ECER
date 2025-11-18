<template>
  <svg width="36" height="4">
    <rect width="36" height="4" fill="#FFC72C" />
  </svg>
  <h2>{{ title }}</h2>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ECEHeader",
  props: {
    title: {
      type: String,
      required: true,
    },
  },
});
</script>
