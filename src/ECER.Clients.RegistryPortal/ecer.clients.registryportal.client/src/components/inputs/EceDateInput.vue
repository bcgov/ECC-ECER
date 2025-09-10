<template>
  <label>
    {{ label }}
    <v-date-input
      ref="dateInput"
      :model-value="modelValue"
      :aria-label="label"
      prepend-icon=""
      label=""
      v-bind="$attrs"
      @update:model-value="(value: any) => updateModelValue(value)"
      :display-format="format"
      :class="['pt-2', $attrs.class]"
    />
  </label>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { defineComponent, type PropType } from "vue";
import type { VInput } from "vuetify/components";

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
    format: {
      type: String,
      default: "fullDate",
      required: false,
    },
  },
  emits: ["update:model-value"],
  methods: {
    /**
     * Update the model value with string formatted as "yyyy-MM-dd"
     * @param value - JS date value as a string
     */
    updateModelValue(value: Date) {
      const luxonDate = DateTime.fromJSDate(new Date(value));
      const formattedDate = luxonDate.toFormat("yyyy-MM-dd");
      this.$emit("update:model-value", formattedDate);
    },
    async validate() {
      await (this.$refs.dateInput as VInput).validate();
    },
  },
});
</script>
