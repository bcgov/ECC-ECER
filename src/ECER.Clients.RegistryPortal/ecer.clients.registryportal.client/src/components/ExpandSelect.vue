<template>
  <v-expansion-panels v-model="selection" variant="accordion">
    <v-expansion-panel v-for="option in options" :key="option.id" :value="option.id" class="rounded-lg">
      <v-expansion-panel-title>
        <v-row no-gutters>
          <v-col cols="12">
            <v-radio-group v-model="selection" :mandatory="true" :hide-details="true">
              <v-radio color="primary" :label="option.title" :value="option.id"></v-radio>
            </v-radio-group>
          </v-col>
          <v-col v-if="option.id === 'FiveYears' && selection !== 'FiveYears'" cols="11" offset="1">
            <p class="small">If you are eligible for an ECE Five Year Certificate, you may also be eligible for one or both specializations:</p>
            <v-checkbox
              v-model="certificationTypeStore.subSelection"
              color="primary"
              label="Infant and Toddler Educator (ITE)"
              value="Ite"
              hide-details="auto"
            ></v-checkbox>
            <v-checkbox
              v-model="certificationTypeStore.subSelection"
              color="primary"
              label="Special Needs Educator (SNE)"
              value="Sne"
              hide-details="auto"
            ></v-checkbox>
          </v-col>
        </v-row>
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <Component :is="option.contentComponent" />
      </v-expansion-panel-text>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useCertificationTypeStore } from "@/store/certificationType";
import type { ExpandSelectOption } from "@/types/expand-select";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "ExpandSelect",
  props: {
    options: {
      type: Array as () => ExpandSelectOption[],
      default: () => [],
    },
  },
  setup: () => {
    const certificationTypeStore = useCertificationTypeStore();

    return { certificationTypeStore };
  },
  computed: {
    selection: {
      get() {
        return this.certificationTypeStore.selection;
      },
      set(newValue: Components.Schemas.CertificationType) {
        if (newValue !== "FiveYears") {
          this.certificationTypeStore.subSelection = [];
        }
        this.certificationTypeStore.selection = newValue;
      },
    },
  },
});
</script>
