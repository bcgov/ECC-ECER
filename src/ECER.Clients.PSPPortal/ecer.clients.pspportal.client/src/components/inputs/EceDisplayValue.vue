<template>
  <v-row no-gutters class="align-baseline">
    <v-col cols="12" sm="4" class="mb-2 mb-sm-0">
      {{ label }}
    </v-col>
    <v-col cols="12" sm="8" class="font-weight-bold text-break">
      {{ displayValue }}
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "EceDisplayValue",
  props: {
    modelValue: {
      type: [String, Number, Date] as PropType<string | number | Date | null | undefined>,
      required: false,
    },
    label: {
      type: String,
      required: true,
    },
    isDate: {
      type: Boolean,
      default: false,
    },
    dateFormat: {
      type: String,
      default: "LLL d, yyyy",
    },
  },
  computed: {
    displayValue(): string {
      if (!this.modelValue) {
        return "";
      }

      if (this.isDate) {
        let dateString: string;
        if (typeof this.modelValue === "string") {
          dateString = this.modelValue;
        } else if (this.modelValue instanceof Date) {
          dateString = this.modelValue.toISOString();
        } else if (typeof this.modelValue === "number") {
          // Handle timestamp
          dateString = new Date(this.modelValue).toISOString();
        } else {
          dateString = String(this.modelValue);
        }
        return formatDate(dateString, this.dateFormat as any);
      }

      return String(this.modelValue);
    },
  },
});
</script>

