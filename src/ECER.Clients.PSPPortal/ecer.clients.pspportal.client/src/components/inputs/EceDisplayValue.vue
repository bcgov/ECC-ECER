<template>
  <div class="ece-display-value">
    <div class="ece-display-value__label">{{ label }}</div>
    <div class="ece-display-value__value font-weight-bold">
      {{ displayValue }}
    </div>
  </div>
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

<style scoped>
.ece-display-value {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.ece-display-value__label {
  flex-shrink: 0;
}

.ece-display-value__value {
  word-break: break-word;
}

@media (min-width: 600px) {
  .ece-display-value {
    flex-direction: row;
    align-items: baseline;
    gap: 1rem;
  }
  
  .ece-display-value__label {
    min-width: 150px;
  }
  
  .ece-display-value__value {
    flex: 1;
  }
}
</style>

