<template>
  <v-checkbox
    color="primary"
    v-bind:="$attrs"
    :readonly="readonly"
    :model-value="modelValue"
    @click="!readonly && $emit('update:model-value', $event.target.checked)"
  >
    <template #label v-if="$slots.label">
      <slot name="label"></slot>
    </template>
  </v-checkbox>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "EceCheckbox",
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    checkableOnce: {
      type: Boolean,
      default: false,
    },
  },
  emits: {
    "update:model-value": (_checked: Boolean) => true,
  },
  computed: {
    readonly(): boolean {
      return this.checkableOnce && this.modelValue ? true : false;
    },
  },
});
</script>
