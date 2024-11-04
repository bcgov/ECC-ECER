<template>
  <p class="mb-2">{{ label }}</p>
  <v-date-input
    :model-value="new Date(modelValue)"
    :value="formattedDate"
    prepend-icon=""
    label=""
    v-bind="$attrs"
    @update:model-value="(value: string) => updateModelValue(value)"
  />
</template>

<script lang="ts">
import { formatDate } from "@/utils/format";
import { DateTime } from "luxon";
import { defineComponent } from "vue";

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
  methods: {
    updateModelValue(value: string) {
      const luxonDate = DateTime.fromJSDate(new Date(value));
      const formattedDate = luxonDate.toFormat("yyyy-MM-dd");
      this.$emit("update:model-value", formattedDate);
    },
  },
  computed: {
    formattedDate() {
      return formatDate(this.modelValue, "LLLL d, yyyy");
    },
  },
});
</script>
