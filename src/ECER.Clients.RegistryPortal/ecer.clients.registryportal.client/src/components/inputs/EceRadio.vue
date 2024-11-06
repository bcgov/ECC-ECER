<template>
  <div>
    <h2>
      {{ title }}
    </h2>
    <br />
    <v-radio-group :rules="rules" @update:model-value="(value) => $emit('update:model-value', value as string)">
      <v-radio v-for="option in options" :key="option.key" :label="option.label" :value="option.key"></v-radio>
    </v-radio-group>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { EceRadioOption, EceRadioFieldProps } from "@/types/input";

export default defineComponent({
  name: "EceRadio",
  props: {
    title: {
      type: String,
      required: false,
    },
    options: {
      type: Array as () => EceRadioOption[],
      required: false,
    },
    rules: {
      type: Array as () => EceRadioFieldProps["rules"],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_reference: string) => true,
  },
});
</script>
