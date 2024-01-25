<template>
  <v-text-field
    :label="props.label"
    variant="outlined"
    color="primary"
    :type="props.type || 'text'"
    :rules="props.rules"
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

import type { Props } from "@/types/form";
import { isNumber } from "@/utils/formInput";

export default defineComponent({
  name: "EceTextField",
  props: {
    props: {
      type: Object as () => Props,
      required: true,
    },
  },
  emits: ["input"],
  methods: {
    isNumber,
  },
});
</script>
