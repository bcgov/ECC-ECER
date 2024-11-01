<template>
  <p class="mb-2">{{ $attrs.label }}</p>
  <v-text-field
    v-bind="$attrs"
    label=""
    @input="$emit('input', $event.target.value)"
    @keypress="
      if (isNumeric) {
        isNumber($event);
      }
    "
  ></v-text-field>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { isNumber } from "@/utils/formInput";

export default defineComponent({
  name: "EceTextField",
  props: {
    isNumeric: {
      type: Boolean,
      default: false,
    },
  },
  emits: ["input"],
  methods: {
    isNumber,
  },
});
</script>
