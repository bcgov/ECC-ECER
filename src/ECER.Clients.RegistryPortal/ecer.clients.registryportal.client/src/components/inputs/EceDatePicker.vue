<template>
  <v-date-picker></v-date-picker>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { Input } from "@/types/form";

export default defineComponent({
  name: "EceDatePicker",
  props: {
    input: {
      type: Object as () => Input,
      required: true,
    },
  },
  emits: ["input"],
});
</script>
