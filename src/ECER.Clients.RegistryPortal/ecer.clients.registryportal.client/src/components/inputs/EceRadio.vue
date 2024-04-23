<template>
  <div>
    <h3>
    {{ props.title }}
  </h3>
  <br />
  <v-radio-group @update:model-value="(value) => $emit('update:model-value', value as string)">
    <v-radio v-for="option in props.options" :key="option.key" :label="option.label" :value="option.key"></v-radio>
  </v-radio-group>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { EceRadioFieldProps } from "@/types/input";

export default defineComponent({
  name: "EceRadio",
  props: {
    props: {
      type: Object as () => EceRadioFieldProps,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_reference: boolean) => true,
  },
});
</script>
