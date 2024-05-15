<template>
  <slot name="radioLabel" :for="`${valueKey}Label`"></slot>
  <v-radio-group id="`${valueKey}Label`" :rules="radioRules" hide-details="auto" @update:model-value="modalValueUpdated">
    <v-radio v-for="(item, index) in items" :key="index" :label="item[itemLabel]" :value="item[itemValue]"></v-radio>
  </v-radio-group>
  <slot v-if="triggerForAdditionalInformation" name="textAreaLabel" :for="`${additionalInfoKey}Label`"></slot>
  <v-textarea
    v-if="triggerForAdditionalInformation"
    v-bind="additionalInfoProps"
    :id="`${valueKey}Label`"
    color="primary"
    variant="outlined"
    hide-details="auto"
    @update:model-value="$emit('update:model-value', { [additionalInfoKey]: $event })"
  ></v-textarea>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

import type { RadioWithAdditionalOption, RadioWithAdditionalOptionProps } from "@/types/input";

export default defineComponent({
  name: "RadioWithAdditionalOption",
  props: {
    modelValue: {
      type: String,
      default: undefined,
    },
    items: {
      type: Array as PropType<RadioWithAdditionalOption[]>,
      required: true,
    },
    itemLabel: {
      type: String,
      default: "label",
    },
    itemValue: {
      type: String,
      default: "value",
    },
    radioRules: {
      type: Array as PropType<any[]>,
      default: () => [],
    },
    triggerValues: {
      type: Array,
      default: () => [],
    },
    additionalInfoKey: {
      type: String,
      required: true,
    },
    valueKey: {
      type: String,
      required: true,
    },
    additionalInfoProps: {
      type: Object as PropType<RadioWithAdditionalOptionProps>,
      required: false,
      default: () => {},
    },
  },
  emits: {
    "update:model-value": (_itemSelection: any) => true,
  },
  computed: {
    triggerForAdditionalInformation() {
      return this.triggerValues.includes(this.modelValue);
    },
  },
  methods: {
    modalValueUpdated(value: any) {
      if (this.triggerForAdditionalInformation) {
        this.$emit("update:model-value", { [this.valueKey]: value });
      } else {
        this.$emit("update:model-value", { [this.additionalInfoKey]: "", [this.valueKey]: value });
      }
    },
  },
});
</script>
