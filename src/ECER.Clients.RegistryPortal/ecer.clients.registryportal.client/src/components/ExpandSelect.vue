<template>
  <v-expansion-panels :model-value="selected" @update:model-value="handlePanelChange">
    <v-expansion-panel v-for="option in options" :key="option.id" :value="option.id" class="rounded-lg">
      <v-expansion-panel-title>
        <v-radio-group :mandatory="true" :hide-details="true" :model-value="selected" @update:model-value="handleRadioChange">
          <v-radio :label="option.title" :value="option.id"></v-radio>
        </v-radio-group>
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <Component :is="option.contentComponent" />
      </v-expansion-panel-text>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { ExpandSelectOption } from "@/types/expand-select";

export default defineComponent({
  name: "ExpandSelect",
  props: {
    options: {
      type: Array as () => ExpandSelectOption[],
      default: () => [],
    },
    selected: {
      type: String,
      default: null,
    },
  },
  emits: ["selection"],
  methods: {
    handleRadioChange(selected: string | null) {
      this.$emit("selection", selected);
    },
    handlePanelChange(selected: unknown) {
      this.$emit("selection", selected);
    },
  },
});
</script>
