<template>
  <v-row class="ga-4">
    <v-expansion-panels v-for="option in options" :key="option.id">
      <v-expansion-panel class="rounded-lg">
        <v-expansion-panel-title>
          <v-radio :label="option.title" :value="option.id"></v-radio>
        </v-expansion-panel-title>
        <v-expansion-panel-text>
          <Component :is="option.contentComponent" />
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { ExpandSelectOption } from "@/types/expand-select";

export default defineComponent({
  name: "ExpandSelect",
  props: {
    selected: {
      type: Array as () => string[],
      default: () => [],
    },
    multiselect: {
      type: Boolean,
      default: false,
    },
    options: {
      type: Array as () => ExpandSelectOption[],
      default: () => [],
    },
  },
});
</script>
