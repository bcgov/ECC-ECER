<template>
  <v-checkbox color="primary" :label="props.label" :disabled="props.disabled" :rules="props.rules"></v-checkbox>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { EceCheckboxProps } from "@/types/input";

export default defineComponent({
  name: "EceCheckbox",
  props: {
    props: {
      type: Object as () => EceCheckboxProps,
      required: true,
    },
  },
});
</script>
