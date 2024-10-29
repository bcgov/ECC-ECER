<template>
  <v-date-input
    :model-value="modelValue"
    :value="formattedDate"
    prepend-icon=""
    :label="label"
    variant="outlined"
    @update:model-value="(value) => $emit('update:model-value', value)"
  />
</template>

<script lang="ts">
import { formatDate } from "@/utils/format";
import { defineComponent, ref } from "vue";

export default defineComponent({
  name: "FormattedDateInput",
  props: {
    modelValue: {
      type: String,
      required: true,
    },
    label: {
      type: String,
      default: "Select Date",
    },
  },
  emits: ["update:model-value"],
  computed: {
    formattedDate() {
      return formatDate(this.modelValue, "LLLL d, yyyy");
    },
  },
});
</script>
