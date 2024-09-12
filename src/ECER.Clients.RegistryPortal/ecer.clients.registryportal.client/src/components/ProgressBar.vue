<template>
  <v-tooltip :text="`${hoursRemaining} Hours Remaining`" location="top">
    <template #activator="{ props }">
      <v-progress-linear v-bind="props" v-model="percentHours" rounded="lg" height="25" color="#67cb7b" bg-color="#f0f2f4" bg-opacity="1">
        <strong>{{ `${Math.ceil(totalHours)}/${hoursRequired} hours` }}</strong>
      </v-progress-linear>
    </template>
  </v-tooltip>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ProgressBar",
  props: {
    totalHours: {
      type: Number,
      required: true,
    },
    hoursRequired: {
      type: Number,
      required: true,
    },
  },
  computed: {
    percentHours(): number {
      return (this.totalHours / this.hoursRequired) * 100;
    },
    hoursRemaining(): number {
      return this.hoursRequired - this.totalHours;
    },
  },
});
</script>
