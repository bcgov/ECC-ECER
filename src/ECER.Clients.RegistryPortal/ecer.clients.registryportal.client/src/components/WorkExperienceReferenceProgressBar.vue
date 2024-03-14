<template>
  <v-row>
    <v-tooltip :text="`${hoursRemaining} Hours Remaining`" location="top">
      <template #activator="{ props }">
        <v-progress-linear v-bind="props" v-model="percentHours" rounded="lg" height="25" color="#67cb7b" bg-color="#f0f2f4" bg-opacity="1">
          <strong>{{ Math.ceil(totalHours) }}/500 hours</strong>
        </v-progress-linear>
      </template>
    </v-tooltip>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { Components } from "@/types/openapi";

export interface WorkExperienceReferenceData {
  reference: Components.Schemas.WorkExperienceReference;
  referenceId: string | number;
}

export default defineComponent({
  name: "WorkExperienceReferenceProgressBar",
  props: {
    references: {
      type: Object as () => { [id: string]: Components.Schemas.WorkExperienceReference },
      required: true,
    },
  },
  computed: {
    percentHours() {
      return (this.totalHours / 500) * 100;
    },
    hoursRemaining() {
      return 500 - this.totalHours;
    },
    totalHours() {
      return this.referenceList.reduce((acc, reference) => {
        return acc + parseInt(reference.hours as string);
      }, 0);
    },
    referenceList() {
      return Object.values(this.references);
    },
  },
});
</script>
