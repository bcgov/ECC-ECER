<template>
  <v-checkbox v-if="selectAll" :model-value="allSelected" label="Select All" :hide-details="true" density="compact" @click="selectAllToggle"></v-checkbox>
  <v-input :model-value="itemSelection" :rules="rules">
    <div>
      <v-checkbox
        v-for="(item, index) in items"
        :key="index"
        v-model="itemSelection"
        :label="item[itemLabel]"
        :value="item[itemValue]"
        :hide-details="true"
        density="compact"
      ></v-checkbox>
    </div>
  </v-input>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";

import type { CheckboxMultipleDropdownItems } from "@/types/input";

export default defineComponent({
  name: "CheckboxMultiple",
  props: {
    items: {
      type: Array as PropType<CheckboxMultipleDropdownItems[]>,
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
    rules: {
      type: Array as PropType<any>,
      default: null,
    },
    selectAll: {
      type: Boolean,
      default: false,
    },
  },
  emits: {
    "update:model-value": (_itemSelection: any) => true,
  },
  data() {
    return {
      itemSelection: [],
    };
  },
  computed: {
    allSelected() {
      return this.itemSelection.length === this.$props.items.length;
    },
    someSelected() {
      return this.itemSelection.length > 0;
    },
  },
  watch: {
    itemSelection() {
      this.$emit("update:model-value", this.itemSelection);
    },
  },
  methods: {
    selectAllToggle() {
      if (this.allSelected) {
        this.itemSelection = [];
      } else {
        this.itemSelection = this.$props.items.map((item: any) => item?.[this.$props.itemValue]) as [];
      }
    },
  },
});
</script>
