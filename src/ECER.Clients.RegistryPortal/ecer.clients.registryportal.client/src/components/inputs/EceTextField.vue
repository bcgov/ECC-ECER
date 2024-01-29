<template>
  <v-text-field
    :label="props.label"
    variant="outlined"
    color="primary"
    :type="props.type || 'text'"
    :rules="props.rules"
    :disabled="props.disabled"
    :maxlength="props.maxLength"
    @input="$emit('input', $event.target.value)"
    @keypress="
      if (props.isNumeric) {
        isNumber($event);
      }
    "
  ></v-text-field>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { EceTextFieldProps } from "@/types/input";
import { isNumber } from "@/utils/formInput";

export default defineComponent({
  name: "EceTextField",
  props: {
    props: {
      type: Object as () => EceTextFieldProps,
      required: true,
    },
  },
  emits: ["input"],
  methods: {
    isNumber,
  },
});
</script>
