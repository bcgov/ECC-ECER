<template>
  <v-expansion-panels :model-value="selected" @update:model-value="handlePanelChange">
    <v-expansion-panel v-for="option in options" :key="option.id" :value="option.id" class="rounded-lg">
      <v-expansion-panel-title>
        <v-radio-group :mandatory="true" :hide-details="true" :model-value="selected" @update:model-value="handleRadioChange">
          <v-radio color="primary" :label="option.title" :value="option.id"></v-radio>
        </v-radio-group>
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <Component :is="option.contentComponent" v-if="option.hasSubSelection" @selection="handleSubSelectionChange" />
        <Component :is="option.contentComponent" v-else />
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
    subSelected: {
      type: Array<string>,
      default: () => [],
    },
    selected: {
      type: String,
      default: null,
    },
  },
  emits: {
    selection: (_selected: string | null) => true,
    subSelection: (_selected: Array<string>) => true,
  },
  methods: {
    handleSubSelectionChange(selected: Array<string>) {
      this.$emit("subSelection", selected);
    },
    handleRadioChange(selected: string | null) {
      this.$emit("selection", selected);
      this.$emit("subSelection", []);
    },
    handlePanelChange(selected: unknown) {
      typeof selected === "string" ? this.$emit("selection", selected) : this.$emit("selection", null);
      this.$emit("subSelection", []);
    },
  },
});
</script>
