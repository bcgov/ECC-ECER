<template>
  <v-text-field
    :label="props.label"
    variant="outlined"
    color="primary"
    :type="props.type || 'text'"
    :rules="props.rules"
    @input="$emit('input', $event.target.value)"
  ></v-text-field>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { Props } from "@/types/form";

export default defineComponent({
  name: "EceTextField",
  props: {
    props: {
      type: Object as () => Props,
      required: true,
    },
  },
  emits: ["input"],
});
</script>
