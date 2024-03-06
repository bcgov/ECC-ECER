<template>
  <v-checkbox
    color="primary"
    :label="props.label"
    :disabled="props.disabled"
    :readonly="readonly"
    :rules="props.rules"
    :model-value="modelValue"
    @click="readonly ?? $emit('update:model-value', $event.target.checked)"
  ></v-checkbox>
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
    modelValue: {
      type: Boolean,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_checked: Boolean) => true,
  },
  computed: {
    readonly(): boolean {
      return this.props.checkableOnce && this.modelValue ? true : false;
    },
  },
});
</script>
