<template>
  <label>
    {{ label }}
    <v-date-input
      :model-value="value"
      :value="formattedDate"
      :aria-label="label"
      prepend-icon=""
      label=""
      v-bind="$attrs"
      @update:model-value="(value: string) => updateModelValue(value)"
      :class="['pt-2', $attrs.class]"
    />
  </label>
</template>

<script lang="ts">
import { formatDate } from "@/utils/format";
import { DateTime } from "luxon";
import { defineComponent, type PropType } from "vue";

export default defineComponent({
  name: "EceDateInput",
  props: {
    modelValue: {
      type: String as PropType<string | undefined | null>,
      required: false,
    },
    label: {
      type: String,
      default: "Select Date",
    },
  },
  emits: ["update:model-value"],
  methods: {
    /**
     * Update the model value with string formatted as "yyyy-MM-dd"
     * @param value - JS date value as a string
     */
    updateModelValue(value: string) {
      const luxonDate = DateTime.fromJSDate(new Date(value));
      const formattedDate = luxonDate.toFormat("yyyy-MM-dd");
      this.$emit("update:model-value", formattedDate);
    },
  },
  computed: {
    /**
     * Format the date as "LLLL d, yyyy" to display in the input field
     */
    formattedDate() {
      return formatDate(this.modelValue ?? "", "LLLL d, yyyy");
    },
    value() {
      if (!this.modelValue) {
        return new Date(); // Return current date as default Date object
      }
      // Convert modelValue to a Date object for date picker consistency
      const [year, month, day] = this.modelValue.split("-").map(Number);
      return new Date(year, month - 1, day); // No UTC to keep in local time
    },
  },
});
</script>
