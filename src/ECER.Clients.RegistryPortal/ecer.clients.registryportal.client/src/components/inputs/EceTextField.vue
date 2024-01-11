<template>
  <v-text-field
    :label="input.label"
    variant="outlined"
    color="primary"
    :type="input.type"
    :rules="input.rules"
    @input="$emit('input', $event.target.value)"
  ></v-text-field>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { Input } from "@/types/form";

export default defineComponent({
  name: "EceTextField",
  props: {
    input: {
      type: Object as () => Input,
      required: true,
    },
  },
  emits: ["input"],
});
</script>
