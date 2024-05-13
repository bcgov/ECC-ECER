<template>
  <slot name="radioLabel" :for="`${valueKey}Label`"></slot>
  <v-radio-group id="`${valueKey}Label`" v-model="itemSelection" :rules="valueRules" hide-details="auto" @update:model-value="modalValueUpdated">
    <v-radio v-for="(item, index) in items" :key="index" :label="item[itemLabel]" :value="item[itemValue]"></v-radio>
  </v-radio-group>
  <slot v-if="triggerForAdditionalInformation" name="textFieldLabel" :for="`${additionalInfoKey}Label`"></slot>
  <v-text-field
    v-if="triggerForAdditionalInformation"
    :id="`${valueKey}Label`"
    variant="outlined"
    counter="1000"
    color="primary"
    maxlength="1000"
    hide-details="auto"
    :auto-grow="true"
    :rules="additionalInfoRules"
    @update:model-value="$emit('update:model-value', { [additionalInfoKey]: $event })"
  ></v-text-field>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";
import type { RadioWithAdditionalOption } from "@/types/input";

export default defineComponent({
  name: "RadioWithAdditionalOption",
  props: {
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
    valueRules: {
      type: Array as PropType<any>,
      default: () => [],
    },
    additionalInfoRules: {
      type: Array as PropType<any>,
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
  },
  emits: {
    "update:model-value": (_itemSelection: any) => true,
  },
  data() {
    return {
      itemSelection: undefined,
    };
  },
  computed: {
    triggerForAdditionalInformation() {
      return this.triggerValues.includes(this?.itemSelection);
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
